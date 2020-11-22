using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using EGamesData;
using EGamesData.Models;
using EGamesData.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

namespace EGamesServices
{
    public class BrainGameQuestionService : IBrainGameQuestionService
    {
        private readonly IConfiguration _configuration;
        private readonly EGamesContext _context;

        public BrainGameQuestionService(IConfiguration configuration, EGamesContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool AddBrainGameQuestion(BrainGameQuestion brainGameQuestion, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(brainGameQuestion.Question))
                {
                    message = "Question Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(brainGameQuestion.Answers))
                {
                    message = "Answers Are Required";
                    return false;
                }

                if(brainGameQuestion.AddedBy == null)
                {
                    message = "Added By Must not be null";
                    return false;
                }

                _context.BrainGameQuestions.Add(brainGameQuestion);
                _context.SaveChanges();
                message = "Brain Game question added successfully.";
                result = true;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        public bool EndGame(long userId, string questionsIDs, string answer1, string answer2, string answer3, string answer4, string answer5, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(userId <= 0)
                {
                    message = "Invalid User ID";
                    return false;
                }

                if(String.IsNullOrWhiteSpace(questionsIDs))
                {
                    message = "Error, Questions set were not provided.";
                    return false;
                }

                List<string> questionIdList = questionsIDs.Split(';').ToList();
                if (questionIdList.Count() < 5)
                {
                    message = "Error, Each round must have a minimum of 5 questions";
                    return false;
                }
                BrainGameQuestion question1 = _context.BrainGameQuestions.Include(x => x.AddedBy).FirstOrDefault(x => x.Id == Convert.ToInt64(questionIdList[0]));
                BrainGameQuestion question2 = _context.BrainGameQuestions.Include(x => x.AddedBy).FirstOrDefault(x => x.Id == Convert.ToInt64(questionIdList[1]));
                BrainGameQuestion question3 = _context.BrainGameQuestions.Include(x => x.AddedBy).FirstOrDefault(x => x.Id == Convert.ToInt64(questionIdList[2]));
                BrainGameQuestion question4 = _context.BrainGameQuestions.Include(x => x.AddedBy).FirstOrDefault(x => x.Id == Convert.ToInt64(questionIdList[3]));
                BrainGameQuestion question5 = _context.BrainGameQuestions.Include(x => x.AddedBy).FirstOrDefault(x => x.Id == Convert.ToInt64(questionIdList[4]));

                if(question1 == null || question2 == null || question3 == null || question4 == null || question5 == null)
                {
                    message = "Error, One or more of the questions passed does not exists.";
                    return false;
                }

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "User does not exist";
                    return false;
                }

                if(user.AmtUsedToPlayBrainGame <= 0)
                {
                    message = "Error, No stake has been made for this Brain Game";
                    return false;
                }

                int correctPoints = 0;
                int answeredQuestions = 0;
                if (!String.IsNullOrWhiteSpace(answer1))
                {
                    List<string> correctAnswers = question1.Answers.Split(';').ToList();
                    foreach(string answer in correctAnswers)
                    {
                        if (answer1.ToLower().Trim().Contains(answer.ToLower().Trim()))
                        {
                            correctPoints = correctPoints + 1;
                            break;
                        }
                    }
                    answeredQuestions = answeredQuestions + 1;
                }

                if (!String.IsNullOrWhiteSpace(answer2))
                {
                    List<string> correctAnswers = question2.Answers.Split(';').ToList();
                    foreach (string answer in correctAnswers)
                    {
                        if (answer2.ToLower().Trim().Contains(answer.ToLower().Trim()))
                        {
                            correctPoints = correctPoints + 1;
                            break;
                        }
                    }
                    answeredQuestions = answeredQuestions + 1;
                }

                if (!String.IsNullOrWhiteSpace(answer3))
                {
                    List<string> correctAnswers = question3.Answers.Split(';').ToList();
                    foreach (string answer in correctAnswers)
                    {
                        if (answer3.ToLower().Trim().Contains(answer.ToLower().Trim()))
                        {
                            correctPoints = correctPoints + 1;
                            break;
                        }
                    }
                    answeredQuestions = answeredQuestions + 1;
                }

                if (!String.IsNullOrWhiteSpace(answer4))
                {
                    List<string> correctAnswers = question4.Answers.Split(';').ToList();
                    foreach (string answer in correctAnswers)
                    {
                        if (answer4.ToLower().Contains(answer.ToLower()))
                        {
                            correctPoints = correctPoints + 1;
                            break;
                        }
                    }
                    answeredQuestions = answeredQuestions + 1;
                }

                if (!String.IsNullOrWhiteSpace(answer5))
                {
                    List<string> correctAnswers = question5.Answers.Split(';').ToList();
                    foreach (string answer in correctAnswers)
                    {
                        if (answer5.ToLower().Trim().Contains(answer.ToLower().Trim()))
                        {
                            correctPoints = correctPoints + 1;
                            break;
                        }
                    }
                    answeredQuestions = answeredQuestions + 1;
                }

                double amountWon = 0;
                if(correctPoints >= 3)
                {
                    double percentWon = (correctPoints == 3) ? 10 : 20;
                    percentWon = percentWon / 100;
                    amountWon = amountWon + (user.AmtUsedToPlayBrainGame * percentWon) + user.AmtUsedToPlayBrainGame;
                }

                GameHistory gameHistory = new GameHistory()
                {
                    User = user,
                    GameType = GameType.BrainGame,
                    DatePlayed = DateTime.Now,
                    AmountSpent = user.AmtUsedToPlayBrainGame,
                    AmountWon = amountWon,
                    SelectedValues = answer1 + ";" + answer2 + ";" + answer3 + ";" + answer4 + ";" + answer5,
                    WinningValues = "Not to be disclosed."
                };

                double halfAmtUsedToStake = user.AmtUsedToPlayBrainGame * 0.5;
                user.AmtUsedToPlayBrainGame = 0;
                user.WithdrawableAmount = user.WithdrawableAmount + amountWon;
                user.TotalGamesPlayed = user.TotalGamesPlayed + 1;
                user.Balance = (amountWon > 0) ? user.Balance : user.Balance + halfAmtUsedToStake;
                _context.Users.Update(user);
                _context.GameHistories.Add(gameHistory);
                _context.SaveChanges();
                message = "Total Questions Answered: " + answeredQuestions + " | Total Questions Gotten Correctly: " + correctPoints + " | Amount Won: " + amountWon;
                result = true;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        public List<BrainGameQuestion> GetAllBrainGameQuestions()
        {
            return _context.BrainGameQuestions.Include(x => x.AddedBy).ToList();
        }

        public bool RemoveBrainGameQuestion(long brainGameQuestionId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (brainGameQuestionId <= 0)
                {
                    message = "Invalid Brain Game Question ID";
                    return false;
                }

                BrainGameQuestion brainGameQuestion = _context.BrainGameQuestions.FirstOrDefault(x => x.Id == brainGameQuestionId);
                if (brainGameQuestion == null)
                {
                    message = "Brain Game Question does not exists";
                    return false;
                }

                _context.BrainGameQuestions.Remove(brainGameQuestion);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception err)
            {
                message = err.Message;
                result = false;
            }

            return result;
        }

        public List<BrainGameQuestion> StartGame(long userId, double stakeAmount, BrainGameCategory category, out string message)
        {
            List<BrainGameQuestion> result = new List<BrainGameQuestion>();
            message = String.Empty;

            try
            {
                if(userId <= 0)
                {
                    message = "Invalid User Id";
                    return new List<BrainGameQuestion>();
                }

                if(stakeAmount < 500)
                {
                    message = "Error, minimum stake amount for the brain game question is 500";
                    return new List<BrainGameQuestion>();
                }

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "Error, No User with this Id exists";
                    return new List<BrainGameQuestion>();
                }

                if(user.Balance < stakeAmount)
                {
                    message = "Insufficient funds in account to stake the amount specified " + stakeAmount;
                    return new List<BrainGameQuestion>();
                }

                List<BrainGameQuestion> availableBrainGameQuestions = _context.BrainGameQuestions.Where(x => x.BrainGameCategory == category).ToList();
                if(availableBrainGameQuestions.Count() < 5)
                {
                    message = "Apologies, The selected category is still being worked on, Kindlys select a different category to play.";
                    return new List<BrainGameQuestion>();
                }

                user.Balance = user.Balance - stakeAmount;
                user.AmtUsedToPlayBrainGame = stakeAmount;
                _context.Users.Update(user);
                _context.SaveChanges();
                result = availableBrainGameQuestions.OrderBy(s => new Random().Next()).Take(5).ToList();
            }
            catch(Exception error)
            {
                message = error.Message;
                result = new List<BrainGameQuestion>();
            }

            return result;
        }

        public bool UpdateBrainGameQuestion(long brainGameQuestionId, string question, string answers, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(brainGameQuestionId <= 0)
                {
                    message = "Invalid Brain Game Question";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(question)){
                    message = "Brain Game Question Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(answers))
                {
                    message = "Brain Game Answers Is Required";
                    return false;
                }

                BrainGameQuestion brainGameQuestion = _context.BrainGameQuestions.FirstOrDefault(x => x.Id == brainGameQuestionId);
                if(brainGameQuestion == null)
                {
                    message = "Error, Brain Game Question does not exist.";
                    return false;
                }

                brainGameQuestion.Question = question;
                brainGameQuestion.Answers = answers;
                _context.BrainGameQuestions.Update(brainGameQuestion);
                _context.SaveChanges();
                result = true;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }
    }
}
