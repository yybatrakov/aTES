namespace PopugAccounting
{
    using System;

    public class BalanceTransaction
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public int Money { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
