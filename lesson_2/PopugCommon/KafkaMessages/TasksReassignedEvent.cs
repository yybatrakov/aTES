using PopugCommon.KafkaMessages;
using System.Collections.Generic;

namespace PopugTaskTracker
{
    public class TasksReassignedEvent
    {
        public List<TaskAssignedEvent> Tasks= new List<TaskAssignedEvent>();
    }
}
