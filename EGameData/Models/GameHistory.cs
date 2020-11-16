using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class GameHistory
    {
        public int Id { get; set; }
        public User User { get; set; }
        public GameType GameType { get; set; }
        public DateTime DatePlayed { get; set; }
        public double AmountSpent { get; set; }
        public double AmountWon { get; set; }
        public string SelectedValues { get; set; }
        public string WinningValues { get; set; }
    }
}
