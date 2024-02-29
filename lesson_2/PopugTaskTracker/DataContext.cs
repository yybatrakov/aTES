namespace PopugTaskTracker
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DataContext : DbContext
    {   
        public class Task
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
        public DbSet<Task> Tasks { get; set; } = null;

        public DataContext(DbContextOptions options) : base(options)
        {
             
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
