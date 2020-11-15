using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class Bingo
    {
        public int Id { get; set; }
        public double AmountSpentToday { get; set; }
        public double TotalAmountSpent { get; set; }
        public double TotalAmountWon { get; set; }
        public string SelectedColor { get; set; }
        public string AvailableOptions { get; set; }
        public bool IsPlaying { get; set; }
    }
}
