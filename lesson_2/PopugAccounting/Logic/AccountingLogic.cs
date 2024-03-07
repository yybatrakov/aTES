using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            return await dataContext.PopugTasks.Where(t => t.PublicId == t.PublicId).FirstOrDefaultAsync();
        }
        public async Task<TaskDb> AddOrUpdateTask(PopugTaskStreamEvent task)
        {
            var taskDb = await GetTask(task.PublicId);
            if (taskDb != null)
            {
                taskDb.AssignedUserId = task.AssignedUserId;
                taskDb.Description = task.Description;
                dataContext.PopugTasks.Update(taskDb);
                await dataContext.SaveChangesAsync();
            }
            else
            {
                var rnd = new Random();
                taskDb = new TaskDb();
                taskDb.Fee = rnd.Next(10, 20);
                taskDb.Amount = rnd.Next(20, 40);
                taskDb.Description = task.Description;
                taskDb.AssignedUserId = task.AssignedUserId;

                await dataContext.PopugTasks.AddAsync(taskDb);
                await dataContext.SaveChangesAsync();
                await Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new PopugMessage(task, Messages.Tasks.Stream.Updated, "v1"));
            }

            return await GetTask(task.PublicId);
        }
        public async Task ProcessPayment()
        {
            var balances = await dataContext.Balances.Where(b => b.Money > 0).ToListAsync();
            foreach (var balance in balances)
            {
                await UpdateBalance(new BalanceTransaction()
                {
                    UserId = balance.UserId,
                    Type = TransactionType.Payment,
                    Date = DateTime.Now,
                    Money = balance.Money
                });
            }
        }
        public async Task CreateBalance(string userId)
        {
            var balance = await dataContext.Balances.Where(b => b.UserId == userId).SingleOrDefaultAsync();
            if (balance != null)
                return;

            balance = new Balance() { UserId = userId, Money = 0 };
            await dataContext.Balances.AddAsync(balance);
            dataContext.SaveChanges();
            await Kafka.Produce(KafkaTopics.BalanceStream, balance.Id.ToString(), new PopugMessage(balance, Messages.Balances.Stream.Created, "v1"));
        }
        public async Task UpdateBalance(BalanceTransaction transaction)
        {
            var balance = await dataContext.Balances.Where(b => b.UserId == transaction.UserId).SingleAsync();
            balance.Money = balance.Money + transaction.Money;
            dataContext.BalanceTransactions.Add(transaction);
            dataContext.Balances.Update(balance);
            await dataContext.SaveChangesAsync();
            
            await Kafka.Produce(KafkaTopics.BalanceStream, balance.Id.ToString(), new PopugMessage(balance, Messages.Balances.Stream.Updated, "v1"));
            await Kafka.Produce(KafkaTopics.BalanceStream, balance.Id.ToString(), new PopugMessage(transaction, Messages.BalanceTransaction.Stream.Created, "v1"));
        }
    }
}
