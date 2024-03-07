using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Linq;
using System.Threading.Tasks;
using static PopugCommon.KafkaMessages.Messages;

namespace PopugAnalitics.Logic
{
    public class AnaliticsLogic
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


        public Task GetExpensiveTask()
        {
            return Task.CompletedTask;
        }
    }
}