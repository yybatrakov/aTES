using Microsoft.EntityFrameworkCore;
using PopugAccounting.Controllers;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PopugCommon.KafkaMessages.KafkaMessages;

namespace PopugAccounting.Logic
{
    public class AccountingLogic
    {
        private readonly DataContext dataContext;

        public AccountingLogic(DbContextOptions options)
        {
            this.dataContext = new DataContext(options);
        }

        public async Task<TaskDb> GetTask(string public_id)
        {
            return await dataContext.PopugTasks.Where(t => t.PublicId == public_id).FirstOrDefaultAsync();
        }

        public async Task<TaskDb> GetTask(int id)
        {
            return await dataContext.PopugTasks.Where(t => t.Id == id).FirstOrDefaultAsync();
        }
        public async Task<TaskDb> AddOrUpdateTask(TaskDb task)
        {
            var taskDb = await GetTask(task.PublicId);
            if (taskDb != null)
            {
                taskDb.PublicId = task.PublicId;
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
                taskDb.PublicId = task.PublicId;
                taskDb.Fee = rnd.Next(10, 20);
                taskDb.Amount = rnd.Next(20, 40);
                taskDb.Title = task.Title;
                taskDb.Description = task.Description;
                taskDb.AssignedUserId = task.AssignedUserId;
                taskDb.IsCompleted = task.IsCompleted;
                await dataContext.PopugTasks.AddAsync(taskDb);
                await dataContext.SaveChangesAsync();
            }

            return await GetTask(task.Id);
        }


        public async Task ProcessPayment()
        {
            var balances = await dataContext.Balances.Where(b => b.Money > 0).ToListAsync();
            var transactions =  balances.Select(b => new BalanceTransactionDb()
            {
                UserId = b.UserId,
                Type = TransactionType.Payment,
                Date = DateTime.Now,
                Money = -b.Money
            }).ToList();
            transactions.ForEach(async t => await UpdateBalance(t));
            var e = new BalancePaymentProcessedEvent() { Transactions = transactions.Select(t => new BalancePaymentTransaction() { UserId = t.UserId, Money = t.Money } ).ToList()};
            await Kafka.Produce(KafkaTopics.BalanceLifecycle, DateTime.Now.Ticks.ToString(), new PopugMessage(e, KafkaMessages.Balances.PaymentProcessd, "v1"));
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
                Money = transaction.Money,
                Date = transaction.Date,
                Type = transaction.Type
            };

            await Kafka.Produce(KafkaTopics.BalanceTransactionStream, balance.Id.ToString(), new PopugMessage(transactionEvent, KafkaMessages.BalanceTransaction.Stream.Created, "v1"));
        }
        public Task SendPaymentNotification(BalancePaymentProcessedEvent balancePaymentProcessedEvent)
        {
            balancePaymentProcessedEvent.Transactions.ForEach(t => {
                //Из требований не понял кому нужно письмо. Оно тут отсылается
            } );
            return Task.CompletedTask;
        }

        public async Task<List<TopManagementStatisticResponse>> GetTopManagementMoneyStatistics()
        {
            return await dataContext.BalanceTransactions.Where(b => (b.Type == TransactionType.Assign || b.Type == TransactionType.Complete)).GroupBy(t => t.Date.Date).Select(g => new TopManagementStatisticResponse() { Date = g.Key, Money = -g.Sum(t => t.Money) }).ToListAsync(); 
        }
        

        public Task<BalanceDb> GetBalance(string userId)
        {
            return dataContext.Balances.Where(b => b.UserId == userId).SingleOrDefaultAsync();
        }
        public Task<List<BalanceTransactionDb>> GetBalanceTransactions(string userId)
        {
            return dataContext.BalanceTransactions.Where(b => b.UserId == userId).ToListAsync();
        }
    }
}
