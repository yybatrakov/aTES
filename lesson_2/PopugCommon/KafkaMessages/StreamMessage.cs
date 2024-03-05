using System;

namespace PopugCommon.KafkaMessages
{
    public class StreamMessage<T>
    {
        public string MessageId { get; }
        public DateTime OpertionDate { get; }
        public Operation Operation { get; }
        public T Value { get; }

        public StreamMessage(T value, Operation operation) { 
            MessageId = Guid.NewGuid().ToString(); 
            OpertionDate= DateTime.Now;
            Operation = operation;
            Value = value;
        }
    }
    public enum Operation
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }




}
