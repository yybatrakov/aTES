using System;

namespace PopugCommon.KafkaMessages
{
    public class PopugMessage
    {
        public string EventId { get; }
        public DateTime EventDate { get; }
        public string Event { get; set; }
        public string Version { get; }
        public object Data { get; }
        public PopugMessage(object data, string e, string version)
        {
            EventId = Guid.NewGuid().ToString();
            EventDate = DateTime.Now;
            Event = e;
            Data = data;
            Version = version;
        }   
    }
}
