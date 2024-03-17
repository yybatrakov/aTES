using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PopugAccounting.Logic
{
    //Вынести в Common
    public class UsersConsumer : KafkaConsumer
    {

        public UsersConsumer(AccountingLogic accountingLogic)
        {
            AccountingLogic = accountingLogic;
        }
        public override string MessageType => KafkaTopics.UsersStream;

        public AccountingLogic AccountingLogic { get; }

        public async override Task OnMessage(PopugMessage popug)
        {
            switch ($"{popug.Event}_{popug.Version}")
            {
                case KafkaMessages.Users.Stream.Created + "_v1":
                case KafkaMessages.Users.Stream.Updated + "_v1":
                    var user = popug.Data.ToString().FromJson<UserStreamEvent>();
                    await AccountingLogic.CreateBalance(user.PublicId);
                    break;
            }
        }
    }
}
