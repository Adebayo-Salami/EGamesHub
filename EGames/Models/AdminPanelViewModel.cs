using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EGames.Models
{
    public class AdminPanelViewModel
    {
        public bool IsAdmin { get; set; }
        public string DisplayMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string EmailAddress { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public EGamesData.Models.BrainGameCategory BrainGameCategory { get; set; }
        public List<EGamesData.Models.BrainGameQuestion> AvailableBrainGameQuestions { get; set; }
        public int TotalUsersRegistered { get; set; }
        public List<EGamesData.Models.User> UsersPendingPayout { get; set; }
        public Dictionary<EGamesData.Models.User, int> AgentList { get; set; }
    }
}
