using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Models
{
    public enum GameType
    {
        ColorBingo,
        GuessingGame,
        BrainGame,
        WordPuzzle
    }

    public enum BrainGameCategory
    {
        Music,
        Football,
        MarvelMovies,
        DCMovies
    }

    public enum ChallangeStatus
    {
        Accepted,
        Declined,
        Pending
    }

    public enum TransactionType
    {
        Credit,
        Debit
    }

    public enum PromoStatus
    {
        Disabled,
        Enabled
    }
}
