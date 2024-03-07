using Confluent.Kafka;
using PopugAccounting.Logic;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Threading.Tasks;

namespace PopugTaskTracker.Logic
{
    //Вынести в Common
    public class UsersConsumer : KafkaConsumer
    {
        
        public UsersConsumer(AccountingLogic accountingLogic) {
            AccountingLogic = accountingLogic;
        }
        public override string MessageType => KafkaTopics.UsersStream;

        public AccountingLogic AccountingLogic { get; }

        public async override Task OnMessage(Message<Ignore, string> message)
        {
            var popug = SerializeExtensions.FromJson<PopugMessage>(message.Value);
            
            switch ($"{popug.Event}_{popug.Version}")
            {
                case Messages.Users.Stream.Created + "_v1":
                case Messages.Users.Stream.Updated + "_v1":
                    var user = SerializeExtensions.FromJson<User>(popug.Data.ToString());
                    await AccountingLogic.AddOrUpdateUser(SerializeExtensions.FromJson<User>(popug.Data.ToString()));
                    await AccountingLogic.CreateBalance(user.UserId);
                    break;
                case Messages.Users.Stream.Deleted + "_v1":
                    await AccountingLogic.DeleteUser(SerializeExtensions.FromJson<User>(popug.Data.ToString()));
                    break;
                default: throw new NotImplementedException();
            }
        }
    }
}
