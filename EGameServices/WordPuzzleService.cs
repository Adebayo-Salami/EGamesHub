using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using EGamesData.Models;
using EGamesData.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using EGamesData;

namespace EGamesServices
{
    public class WordPuzzleService : IWordPuzzleService
    {
        private readonly IConfiguration _configuration;
        private readonly EGamesContext _context;

        public WordPuzzleService(IConfiguration configuration, EGamesContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool AddWordPuzzle(long userId, string explanation, string answers, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(userId <= 0)
                {
                    message = "Invalid User Id";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(explanation))
                {
                    message = "Word Explanation Is Required.";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(answers))
                {
                    message = "Word Puzzle Answer Is Required.";
                    return result;
                }

                User addedBy = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(addedBy == null)
                {
                    message = "The user is null (does not exist)";
                    return result;
                }

                WordPuzzle wordPuzzle = new WordPuzzle()
                {
                    AddedBy = addedBy,
                    Explanation = explanation,
                    Answers = answers
                };

                _context.WordPuzzles.Add(wordPuzzle);
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

        public bool EndGame(long userId, long wordPuzzleId, string answer, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(wordPuzzleId <= 0)
                {
                    message = "Invalid Word Puzzle ID";
                    return result;
                }

                if(userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                //if (String.IsNullOrWhiteSpace(answer))
                //{
                //    message = "No answer has been supplied";
                //    return result;
                //}

                User userPlaying = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if (userPlaying == null)
                {
                    message = "Error, User playing does not exists.";
                    return result;
                }

                WordPuzzle wordPuzzle = _context.WordPuzzles.FirstOrDefault(x => x.Id == wordPuzzleId);
                if (wordPuzzle == null)
                {
                    message = "Word puzzle game played doesn't exists.";
                    return result;
                }

                List<string> correctAnswers = wordPuzzle.Answers.Split(';').ToList();
                bool isCorrectAnswer = correctAnswers.Any(x => x.ToLower() == answer.ToLower());
                double amountWon = 0;
                if (isCorrectAnswer)
                {
                    double interestProf = 0.2 * userPlaying.AmtUsedToPlayWordPuzzle;
                    amountWon = interestProf + userPlaying.AmtUsedToPlayWordPuzzle;
                }

                GameHistory gameHistory = new GameHistory()
                {
                    User = userPlaying,
                    GameType = GameType.WordPuzzle,
                    DatePlayed = DateTime.Now,
                    AmountSpent = userPlaying.AmtUsedToPlayWordPuzzle,
                    AmountWon = amountWon,
                    SelectedValues = answer,
                    WinningValues = "Not to be disclosed."
                };

                userPlaying.AmtUsedToPlayWordPuzzle = 0;
                userPlaying.WithdrawableAmount = userPlaying.WithdrawableAmount + amountWon;
                userPlaying.TotalGamesPlayed = userPlaying.TotalGamesPlayed + 1;
                _context.Users.Update(userPlaying);
                _context.GameHistories.Add(gameHistory);
                _context.SaveChanges();
                message = (!isCorrectAnswer) ? "Sorry, the correct answer is " + wordPuzzle.Answers.Split(';').FirstOrDefault() + ". You can do it, Play again and win cash prize!" : "Congrats!, Answer is correct. You just won " + amountWon + " Naira";
                result = true;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        public List<WordPuzzle> GetWordPuzzles()
        {
            return _context.WordPuzzles.Include(x => x.AddedBy).ToList();
        }

        public bool RemoveWordPuzzle(long wordPuzzleId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(wordPuzzleId <= 0)
                {
                    message = "Invalid Word Puzzle ID";
                    return result;
                }

                WordPuzzle wordPuzzle = _context.WordPuzzles.FirstOrDefault(x => x.Id == wordPuzzleId);
                if(wordPuzzle == null)
                {
                    message = "Word Puzzle does not exist.";
                    return result;
                }

                _context.WordPuzzles.Remove(wordPuzzle);
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

        public WordPuzzle StartGame(long userId, double stakeAmount, out string message)
        {
            WordPuzzle result = null;
            message = String.Empty;

            try
            {
                if(userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                if(stakeAmount < 500)
                {
                    message = "Apologies, Minimum amount that can be used to play Word Puzzle Game is 500 Naira.";
                    return result;
                }

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "No user with this ID exists";
                    return result;
                }

                if(user.Balance < stakeAmount)
                {
                    message = "Insufficient funds in account to stake " + stakeAmount;
                    return result;
                }

                List<WordPuzzle> wordPuzzles = _context.WordPuzzles.ToList();
                if(wordPuzzles.Count() <= 0)
                {
                    message = "Apologies, Word Puzzle game is still being set-up, Please come back again later.";
                    return result;
                }

                user.Balance = user.Balance - stakeAmount;
                user.AmtUsedToPlayWordPuzzle = stakeAmount;
                _context.Users.Update(user);
                _context.SaveChanges();
                result = wordPuzzles.OrderBy(s => new Random().Next()).Take(1).FirstOrDefault();
            }
            catch(Exception error)
            {
                message = error.Message;
                result = null;
            }

            return result;
        }
    }
}
