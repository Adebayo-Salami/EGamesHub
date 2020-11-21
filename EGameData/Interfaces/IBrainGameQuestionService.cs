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
        bool EndGame(long userId, BrainGameQuestion question1, string answer1, BrainGameQuestion question2, string answer2, BrainGameQuestion question3, string answer3, BrainGameQuestion question4, string answer4, BrainGameQuestion question5, string answer5, out string message);
        List<BrainGameQuestion> GetAllBrainGameQuestions();
        bool RemoveBrainGameQuestion(long brainGameQuestionId, out string message);
    }
}
