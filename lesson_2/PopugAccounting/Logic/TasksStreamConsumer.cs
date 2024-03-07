using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
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
            //var user = message.Value.FromJson<StreamEvent<Task>>();
        }
    }
}
