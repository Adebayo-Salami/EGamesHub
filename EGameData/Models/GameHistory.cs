using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class GameHistory
    {
        public int Id { get; set; }
        public GameType GameType { get; set; }
        public DateTime DatePlayed { get; set; }
        public double AmountSpent { get; set; }
        public double AmountWon { get; set; }
        public double SelectedValues { get; set; }
        public double WinningValues { get; set; }
    }
}
