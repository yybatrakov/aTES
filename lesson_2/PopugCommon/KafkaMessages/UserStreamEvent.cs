namespace PopugCommon.KafkaMessages
{
    public class UserStreamEvent
    {
        public string UserId { get; set; }
        public string PublicId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}
