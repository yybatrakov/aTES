namespace PopugTaskTracker
{
    public class TaskStreamEvent
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string AssignedUserId { get; set; }
        public int Fee { get; set; }
        public int Amount { get; set; }

    }
}
