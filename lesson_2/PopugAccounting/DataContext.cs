namespace PopugAccounting
{
    using Microsoft.EntityFrameworkCore;
    using PopugCommon.KafkaMessages;
    using PopugTaskTracker;

    public class DataContext : DbContext
    {
        public DbSet<PopugTask> PopugTasks { get; set; } = null;
        public DbSet<User> Users { get; set; } = null;

        public DataContext(DbContextOptions options) : base(options)
        {
             
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
