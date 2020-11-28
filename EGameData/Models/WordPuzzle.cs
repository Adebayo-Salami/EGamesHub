using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class WordPuzzle
    {
        public int Id { get; set; }
        public string Explanation { get; set; }
        public string Answers { get; set; }
        public User AddedBy { get; set; }
    }
}
