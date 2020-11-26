using EGamesData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Interfaces
{
    public interface IBrainGameQuestionService
    {
        bool AddBrainGameQuestion(BrainGameQuestion brainGameQuestion, out string message);
        bool UpdateBrainGameQuestion(long brainGameQuestionId, string question, string answers, out string message);
        List<BrainGameQuestion> StartGame(long userId, double stakeAmount, BrainGameCategory category, out string message);
        bool EndGame(long userId, string questionsIDs, double percentage, string answer1, string answer2, string answer3, string answer4, string answer5, out string message);
        List<BrainGameQuestion> GetAllBrainGameQuestions();
        bool RemoveBrainGameQuestion(long brainGameQuestionId, out string message);
    }
}
