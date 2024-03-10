using Microsoft.EntityFrameworkCore;
using System;

namespace PopugAccounting
{

    public class BalanceTransactionDb
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public int Money { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }

    public class BalanceDb
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Money { get; set; }
    }

    public class TaskDb
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string AssignedUserId { get; set; }
        public int Fee { get; set; }
        public int Amount { get; set; }

    }

    public class DataContext : DbContext
    {
        
        public DbSet<TaskDb> PopugTasks { get; set; } = null;
        public DbSet<BalanceDb> Balances { get; set; } = null;
        public DbSet<BalanceTransactionDb> BalanceTransactions { get; set; } = null;

        public DataContext(DbContextOptions options) : base(options)
        {
             
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        
    }
}
