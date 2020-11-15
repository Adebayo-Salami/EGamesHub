using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class TransactionHistory
    {
        public int Id { get; set; }
        public User FundedBy { get; set; }
        public User UserFunded { get; set; }
        public double AmountFunded { get; set; }
        public DateTime DateFunded { get; set; }
    }
}
