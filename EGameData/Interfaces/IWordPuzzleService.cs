using System;
using System.Collections.Generic;
using System.Text;
using EGamesData.Models;

namespace EGamesData.Interfaces
{
    public interface IWordPuzzleService
    {
        bool AddWordPuzzle(long userId, string explanation, string answers, out string message);
        bool RemoveWordPuzzle(long wordPuzzleId, out string message);
        WordPuzzle StartGame(long userId, double stakeAmount, out string message);
        bool EndGame(long userId, long wordPuzzleId, string answer, out string message);
        List<WordPuzzle> GetWordPuzzles();
    }
}
