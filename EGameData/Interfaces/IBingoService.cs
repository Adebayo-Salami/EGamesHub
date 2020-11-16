using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Interfaces
{
    public interface IBingoService
    {
        bool StartGame(long bingoId, out string message);
        bool EndGame(long bingoId, double amount, string selectedColor, out string message);
    }
}
