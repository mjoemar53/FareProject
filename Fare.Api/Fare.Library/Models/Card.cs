using System;
using System.Collections.Generic;

namespace Fare.Library.Models
{
    public class Card : BaseEntity
    {
        public decimal Load { get; set; }
        public DateTime? LastUsed { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool Discounted { get; set; }
        public string RegisteredId { get; set; }
        public List<Transaction> CompletedTransactions { get; set; }
        public int? LastLine { get; set; }
        public int? LastStation { get; set; }
        public Card()
        {
            Load = 100;
            LastUsed = DateTime.UtcNow;
            ValidUntil = DateTime.UtcNow.AddYears(5);
            Discounted = false;
            RegisteredId = string.Empty;
            CompletedTransactions = new List<Transaction>();
        }
    }

    public class Transaction
    {
        public DateTime TransactionDate { get; set; }
        public int? Line { get; set; }
        public int? Entry { get; set; }
        public int? Exit { get; set; }

        public Transaction()
        {
            TransactionDate = DateTime.UtcNow;
        }
    }
}
