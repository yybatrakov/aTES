using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

namespace PopugCommon.KafkaMessages
{
    public class BalanceTransactionStreamEvent
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public int Money { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
