using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace PopugTaskTracker
{
    public class BalancePaymentProcessedEvent
    {
        public List<BalancePaymentTransaction> Transactions { get; set; } = new List<BalancePaymentTransaction>();

        
    }
    public class BalancePaymentTransaction
    {
        public string UserId { get; set; }
        public int Money { get; set; }
    }
}
