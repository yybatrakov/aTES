using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Threading.Tasks;

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

        public async override Task OnMessage(PopugMessage popug)
        {
            switch ($"{popug.Event}_{popug.Version}")
            {
                case Messages.Users.Stream.Created + "_v1":
                case Messages.Users.Stream.Updated + "_v1":
                    await usersLogic.AddOrUpdate(SerializeExtensions.FromJson<UserStreamEvent>(popug.Data.ToString()));
                    break;
                case Messages.Users.Stream.Deleted + "_v1":
                    await usersLogic.Delete(SerializeExtensions.FromJson<UserStreamEvent>(popug.Data.ToString()));
                    break;
                default: throw new NotImplementedException();
            }
        }
    }
}
