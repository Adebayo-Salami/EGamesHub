using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EGames.Models
{
    public class AdminColorBingoViewModel
    {
        public string Username { get; set; }
        public double Balance { get; set; }
        public int TotalGamesPlayed { get; set; }
        public double TotalAmountWon { get; set; }
        public double WithdrawableAmount { get; set; }
        public double PendingWithdrawal { get; set; }
        public Dictionary<long, string> Notifications { get; set; }
        public string DisplayMessage { get; set; }
        public bool IsAdmin { get; set; }
        public string EmailAddress { get; set; }
        public double Amount { get; set; }
        public bool isWithdrawing { get; set; }
        public ColorBomb Color { get; set; }
        public string FirstColor { get; set; }
        public string Message { get; set; }
        public string SecondColor { get; set; }
        public string ThirdColor { get; set; }
        public bool IsPlaying { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool SubscriptionStatus { get; set; }
        public double SubscriptionAmount { get; set; }
        public int SubscriptionTrialsLeft { get; set; }
        public SubscriptionPlan SelectedSubscriptionPlan { get; set; }
        public EGamesData.Models.PromoStatus SpecialSundayPromoStatus { get; set; }
        public string SpecialSundayPromoDescription { get; set; }
    }

    public enum ColorBomb
    {
        FirstColor,
        SecondColor,
        ThirdColor
    }

    public enum SubscriptionPlan
    {
        TwoThousandNaira_Plan,
        FiveThousandNaira_Plan,
        SevenThousandFiveHunderedNaira_Plan,
        TenThousandNaira_Plan,
    }
}
