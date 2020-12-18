using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public class Promo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastEnabledDate { get; set; }
        public GameType GameType { get; set; }
        public PromoStatus Status { get; set; }
        public string UniquePromoCode { get; set; }
        public string Description { get; set; }
        public User UserWhoEnabledLast { get; set; }
    }
}
