using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System.Threading.Tasks;

namespace PopugAnalitics.Logic
{
    public class TaskStreamStreamConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.TasksStream;

        public AnaliticsLogic AnaliticsLogic { get; }

        public TaskStreamStreamConsumer(AnaliticsLogic analiticsLogic)
        {
            AnaliticsLogic = analiticsLogic;
        }

        public async override Task OnMessage(PopugMessage popug)
        {
            switch ($"{popug.Event}_{popug.Version}")
            {
                case KafkaMessages.Tasks.Stream.Created + "_v1":
                    {
                        var e = popug.Data.ToString().FromJson<TaskStreamEvent>();
                        await AnaliticsLogic.AddOrUpdateTask(new TaskDb()
                        {
                            PublicId = e.PublicId,
                            Title = e.Title,
                            IsCompleted = e.IsCompleted,
                            Description = e.Description,
                            AssignedUserId = e.AssignedUserId
                        });
                    }
                    break;
                case KafkaMessages.Tasks.Stream.Created + "_v2":
                    {
                        var e = popug.Data.ToString().FromJson<TaskStreamEvent_2>();

                        await AnaliticsLogic.AddOrUpdateTask(new TaskDb()
                        {
                            PublicId = e.PublicId,
                            Title = e.Title,
                            IsCompleted = e.IsCompleted,
                            Description = e.Description,
                            AssignedUserId = e.AssignedUserId
                        });
                    }
                    break;
            }

        }
    }
}
