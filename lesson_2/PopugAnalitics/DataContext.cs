namespace PopugAnalitics
{
    using Microsoft.EntityFrameworkCore;
    using System;

    public class BalanceTransactionDb
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public int Money { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }

    }

    public class DataContext : DbContext
    {
        public DbSet<BalanceTransactionDb> BalanceTransactionsLog { get; set; } = null;
        
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
