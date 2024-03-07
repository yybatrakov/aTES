using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System.Threading.Tasks;

namespace PopugAnalitics.Logic
{
    public class TasksEventsConsumer : KafkaConsumer
    {

        public TasksEventsConsumer()
        {
        }
        public override string MessageType => KafkaTopics.TasksLifecycle;

        public async override Task OnMessage(Message<Ignore, string> message)
        {
            //var user = message.Value.FromJson<BussinessEvent<Task>>();
        }
    }
}
