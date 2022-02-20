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
    public class BingoService : IBingoService
    {
        private readonly IConfiguration _configuration;
        private readonly EGamesContext _context;

        public BingoService(IConfiguration configuration, EGamesContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool EndGame(long userId, double amount, int selectedColorKey, bool useSubscription, out string message)
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

                if(amount < 100)
                {
                    message = "Invalid Amount, Minium amount that can be used to stake is 100 Naira";
                    return false;
                }

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "User does not exist";
                    return false;
                }

                if(user.BingoProfile == null)
                {
                    message = "User does not have a Gaming Profile or user hasn't started game yet.";
                    return false;
                }

                bool allowedFromSubscription = false;
                if (user.BingoProfile.IsSubscribed && useSubscription)
                {
                    if(amount > user.BingoProfile.SubscriptionAmount)
                    {
                        message = "The stake amount must not be greater than the subscription amount when using subscription trial to play.";
                        return false;
                    }

                    allowedFromSubscription = true;
                    user.BingoProfile.SubscriptionTrials = user.BingoProfile.SubscriptionTrials - 1;

                    user.BingoProfile.IsSubscribed = (user.BingoProfile.SubscriptionTrials > 0);
                    user.BingoProfile.SubscriptionAmount = (user.BingoProfile.SubscriptionTrials > 0) ? user.BingoProfile.SubscriptionAmount : 0;
                }

                bool isSundaySpecialPromoEnabled = new PromoService(_configuration, _context).GetPromoStatus(_configuration["PromoUniqueCodes:SpecialSunday"], out string msg);
                if (isSundaySpecialPromoEnabled)
                {
                    if(user.BingoProfile.PromoTrial <= 2 && amount >= 5000)
                    {
                        allowedFromSubscription = true;
                        user.BingoProfile.PromoTrial = user.BingoProfile.PromoTrial + 1;
                    }
                }
                

                if (user.Balance < amount)
                {
                    message = "Insufficient balance in account to stake " + amount;
                    return false;
                }

                List<string> availableOpt = user.BingoProfile.AvailableOptions.Split(";").ToList();
                if(availableOpt.Count < 2)
                {
                    message = "Error, Available options must be two minimal";
                    return false;
                }

                string selectedColor = availableOpt[selectedColorKey];

                user.Balance = user.Balance - amount;
                string winingColor = availableOpt.OrderBy(s => new Random().Next()).First();
                winingColor = allowedFromSubscription ? selectedColor : winingColor;
                bool userWon = (selectedColor.ToLower() == winingColor.ToLower()) ? true : false;
                GameHistory gameHistory = new GameHistory()
                {
                    User = user,
                    GameType = GameType.ColorBingo,
                    DatePlayed = DateTime.Now,
                    AmountSpent = amount,
                    AmountWon = !userWon ? 0 : ((0.3 * amount) + amount),
                    SelectedValues = selectedColor,
                    WinningValues = winingColor
                };

                user.BingoProfile.IsPlaying = false;
                user.BingoProfile.AvailableOptions = String.Empty;
                user.BingoProfile.TotalAmountSpent = user.BingoProfile.TotalAmountSpent + amount;
                user.BingoProfile.TotalAmountWon = !userWon ? user.BingoProfile.TotalAmountWon : ((0.3 * amount) + amount) + user.BingoProfile.TotalAmountWon;
                user.BingoProfile.SelectedColor = String.Empty;
                user.TotalGamesPlayed = user.TotalGamesPlayed + 1;
                user.WithdrawableAmount = !userWon ? user.WithdrawableAmount : ((0.3 * amount) + amount) + user.WithdrawableAmount;
                TransactionHistory transactionHistory = new TransactionHistory()
                {
                    UserFunded = user,
                    FundedBy = user,
                    AmountFunded = amount,
                    DateFunded = DateTime.Now,
                    Narration = "Debiting User account " + user.EmailAddress + " with " + amount + " for Color Bingo Staking.",
                    TransactionType = TransactionType.Debit
                };
                _context.TransactionHistories.Add(transactionHistory);
                _context.GameHistories.Add(gameHistory);
                
                _context.Users.Update(user);
                _context.Bingos.Update(user.BingoProfile);
                _context.SaveChanges();
                message = !userWon ? " Sorry, You lost!" : " Congrats!, you just won " + ((0.3 * amount) + amount);
                result = true;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        public List<GameHistory> GetGameHistories(long userId, GameType gameType)
        {
            return _context.GameHistories.Where(x => x.GameType == gameType && x.User.Id == userId).ToList();
        }

        public bool StartGame(long bingoId, out string message)
        {
            bool result = false;
            message = String.Empty;
            List<string> selectedColors = new List<string>();

            try
            {
                if(bingoId <= 0)
                {
                    message = "Invalid ID";
                    return false;
                }

                var user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.BingoProfile.Id == bingoId);

                if (String.IsNullOrWhiteSpace(user.AgentCode))
                {
                    message = "Error, You mst have referred at least 3 people to play this game. Kindly request for an agent code and send to your referrals.";
                    return false;
                }

                if (_context.Users.Count(x => x.ReferralCode == user.AgentCode) < 3)
                {
                    message = "Error, You mst have referred at least 3 people to play this game";
                    return false;
                }

                //Bingo bingoProfile = _context.Bingos.FirstOrDefault(x => x.Id == bingoId);
                Bingo bingoProfile = user.BingoProfile;
                if(bingoProfile == null)
                {
                    message = "Bingo Profile Does not exist";
                    return false;
                }

                Random rnd = new Random();
                List<string> colorsToPickFrom = new List<string>()
                {
                    "red",
                    "green",
                    "yellow",
                    "white",
                    "pink",
                    "violet",
                    "lightblue",
                    "purple",
                    "orange",
                    "black"
                };

                selectedColors = colorsToPickFrom.OrderBy(x => rnd.Next()).Take(2).ToList();
                bingoProfile.IsPlaying = true;
                bingoProfile.AvailableOptions = string.Join(";", selectedColors);
                _context.Bingos.Update(bingoProfile);
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

        public bool SubscribeToBingoSubscription(long userId, double amount, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                if (amount <= 0)
                {
                    message = "Subscription amount must be greater than zero.";
                    return result;
                }

                List<double> AvailableSubscriptions = new List<double>()
                {
                    2000,
                    5000,
                    7500,
                    10000
                };

                if (!AvailableSubscriptions.Any(x => x == amount))
                {
                    message = "Apologies, The available subscription amounts are [" + string.Join(" Naira,", AvailableSubscriptions) + "]";
                    return result;
                }

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "No User with the specified ID exists.";
                    return result;
                }

                if(user.BingoProfile == null)
                {
                    message = "Error, User does not have a bigingo profile";
                    return result;
                }

                if(user.Balance < amount)
                {
                    message = "Insifficient funds to subscribe to selected Subscription Plan.";
                    return result;
                }

                user.Balance = user.Balance - amount;
                TransactionHistory transactionHistory = new TransactionHistory()
                {
                    FundedBy = user,
                    AmountFunded = amount,
                    DateFunded = DateTime.Now,
                    Narration = "Debit For Bingo Subscription Plan",
                    TransactionType = TransactionType.Debit,
                    UserFunded = user
                };

                user.BingoProfile.IsSubscribed = true;
                user.BingoProfile.SubscriptionTrials = 6;
                user.BingoProfile.SubscriptionAmount = amount;
                _context.Bingos.Update(user.BingoProfile);
                _context.Users.Update(user);
                _context.TransactionHistories.Add(transactionHistory);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }
    }
}
