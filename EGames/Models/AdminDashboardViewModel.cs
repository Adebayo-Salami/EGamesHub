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
        public Dictionary<long, string> Notifications { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public bool IsAdmin { get; set; }
        public string EmailAddress { get; set; }
        public double Amount { get; set; }
        public string DisplayMessage { get; set; }
        public bool isWithdrawing { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        public List<EGamesData.Models.Notification> getAllNotifications { get; set; }
        public List<EGamesData.Models.GameHistory> getAllGameHistories { get; set; }
        public List<EGamesData.Models.Challenge> getAllChallenges { get; set; }
        public double GetChallengeWinningPrize(double amount)
        {
            double amtToAdd = amount / 2;
            return amount + amtToAdd;
        }
        public bool IsAgent { get; set; }
        public int TotalUsersReferredByAgent { get; set; }
        public string AgentCode { get; set; }
        public EGamesData.Models.GameType GameType { get; set; }
        public EGamesData.Models.BrainGameCategory BrainGameCategory { get; set; }
        public string UserChallengedEmailAddress { get; set; }
        public double StakeAmount { get; set; }
        public string ChallengeName { get; set; }
        public List<EGamesData.Models.Withdrawal> LatestWithdrawalRecords { get; set; }
    }
}
