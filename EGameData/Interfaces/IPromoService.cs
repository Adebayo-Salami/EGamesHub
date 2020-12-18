using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Interfaces
{
    public interface IPromoService
    {
        bool GetPromoStatus(string promoUniqueCode, out string message);
        bool SwitchPromoStatus(string promoUniqueCode, long userId, out string message);
    }
}
