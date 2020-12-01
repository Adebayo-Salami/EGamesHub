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
        public BrainGameCategory BrainGameCategory { get; set; }
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
    }
}
