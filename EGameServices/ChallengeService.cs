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
    public class ChallengeService : IChallengeService
    {
        private readonly IConfiguration _configuration;
        private readonly EGamesContext _context;

        public ChallengeService(IConfiguration configuration, EGamesContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool AcceptOrDeclineChallenge(long challengeId, long userId, bool isAccepted, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(challengeId <= 0)
                {
                    message = "Invalid Challenge ID";
                    return result;
                }

                if(userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                Challenge challenge = _context.Challenges.Include(x => x.UserChallenged).Include(x => x.GameHost).FirstOrDefault(x => x.Id == challengeId);
                if(challenge == null)
                {
                    message = "No Challenge with this ID exists";
                    return result;
                }
                if (challenge.IsChallengeEnded)
                {
                    message = "Challenge has already been ended.";
                    return result;
                }

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "No User with this ID exists";
                    return result;
                }
                if(challenge.UserChallenged != user)
                {
                    message = "Apologies, your not the user the challenge is meant for.";
                    return result;
                }

                challenge.ChallengeStatus = (isAccepted) ? ChallangeStatus.Accepted : ChallangeStatus.Declined;
                if(challenge.ChallengeStatus == ChallangeStatus.Accepted)
                {
                    if (user.Balance < challenge.AmountToStaked)
                    {
                        message = "Apologies, Insufficient funds to stake the amount required " + challenge.AmountToStaked + ". Fund your account and try again";
                        return result;
                    }
                    if(challenge.GameHost.AmtUsedToPlayChallenge < challenge.AmountToStaked)
                    {
                        message = "Error, Game Host Amount available for challenge is insufficient to stake this challenge amount";
                        return result;
                    }

                    TransactionHistory transactionHistory = new TransactionHistory()
                    {
                        UserFunded = user,
                        FundedBy = user,
                        AmountFunded = challenge.AmountToStaked,
                        DateFunded = DateTime.Now,
                        Narration = "Debiting User account " + user.EmailAddress + " with " + challenge.AmountToStaked + " for Challenge " + challenge.ChallengeName + " Staking.",
                        TransactionType = TransactionType.Debit
                    };
                    _context.TransactionHistories.Add(transactionHistory);
                    user.Balance = user.Balance - challenge.AmountToStaked;
                    challenge.GameHost.AmtUsedToPlayChallenge = challenge.GameHost.AmtUsedToPlayChallenge - challenge.AmountToStaked;
                }
                else
                {
                    if(challenge.GameHost.AmtUsedToPlayChallenge >= challenge.AmountToStaked)
                    {
                        TransactionHistory transactionHistory = new TransactionHistory()
                        {
                            UserFunded = user,
                            FundedBy = user,
                            AmountFunded = challenge.AmountToStaked,
                            DateFunded = DateTime.Now,
                            Narration = "Refunding User account " + user.EmailAddress + " with " + challenge.AmountToStaked + " for Challenge " + challenge.ChallengeName + " Staking.",
                            TransactionType = TransactionType.Credit
                        };
                        _context.TransactionHistories.Add(transactionHistory);
                        challenge.GameHost.Balance = challenge.GameHost.Balance + challenge.AmountToStaked;
                        challenge.GameHost.AmtUsedToPlayChallenge = challenge.GameHost.AmtUsedToPlayChallenge - challenge.AmountToStaked;
                    }
                }
                _context.Challenges.Update(challenge);
                _context.Users.Update(user);
                _context.Update(challenge.GameHost);
                _context.SaveChanges();
                message = "Challenge " + challenge.ChallengeStatus.ToString() + " Successfully";
                result = true;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        public bool CreateChallenge(long userId, string challengedEmail, double amountToStake, string challengeName, GameType gameType, BrainGameCategory brainGameCategory, out string message)
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

                if (String.IsNullOrWhiteSpace(challengedEmail))
                {
                    message = "Email of user challenged is required";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(challengeName))
                {
                    message = "Challenge Name is required";
                    return result;
                }

                if(amountToStake < 500)
                {
                    message = "Minimum amount that can be staked is 500 Naira";
                    return result;
                }

                User user = _context.Users.FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "No User with the provided ID exists";
                    return result;
                }

                User challengedUser = _context.Users.FirstOrDefault(x => x.EmailAddress == challengedEmail);
                if(challengedUser == null)
                {
                    message = "No user with email " + challengedEmail + " exists on the platform";
                    return result;
                }

                if(user == challengedUser)
                {
                    message = "Error, Both users can't be the same account.";
                    return result;
                }

                if(user.Balance < amountToStake)
                {
                    message = "Insufficient funds in User Account to stake " + amountToStake+ ". Fund your account and try again.";
                    return result;
                }

                if(gameType != GameType.BrainGame)
                {
                    message = "Apologies, Other Game Types challenges are still in development, Please try the brain Game Challnge";
                    return result;
                }

                Challenge challenge = new Challenge()
                {
                    ChallengeName = challengeName,
                    GameType = gameType,
                    BrainGameCategory = (gameType == GameType.BrainGame) ? (BrainGameCategory?)brainGameCategory : null,
                    GameHost = user,
                    UserChallenged = challengedUser,
                    AmountToStaked = amountToStake,
                    DateCreated = DateTime.UtcNow,
                    ChallengeStatus = ChallangeStatus.Pending,
                };

                TransactionHistory transactionHistory = new TransactionHistory()
                {
                    UserFunded = user,
                    FundedBy = user,
                    AmountFunded = amountToStake,
                    DateFunded = DateTime.Now,
                    Narration = "Debiting User account " + user.EmailAddress + " with " + amountToStake + " for Challenge Game " + challenge.ChallengeName + " Staking.",
                    TransactionType = TransactionType.Debit
                };

                user.AmtUsedToPlayChallenge = user.AmtUsedToPlayChallenge + amountToStake;
                user.Balance = user.Balance - amountToStake;
                _context.TransactionHistories.Add(transactionHistory);
                _context.Challenges.Add(challenge);
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

        public bool EndChallenge(long challengeId, List<string> answers, long userId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(challengeId <= 0)
                {
                    message = "Invalid Challenge ID";
                    return result;
                }

                if(userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                Challenge challenge = _context.Challenges.Include(x => x.WinningUser)
                    .Include(x => x.BrainGameQuestion1)
                    .Include(x => x.BrainGameQuestion2)
                    .Include(x => x.BrainGameQuestion3)
                    .Include(x => x.BrainGameQuestion4)
                    .Include(x => x.BrainGameQuestion5)
                    .Include(x => x.GameHost)
                    .Include(x => x.UserChallenged)
                    .FirstOrDefault(x => x.Id == challengeId);

                if(challenge == null)
                {
                    message = "No Challenge with this ID exists";
                    return result;
                }

                if(challenge.GameHost.Id != userId && challenge.UserChallenged.Id != userId)
                {
                    message = "This Challenge does not belong to you";
                    return result;
                }
                bool isGameHost = (challenge.GameHost.Id == userId) ? true : false;

                if(challenge.GameType == GameType.BrainGame)
                {
                    string answer1 = String.Empty;
                    string answer2 = String.Empty;
                    string answer3 = String.Empty;
                    string answer4 = String.Empty;
                    string answer5 = String.Empty;

                    try
                    {
                        answer1 = answers[0];
                    } catch { }
                    try
                    {
                        answer2 = answers[1];
                    }
                    catch { }
                    try
                    {
                        answer3 = answers[2];
                    }
                    catch { }
                    try
                    {
                        answer4 = answers[3];
                    }
                    catch { }
                    try
                    {
                        answer5 = answers[4];
                    }
                    catch { }

                    int correctPoints = 0;
                    int answeredQuestions = 0;
                    bool HostWon = false;
                    bool IsDraw = false;
                    if (!String.IsNullOrWhiteSpace(answer1))
                    {
                        List<string> correctAnswers = challenge.BrainGameQuestion1.Answers.Split(';').ToList();
                        foreach (string answer in correctAnswers)
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
                        List<string> correctAnswers = challenge.BrainGameQuestion2.Answers.Split(';').ToList();
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
                        List<string> correctAnswers = challenge.BrainGameQuestion3.Answers.Split(';').ToList();
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
                        List<string> correctAnswers = challenge.BrainGameQuestion4.Answers.Split(';').ToList();
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
                        List<string> correctAnswers = challenge.BrainGameQuestion5.Answers.Split(';').ToList();
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

                    if(challenge.AmountToBeWon <= 0)
                    {
                        challenge.AmountToBeWon = (challenge.AmountToStaked / 2) + challenge.AmountToStaked;
                    }

                    if (isGameHost)
                    {
                        challenge.IsChallengeEnded = true;
                        challenge.IsGameHostDone = true;
                        challenge.GameHostPoints = correctPoints;
                        challenge.TimeGameHostEnded = DateTime.Now;
                    }
                    else
                    {
                        challenge.IsUserChallengedDone = true;
                        challenge.UserChallengedPoints = correctPoints;
                        challenge.TimeUserChallengeEnded = DateTime.Now;
                    }

                    if(challenge.IsGameHostDone && challenge.IsUserChallengedDone)
                    {
                        if(challenge.GameHostPoints > challenge.UserChallengedPoints)
                        {
                            challenge.WinningUser = challenge.GameHost;
                            challenge.GameHost.WithdrawableAmount = challenge.GameHost.WithdrawableAmount + challenge.AmountToBeWon;
                            challenge.GameSummary = "Game Host Won!";
                            HostWon = true;
                        }
                        else if(challenge.UserChallengedPoints > challenge.GameHostPoints)
                        {
                            challenge.WinningUser = challenge.UserChallenged;
                            challenge.UserChallenged.WithdrawableAmount = challenge.UserChallenged.WithdrawableAmount + challenge.AmountToBeWon;
                            challenge.GameSummary = "User Challenged Won!";
                            HostWon = true;
                        }
                        else if((challenge.UserChallengedPoints == challenge.GameHostPoints) && challenge.GameHostPoints > 2 && challenge.TimeGameHostEnded != challenge.TimeUserChallengeEnded)
                        {
                            if(challenge.TimeGameHostEnded < challenge.TimeUserChallengeEnded)
                            {
                                challenge.WinningUser = challenge.GameHost;
                                challenge.GameHost.WithdrawableAmount = challenge.GameHost.WithdrawableAmount + challenge.AmountToBeWon;
                                challenge.GameSummary = "Game Host Won Due to time frame!";
                                HostWon = true;
                            }
                            else
                            {
                                challenge.WinningUser = challenge.UserChallenged;
                                challenge.UserChallenged.WithdrawableAmount = challenge.UserChallenged.WithdrawableAmount + challenge.AmountToBeWon;
                                challenge.GameSummary = "User Challenged Won Due to time frame!";
                                HostWon = true;
                            }
                        }
                        else
                        {
                            IsDraw = true;
                            challenge.GameSummary = "Game Ended In A Draw! (Both participants get a 30% refund of the amount staked)";
                            challenge.GameHost.Balance = challenge.GameHost.Balance + (0.3 * challenge.AmountToStaked);
                            challenge.UserChallenged.Balance = challenge.UserChallenged.Balance + (0.3 * challenge.AmountToStaked);
                            TransactionHistory transactionHistory1 = new TransactionHistory()
                            {
                                UserFunded = challenge.GameHost,
                                FundedBy = challenge.GameHost,
                                AmountFunded = 0.3 * challenge.AmountToStaked,
                                DateFunded = DateTime.Now,
                                Narration = "Refunding User account " + challenge.GameHost.EmailAddress + " with " + challenge.AmountToStaked + " (30% of amount staked) for Challenge " + challenge.ChallengeName + " Staking.",
                                TransactionType = TransactionType.Credit
                            };
                            TransactionHistory transactionHistory2 = new TransactionHistory()
                            {
                                UserFunded = challenge.UserChallenged,
                                FundedBy = challenge.UserChallenged,
                                AmountFunded = 0.3 * challenge.AmountToStaked,
                                DateFunded = DateTime.Now,
                                Narration = "Refunding User account " + challenge.UserChallenged.EmailAddress + " with " + challenge.AmountToStaked + " (30% of amount staked) for Challenge " + challenge.ChallengeName + " Staking.",
                                TransactionType = TransactionType.Credit
                            };
                            _context.TransactionHistories.Add(transactionHistory1);
                            _context.TransactionHistories.Add(transactionHistory2);
                        }

                        GameHistory gameHistory1 = new GameHistory()
                        {
                            User = challenge.GameHost,
                            GameType = GameType.BrainGame,
                            DatePlayed = DateTime.Now,
                            AmountSpent = challenge.AmountToStaked,
                            AmountWon = HostWon ? (0.3 * challenge.AmountToBeWon) : 0,
                            SelectedValues = String.Empty,
                            WinningValues = String.Empty
                        };
                        GameHistory gameHistory2 = new GameHistory()
                        {
                            User = challenge.UserChallenged,
                            GameType = GameType.ColorBingo,
                            DatePlayed = DateTime.Now,
                            AmountSpent = challenge.AmountToStaked,
                            AmountWon = HostWon ? 0 : (IsDraw) ? 0 : challenge.AmountToBeWon,
                            SelectedValues = String.Empty,
                            WinningValues = String.Empty
                        };
                        _context.GameHistories.Add(gameHistory1);
                        _context.GameHistories.Add(gameHistory2);
                    }

                    _context.Users.Update(challenge.GameHost);
                    _context.Users.Update(challenge.UserChallenged);
                    _context.Challenges.Update(challenge);
                    _context.SaveChanges();
                    result = true;
                }
                else
                {
                    message = "Error, Game type not configured yet.";
                    return result;
                }
            }
            catch (Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        public Challenge GetChallengeByID(long challengeId, out string message)
        {
            Challenge result = null;
            message = String.Empty;

            try
            {
                if(challengeId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                Challenge challenge = _context.Challenges
                    .Include(x => x.GameHost)
                    .Include(x => x.UserChallenged)
                    .Include(x => x.WinningUser)
                    .Include(x => x.BrainGameQuestion1)
                    .Include(x => x.BrainGameQuestion2)
                    .Include(x => x.BrainGameQuestion3)
                    .Include(x => x.BrainGameQuestion4)
                    .Include(x => x.BrainGameQuestion5)
                    .FirstOrDefault(x => x.Id == challengeId);

                if(challenge == null)
                {
                    message = "Error, No Challenge with this ID exists";
                    return result;
                }

                result = challenge;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = null;
            }

            return result;
        }

        public List<Challenge> GetListOfUserChallenges(long userId)
        {
            List<Challenge> result = new List<Challenge>();

            if(userId <= 0)
            {
                return result;
            }

            User user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if(user == null)
            {
                return result;
            }
            result = _context.Challenges.Include(x => x.UserChallenged).Include(x => x.WinningUser).Include(x => x.GameHost).Where(x => x.GameHost == user || x.UserChallenged == user).OrderByDescending(x => x.Id).ToList();

            return result;
        }

        public Challenge PlayChallenge(long challengeId, long userId, out string message)
        {
            Challenge result = null;
            message = String.Empty;

            try
            {
                if(challengeId <= 0)
                {
                    message = "Challene ID Is Invalid";
                    return result;
                }

                Challenge challenge = _context.Challenges.Include(x => x.GameHost).Include(x => x.UserChallenged).FirstOrDefault(x => x.Id == challengeId);
                if(challenge == null)
                {
                    message = "No Challenge with this ID exists";
                    return result;
                }

                if(challenge.GameHost.Id != userId && challenge.UserChallenged.Id != userId)
                {
                    message = "Error, Challende does not belong to you.";
                    return result;
                }
                if(challenge.GameType == GameType.BrainGame)
                {
                    if(challenge.BrainGameCategory == null)
                    {
                        message = "Error, No Category was configured for the selected Brain Game Challenge";
                        return result;
                    }

                    if(challenge.GameHost.Id == userId)
                    {
                        //Check if user left during game play
                        if (challenge.IsChallengeStarted)
                        {
                            List<string> answers = new List<string>();
                            EndChallenge(challengeId, answers, userId, out string mmg);
                            result = null;
                            message = "User Left while game is ongoing and has automatically forfeited the game. || " + mmg;
                        }
                        else
                        {
                            List<BrainGameQuestion> avalilableBrainGameQuestions = _context.BrainGameQuestions.Where(x => x.BrainGameCategory == challenge.BrainGameCategory).ToList();
                            if (avalilableBrainGameQuestions.Count() < 5)
                            {
                                message = "Apologies, Brain Game Questions are still being configured. Try again later";
                                return result;
                            }

                            List<BrainGameQuestion> selectedBrainGameQuestions = avalilableBrainGameQuestions.OrderBy(s => new Random().Next()).Take(5).ToList();
                            challenge.BrainGameQuestion1 = selectedBrainGameQuestions[0];
                            challenge.BrainGameQuestion2 = selectedBrainGameQuestions[1];
                            challenge.BrainGameQuestion3 = selectedBrainGameQuestions[2];
                            challenge.BrainGameQuestion4 = selectedBrainGameQuestions[3];
                            challenge.BrainGameQuestion5 = selectedBrainGameQuestions[4];
                            challenge.IsChallengeStarted = true;
                            _context.Challenges.Update(challenge);
                            _context.SaveChanges();
                            result = challenge;
                        }

                    }
                    else
                    {
                        result = GetChallengeByID(challengeId, out string dontNeed);
                        if(result == null)
                        {
                            message = dontNeed;
                        }
                        else
                        {
                            if (result.IsChallengeStarted && result.IsUserJoined)
                            {
                                List<string> answers = new List<string>();
                                EndChallenge(challengeId, answers, userId, out string mmg);
                                result = null;
                                message = "User Left while game is ongoing and has automatically forfeited the game. || " + mmg;
                            }
                            else
                            {
                                result.IsUserJoined = true;
                                _context.Challenges.Update(result);
                                _context.SaveChanges();
                            }
                        }
                    }

                }
                else
                {
                    message = "Apologies, Other Game Types challenges are still in development, Please try the brain Game Challnge";
                    return result;
                }
            }
            catch (Exception error)
            {
                message = error.Message;
                result = null;
            }

            return result;
        }
    }
}
