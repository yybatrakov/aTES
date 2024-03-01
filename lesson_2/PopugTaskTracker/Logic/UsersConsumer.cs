using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System.Threading.Tasks;
using static PopugTaskTracker.DataContext;

namespace PopugTaskTracker.Logic
{
    //Вынести в Common
    public class UsersConsumer : KafkaConsumer
    {
        private readonly UsersLogic usersLogic;

        public UsersConsumer(UsersLogic usersLogic) {
            this.usersLogic = usersLogic;
        }
        public override string MessageType => KafkaTopics.UsersStream;

        public async override Task OnMessage(Message<Ignore, string> message)
        {
            var user = SerializeExtensions.FromJson<StreamMessage<User>>(message.Value);
            switch (user.Operation)
            {
                case Operation.Add:
                case Operation.Update:
                    await usersLogic.AddOrUpdate(user.Value);
                    break;
                case Operation.Delete:
                    await usersLogic.Delete(user.Value);
                    break;
            }
        }
    }
}
