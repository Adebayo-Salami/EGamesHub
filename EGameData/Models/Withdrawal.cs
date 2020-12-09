using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class Withdrawal
    {
        public int Id { get; set; }
        public User User { get; set; }
        public double Amount { get; set; }
        public string NameOfWithdrawee { get; set; }
        public DateTime DateOfWithdrawal { get; set; }
    }
}
