namespace PopugAnalitics
{
    using Microsoft.EntityFrameworkCore;
    using PopugCommon.KafkaMessages;
    public class TaskDb
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string AssignedUserId { get; set; }
        public int Fee { get; set; }
        public int Amount { get; set; }

    }

    public class DataContext : DbContext
    {
        public DbSet<TaskDb> PopugTasks { get; set; } = null;
        
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
