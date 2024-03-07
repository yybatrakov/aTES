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

        public async Task ProcessPayment()
        {
            var balances = dataContext.Balances.Where(b => b.Money > 0).ToListAsync();
                        
        }
        public async Task CreateBalance(string userId)
        {
            var balance = new Balance() { UserId = userId, Money = 0 };
            await dataContext.Balances.AddAsync(balance);
            //Kafka.Produce(KafkaTopics.BalanceStream, balance.UserId, new StreamEvent<Balance>(balance, Operation.Created).ToJson());
        }
        public async Task UpdateBalance(BalanceTransaction transaction)
        {
            var balance = await dataContext.Balances.Where(b => b.UserId == transaction.UserId).SingleAsync();
            balance.Money = balance.Money + transaction.Money;
            dataContext.BalanceTransactions.Add(transaction);
            dataContext.Balances.Update(balance);
            await dataContext.SaveChangesAsync();

            //Kafka.Produce(KafkaTopics.BalanceStream, balance.UserId, new StreamEvent<Balance>(balance, Operation.Updated).ToJson());
            //Kafka.Produce(KafkaTopics.BalanceTransactionStream, balance.UserId, new StreamEvent<BalanceTransaction>(transaction, Operation.Created).ToJson());
        }
    }
}
