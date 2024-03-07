﻿using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Threading.Tasks;

namespace PopugAccounting.Logic
{
    public class TasksStreamConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.TasksStream;

        public AccountingLogic AccountingLogic { get; }

        public TasksStreamConsumer(AccountingLogic accountingLogic)
        {
            AccountingLogic = accountingLogic;
        }
        
        public async override Task OnMessage(Message<Ignore, string> message)
        {
            var popug = SerializeExtensions.FromJson<PopugMessage>(message.Value);

            switch ($"{popug.Event}_{popug.Version}")
            {
                case Messages.Tasks.Stream.Created + "_v1":
                    var task = SerializeExtensions.FromJson<TaskStreamEvent>(popug.Data.ToString());
                    await AccountingLogic.AddOrUpdateTask(SerializeExtensions.FromJson<TaskStreamEvent>(popug.Data.ToString()));
                    break;
            }

        }
    }
}
