using EGamesData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EGames.Models
{
    public class AdminBrainGameViewModel
    {
        public bool IsPlaying { get; set; }
        public bool IsAdmin { get; set; }
        public string DisplayMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string EmailAddress { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
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
    }
}
