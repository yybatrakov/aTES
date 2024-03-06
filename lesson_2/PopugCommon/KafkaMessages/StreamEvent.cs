using System;

namespace PopugCommon.KafkaMessages
{
    public class StreamEvent<T>
    {
        public string EventId { get; }
        public DateTime EventDate { get; }
        public string Operation { get; }
        public T Value { get; }

        public StreamEvent(T value, string operation) { 
            EventId = Guid.NewGuid().ToString(); 
            EventDate= DateTime.Now;
            Operation = operation;
            Value = value;
        }
    }
    public static class Operation
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }




}
