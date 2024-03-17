using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using static PopugCommon.KafkaMessages.KafkaMessages;

namespace PopugAnalitics.Logic
{
    public partial class AnaliticsLogic
    {
        private readonly DataContext dataContext;

        public AnaliticsLogic(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task Add(BalanceTransactionDb transaction)
        {
            dataContext.Add(transaction);
            await dataContext.SaveChangesAsync();
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
                taskDb.Title = task.Title;
                taskDb.Description = task.Description;
                taskDb.AssignedUserId = task.AssignedUserId;
                taskDb.IsCompleted = task.IsCompleted;
                await dataContext.PopugTasks.AddAsync(taskDb);
                await dataContext.SaveChangesAsync();
            }

            return await GetTask(task.Id);
        }
        public async Task<TaskDb> AddOrUpdateTaskCosts(TaskDb task)
        {
            var taskDb = await GetTask(task.PublicId);
            if (taskDb != null)
            {
                taskDb.PublicId = task.PublicId;
                taskDb.Fee = task.Fee;
                taskDb.Amount = task.Amount;

                dataContext.PopugTasks.Update(taskDb);
                await dataContext.SaveChangesAsync();
            }
            else
            {
                var rnd = new Random();
                taskDb = new TaskDb();
                taskDb.PublicId = task.PublicId;
                taskDb.Fee = task.Fee;
                taskDb.Amount = task.Amount;
                await dataContext.PopugTasks.AddAsync(taskDb);
                await dataContext.SaveChangesAsync();
            }

            return await GetTask(task.Id);
        }

        public async Task SaveEvent(TaskEventsDb e)
        {
            dataContext.Add(e);
            await dataContext.SaveChangesAsync();
        }
        public async Task SaveEvent(List<TaskEventsDb> e)
        {
            await dataContext.AddRangeAsync(e);
            await dataContext.SaveChangesAsync();
        }



        public async Task<List<ExpensiveTaskResult>> GetExpensiveTasks(DateTime from, DateTime to)
        {
            var completedEvents = await dataContext.PopugTaskEvents.Where(e => e.Event == KafkaMessages.Tasks.Completed && e.EventDate >= from && e.EventDate <= to).ToListAsync();
            var tasks = await dataContext.PopugTasks.Where(t => completedEvents.Select(e => e.PublicId).Contains(t.PublicId)).ToListAsync();

            return completedEvents.GroupBy(e => e.EventDate.ToShortDateString()).Select(g => new ExpensiveTaskResult() { Date = g.Key, Money = g.Max(e => tasks.Single(t => t.PublicId == e.PublicId).Fee) }).ToList();
        }
        public async Task<int> GetTopManagementMoneyForToday()
        {
            var assignedSum = dataContext.BalanceTransactionsLog.Where(b => b.Type == TransactionType.Assign && b.Date >= DateTime.Now.Date && b.Date < DateTime.Now.Date.AddDays(1)).Sum(t=> t.Money);
            var completedSum = dataContext.BalanceTransactionsLog.Where(b => b.Type == TransactionType.Complete && b.Date >= DateTime.Now.Date && b.Date < DateTime.Now.Date.AddDays(1)).Sum(t => t.Money);
            return Math.Abs(assignedSum) - completedSum;
        }
        public async Task<List<string>> GetPopugsInMinusForToday()
        {
            var users = await dataContext.BalanceTransactionsLog.Where(b => (b.Type == TransactionType.Assign || b.Type == TransactionType.Complete) && b.Date >= DateTime.Now.Date && b.Date < DateTime.Now.Date.AddDays(1)).GroupBy(t=>t.UserId).Select(g=> new { UserId = g.Key, Money = g.Sum(t=> t.Money) }).ToListAsync();
            return users.Where(u => u.Money < 0).Select(u => u.UserId).ToList();
        }
    }
}