using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System.Threading.Tasks;

namespace PopugTaskTracker.Logic
{
    public class TasksEventsConsumer : KafkaConsumer
    {

        public TasksEventsConsumer() {
        }
        public override string MessageType => KafkaTopics.TasksEvents;

        public async override Task OnMessage(Message<Ignore, string> message)
        {
            var user = SerializeExtensions.FromJson<EventMessage<Task>>(message.Value);
        }
    }
}
