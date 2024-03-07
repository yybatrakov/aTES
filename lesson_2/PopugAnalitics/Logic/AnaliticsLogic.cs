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
using static PopugCommon.KafkaMessages.Messages;

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
            dataContext.SaveChanges();
        }


        public async Task<List<ExpensiveTaskResult>> GetExpensiveTasks(DateTime from, DateTime to)
        {
            var transactions =  await dataContext.BalanceTransactionsLog.Where(b => b.Type == TransactionType.Complete && b.Date >= from && b.Date <= to).ToListAsync();
            return transactions.GroupBy(t => t.Date.ToShortDateString()).Select(g => new ExpensiveTaskResult() { Date = g.Key, Money = g.Max(t => t.Money) }).ToList();
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