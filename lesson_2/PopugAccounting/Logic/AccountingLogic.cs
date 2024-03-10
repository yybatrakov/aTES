using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PopugAccounting.Logic
{
    public class AccountingLogic
    {
        private readonly DataContext dataContext;

        public AccountingLogic(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<TaskDb> GetTask(string public_id)
        {
            return await dataContext.PopugTasks.Where(t => t.PublicId == public_id).FirstOrDefaultAsync();
        }
        public async Task<TaskDb> AddOrUpdateTask(TaskDb task)
        {
            var taskDb = await GetTask(task.PublicId);
            if (taskDb != null)
            {
                taskDb.Title = task.Title ?? taskDb.Title;
                taskDb.AssignedUserId = task.AssignedUserId ?? taskDb.AssignedUserId;
                taskDb.Description = task.Description ?? taskDb.Description;
                taskDb.IsCompleted = task.IsCompleted;
                dataContext.PopugTasks.Update(taskDb);
                await dataContext.SaveChangesAsync();
            }
            else
            {
                var rnd = new Random();
                taskDb = new TaskDb();
                taskDb.Fee = rnd.Next(10, 20);
                taskDb.Amount = rnd.Next(20, 40);
                taskDb.Title = task.Title;
                taskDb.Description = task.Description;
                taskDb.AssignedUserId = task.AssignedUserId;
                taskDb.IsCompleted = task.IsCompleted;
                await dataContext.PopugTasks.AddAsync(taskDb);
                await dataContext.SaveChangesAsync();
                await Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new PopugMessage(task, Messages.Tasks.Stream.Updated, "v1"));
            }

            return await GetTask(task.PublicId);
        }


        public async Task ProcessPayment()
        {
            var balances = await dataContext.Balances.Where(b => b.Money > 0).ToListAsync();
            var transactions =  balances.Select(b => new BalanceTransactionDb()
            {
                UserId = b.UserId,
                Type = TransactionType.Payment,
                Date = DateTime.Now,
                Money = b.Money
            }).ToList();
            transactions.ForEach(async t => await UpdateBalance(t));
        }
        public async Task CreateBalance(string userId)
        {
            var balance = await dataContext.Balances.Where(b => b.UserId == userId).SingleOrDefaultAsync();
            if (balance != null)
                return;

            balance = new BalanceDb() { UserId = userId, Money = 0 };
            await dataContext.Balances.AddAsync(balance);
            dataContext.SaveChanges();
            
        }
        public async Task UpdateBalance(BalanceTransactionDb transaction)
        {
            var balance = await dataContext.Balances.Where(b => b.UserId == transaction.UserId).SingleAsync();
            balance.Money = balance.Money + transaction.Money;
            dataContext.BalanceTransactions.Add(transaction);
            dataContext.Balances.Update(balance);
            await dataContext.SaveChangesAsync();

            var transactionEvent = new BalanceTransactionStreamEvent()
            {
                id = transaction.id,
                UserId = transaction.UserId,
                Money = balance.Money,
                Date = transaction.Date,
                Type = transaction.Type
            };

            await Kafka.Produce(KafkaTopics.BalanceTransactionStream, balance.Id.ToString(), new PopugMessage(transactionEvent, Messages.BalanceTransaction.Stream.Created, "v1"));
        }
        public Task SendPaymentNotification(BalancePaymentProcessedEvent balancePaymentProcessedEvent)
        {
            balancePaymentProcessedEvent.Transactions.ForEach(t => {
                //Из требований не понял кому нужно письмо. Оно тут отсылается
            } );
            return Task.CompletedTask;
        }
    }
}
