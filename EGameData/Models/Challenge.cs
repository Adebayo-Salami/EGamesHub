using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class Challenge
    {
        public int Id { get; set; }
        public string ChallengeName { get; set; }
        public GameType GameType { get; set; }
        public BrainGameCategory? BrainGameCategory { get; set; }
        public User GameHost { get; set; }
        public User UserChallenged { get; set; }
        public double AmountToStaked { get; set; }
        public DateTime DateCreated { get; set; }
        public User WinningUser { get; set; }
        public ChallangeStatus ChallengeStatus { get; set; }
        public string ChallengeLink { get; set; }
        public bool IsChallengeStarted { get; set; }
        public bool IsChallengeEnded { get; set; }
        public bool IsUserJoined { get; set; }
        public Bingo BingoGame { get; set; }
        public BrainGameQuestion BrainGameQuestion1 { get; set; }
        public BrainGameQuestion BrainGameQuestion2 { get; set; }
        public BrainGameQuestion BrainGameQuestion3 { get; set; }
        public BrainGameQuestion BrainGameQuestion4 { get; set; }
        public BrainGameQuestion BrainGameQuestion5 { get; set; }
        public WordPuzzle WordPuzzle { get; set; }
        public int GameHostPoints { get; set; }
        public int UserChallengedPoints { get; set; }
        public double AmountToBeWon { get; set; }
        public bool IsGameHostDone { get; set; }
        public bool IsUserChallengedDone { get; set; }
        public DateTime TimeGameHostEnded { get; set; }
        public DateTime TimeUserChallengeEnded { get; set; }
        public string GameSummary { get; set; }
    }
}
