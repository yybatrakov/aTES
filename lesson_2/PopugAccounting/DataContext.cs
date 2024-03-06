namespace PopugAccounting
{
    using Microsoft.EntityFrameworkCore;
    using PopugCommon.KafkaMessages;
    using PopugTaskTracker;
    using System;

    public class DataContext : DbContext
    {
        public DbSet<PopugTask> PopugTasks { get; set; } = null;
        public DbSet<User> Users { get; set; } = null;
        public DbSet<Balance> Balances { get; set; } = null;
        public DbSet<BalanceTransaction> BalanceTransactions { get; set; } = null;

        public DataContext(DbContextOptions options) : base(options)
        {
             
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }

    public class Balance
    {
        public string UserId { get; set; }
        public int Money { get; set; }
    }
    public class BalanceTransaction
    {
        public string UserId { get; set; }
        public int Money { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
    public static class TransactionType
    {
        public const string Assign = "Assign";
        public const string Complete = "Complete";
        public const string Payment = "Payment";
    }
}
