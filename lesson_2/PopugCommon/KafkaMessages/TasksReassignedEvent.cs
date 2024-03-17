using PopugCommon.KafkaMessages;
using System.Collections.Generic;

namespace PopugTaskTracker
{
    public class TasksReassignedEvent
    {
        public List<TasksReassignedEventItem> Tasks= new List<TasksReassignedEventItem>();
    }
    public class TasksReassignedEventItem
    {
        public string PublicId { get; set; }
        public string AssignedUserId { get; set; }
    }

}
