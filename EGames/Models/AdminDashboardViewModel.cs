using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EGames.Models
{
    public class AdminDashboardViewModel
    {
        public string Username { get; set; }
        public double Balance { get; set; }
        public int TotalGamesPlayed { get; set; }
        public double TotalAmountWon { get; set; }
        public double WithdrawableAmount { get; set; }
        public double PendingWithdrawal { get; set; }
        public Dictionary<DateTime, string> Notifications { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public bool IsAdmin { get; set; }
        public string EmailAddress { get; set; }
        public double Amount { get; set; }
        public string DisplayMessage { get; set; }
    }
}
