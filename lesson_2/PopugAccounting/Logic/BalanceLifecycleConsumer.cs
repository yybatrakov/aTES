﻿using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Threading.Tasks;

namespace PopugAccounting.Logic
{
    public class BalanceLifecycleConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.TasksLifecycle;

        public AccountingLogic AccountingLogic { get; }

        public BalanceLifecycleConsumer(AccountingLogic accountingLogic)
        {
            AccountingLogic = accountingLogic;
        }

        public async override Task OnMessage(PopugMessage popug)
        {
            switch ($"{popug.Event}_{popug.Version}")
            {
                case KafkaMessages.Balances.PaymentProcessd + "_v1":
                    var paymentEvent = SerializeExtensions.FromJson<BalancePaymentProcessedEvent>(popug.Data.ToString());
                    await AccountingLogic.SendPaymentNotification(paymentEvent);
                    break;
            }
        }
    }
}
