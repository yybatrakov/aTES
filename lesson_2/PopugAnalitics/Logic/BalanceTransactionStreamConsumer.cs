using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Threading.Tasks;

namespace PopugAnalitics.Logic
{
    public class BalanceTransactionStreamConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.BalanceTransactionStream;

        public AnaliticsLogic AnaliticsLogic { get; }

        public BalanceTransactionStreamConsumer(AnaliticsLogic analiticsLogic)
        {
            AnaliticsLogic = analiticsLogic;
        }

        public async override Task OnMessage(Message<Ignore, string> message)
        {
            var popug = message.Value.FromJson<PopugMessage>();

            switch ($"{popug.Event}_{popug.Version}")
            {
                case Messages.BalanceTransaction.Stream.Created + "_v1":
                    var transactionMessage = popug.Data.ToString().FromJson<BalanceTransactionStreamEvent>();
                    
                    await AnaliticsLogic.Add(new BalanceTransactionDb() { 
                        Date= transactionMessage.Date,
                        Type= transactionMessage.Type,
                        Money= transactionMessage.Money,
                        UserId= transactionMessage.UserId
                    });
                    break;
            }

        }
    }
}
