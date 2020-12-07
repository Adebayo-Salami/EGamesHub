using EGamesData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Interfaces
{
    public interface IBingoService
    {
        bool StartGame(long bingoId, out string message);
        bool EndGame(long userId, double amount, int selectedColorKey, bool useSubscription, out string message);
        List<GameHistory> GetGameHistories(long userId, GameType gameType);
        bool SubscribeToBingoSubscription(long userId, double amount, out string message);
    }
}
