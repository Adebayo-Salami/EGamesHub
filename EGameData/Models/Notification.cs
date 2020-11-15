using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTime DatePosted { get; set; }
        public string Message { get; set; }
    }
}
