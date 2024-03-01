using System;

namespace PopugCommon.KafkaMessages
{
    public class EventMessage<T>
    {
        public string MessageId { get; }
        public DateTime OpertionDate { get; }
        public Events Event { get; }
        public T Value { get; }

        public EventMessage(T value, Events e) { 
            MessageId = Guid.NewGuid().ToString(); 
            OpertionDate= DateTime.Now;
            Event = e;
            Value = value;
        }
    }
    public enum Events
    {
        TaskAssigned = 1,
        TaskCompleted = 2
    }




}
