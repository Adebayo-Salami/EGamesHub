using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class BrainGameQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answers { get; set; }
        public BrainGameCategory BrainGameCategory { get; set; }
        public User AddedBy { get; set; }
    }
}
