namespace PopugTaskTracker
{
    public class TaskStreamEvent
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string AssignedUserId { get; set; }
        public string JiraId { get; set; }

    }
    public class TaskStreamEvent_2
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string AssignedUserId { get; set; }
        public string JiraId { get; set; }

    }
}
