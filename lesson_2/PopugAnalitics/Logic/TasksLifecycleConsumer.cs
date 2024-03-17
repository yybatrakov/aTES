using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System.Linq;
using System.Threading.Tasks;

namespace PopugAnalitics.Logic
{
    public class TasksLifecycleConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.TasksLifecycle;

        public AnaliticsLogic AnaliticsLogic { get; }

        public TasksLifecycleConsumer(AnaliticsLogic analiticsLogic)
        {
            AnaliticsLogic = analiticsLogic;
        }

        public async override Task OnMessage(PopugMessage popug)
        {
            switch ($"{popug.Event}_{popug.Version}")
            {
                case KafkaMessages.Tasks.ReAssigned + "_v1":
                    {
                        var e = popug.Data.ToString().FromJson<TasksReassignedEvent>();
                        var es = e.Tasks.Select(t =>  new TaskEventsDb() {
                            Event = popug.Event,
                            PublicId = t.PublicId,
                            EventDate = popug.EventDate
                        }).ToList();
                        await AnaliticsLogic.SaveEvent(es);
                    }
                    break;
                case KafkaMessages.Tasks.Added + "_v1":
                    {
                        var e = popug.Data.ToString().FromJson<TaskAddedEvent>();
                        await AnaliticsLogic.SaveEvent(new TaskEventsDb()
                        {
                            Event = popug.Event,
                            PublicId = e.PublicId,
                            EventDate = popug.EventDate
                        });
                    }
                    break;
                case KafkaMessages.Tasks.Completed + "_v1":
                    {
                        var e = popug.Data.ToString().FromJson<TaskCompletedEvent>();
                        await AnaliticsLogic.SaveEvent(new TaskEventsDb()
                        {
                            Event = popug.Event,
                            PublicId = e.PublicId,
                            EventDate = popug.EventDate
                        });
                    }
                    break;
            }

        }
    }
}
