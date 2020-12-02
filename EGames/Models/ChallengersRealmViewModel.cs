using System;
using System.Collections.Generic;
using System.Linq;
using EGamesData.Models;
using System.Threading.Tasks;

namespace EGames.Models
{
    public class ChallengersRealmViewModel
    {
        public long ChallengeId { get; set; }
        public string Username { get; set; }
        public string DisplayMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsGameStarted { get; set; }
        public bool IsChallengedJoined { get; set; }
        public bool IsGameEnded { get; set; }
        public bool IsChallengeValid { get; set; }
        public WordPuzzle WordPuzzleQuestion { get; set; }
        public BrainGameQuestion Question1 { get; set; }
        public BrainGameQuestion Question2 { get; set; }
        public BrainGameQuestion Question3 { get; set; }
        public BrainGameQuestion Question4 { get; set; }
        public BrainGameQuestion Question5 { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string Answer5 { get; set; }
        public string UserWon { get; set; }
        public string UserLost { get; set; }
        public double AmountWon { get; set; }
        public DateTime HostFinishedGame { get; set; }
        public DateTime ChallengedFinishedGame { get; set; }
        public int HostPoints { get; set; }
        public int ChallengedPoints { get; set; }
        public bool IsGameHost { get; set; }
        public bool IsUserChallenged { get; set; }
        public GameType? GameType { get; set; }
    }
}
