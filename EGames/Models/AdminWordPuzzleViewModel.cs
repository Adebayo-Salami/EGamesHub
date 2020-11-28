using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EGames.Models
{
    public class AdminWordPuzzleViewModel
    {
        public bool IsPlaying { get; set; }
        public bool IsAdmin { get; set; }
        public string DisplayMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public double Amount { get; set; }
        public string QuestionExplanation { get; set; }
        public string Answer { get; set; }
    }
}
