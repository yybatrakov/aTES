using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System.Threading.Tasks;

namespace PopugAccounting.Logic
{
    public class TasksTasksLifecycleConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.TasksLifecycle;

        public AccountingLogic AccountingLogic { get; }

        public TasksTasksLifecycleConsumer(AccountingLogic accountingLogic)
        {
            AccountingLogic = accountingLogic;
        }

        public async override Task OnMessage(Message<Ignore, string> message)
        {
            //var user = message.Value.FromJson<BussinessEvent<Task>>();
        }
    }
}
