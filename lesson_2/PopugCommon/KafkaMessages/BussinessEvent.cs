using System;

namespace PopugCommon.KafkaMessages
{
    public class BussinessEvent<T>
    {
        public string EventId { get; }
        public DateTime EventDate { get; }
        public string Event { get; }
        public T Value { get; }

        public BussinessEvent(T value, string e) { 
            EventId = Guid.NewGuid().ToString(); 
            EventDate= DateTime.Now;
            Event = e;
            Value = value;
        }
    }
    public static class Events
    {
        public const string TaskAssigned = "TaskAssigned";
        public const string TaskCompleted = "TaskCompleted";
    }




}
