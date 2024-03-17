using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System.Threading.Tasks;

namespace PopugAnalitics.Logic
{
    public class TaskCostsStreamConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.TaskCostsStream;

        public AnaliticsLogic AnaliticsLogic { get; }

        public TaskCostsStreamConsumer(AnaliticsLogic analiticsLogic)
        {
            AnaliticsLogic = analiticsLogic;
        }

        public async override Task OnMessage(PopugMessage popug)
        {
            switch ($"{popug.Event}_{popug.Version}")
            {
                case KafkaMessages.TaskCosts.Stream.Created + "_v1":
                    {
                        var e = popug.Data.ToString().FromJson<TaskCostsStreamEvent>();
                        await AnaliticsLogic.AddOrUpdateTaskCosts(new TaskDb()
                        {
                            PublicId = e.PublicId,
                            Fee = e.Fee,
                            Amount= e.Amount
                        });
                    }
                    break;
            }
        }
    }
}
