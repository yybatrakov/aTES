namespace PopugTaskTracker
{
    using Microsoft.EntityFrameworkCore;

    public class TaskDb
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string AssignedUserId { get; set; }
        public string JiraId { get; set; }
        
    }

    public class UserDb
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }

    public class DataContext : DbContext
    {
        public DbSet<TaskDb> PopugTasks { get; set; } = null;
        public DbSet<UserDb> Users { get; set; } = null;

        public DataContext(DbContextOptions options) : base(options)
        {
             
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
