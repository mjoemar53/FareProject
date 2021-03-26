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
        public List<Transaction> Transactions { get; set; }
        public Card()
        {
            Load = 100;
            LastUsed = DateTime.UtcNow;
            ValidUntil = DateTime.UtcNow.AddYears(5);
            Discounted = false;
            RegisteredId = string.Empty;
            Transactions = new List<Transaction>();
        }
    }

    public class Transaction
    {
        public DateTime TransactionDate { get; set; }
    }
}
