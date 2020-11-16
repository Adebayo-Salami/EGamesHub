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
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly EGamesContext _context;

        public UserService(IConfiguration configuration, EGamesContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public User Login(string email, string password, out string message)
        {
            message = String.Empty;
            User loggedUser = null;

            try
            {
                if (String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password))
                {
                    message = "Incomplete Credentials";
                    return null;
                }

                loggedUser = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(user => user.EmailAddress == email && user.Password == password);
                if (loggedUser == null)
                {
                    message = "No User with this credential exists";
                    return null;
                }

                string tokenString = GenerateJSONWebToken(loggedUser);
                loggedUser.AuthenticationToken = tokenString;
                _context.Users.Update(loggedUser);
                _context.SaveChanges();
            }
            catch(Exception error)
            {
                message = error.Message;
                loggedUser = null;
            }

            return loggedUser;
        }

        public bool Register(string email, string password, string bankName, string accountNumber, out string message)
        {
            message = String.Empty;
            bool result = false;

            try
            {
                if (String.IsNullOrWhiteSpace(email))
                {
                    message = "Email Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(password))
                {
                    message = "Password Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(accountNumber))
                {
                    message = "Account Number Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(bankName))
                {
                    message = "Bank Name Is Required";
                    return false;
                }

                User userRegistering = new User()
                {
                    EmailAddress = email,
                    Password = password,
                    DateJoined = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    AccountNumber = accountNumber,
                    BankName = bankName,
                    BingoProfile = new Bingo()
                    {
                        TotalAmountSpent = 0,
                    },
                };

                _context.Users.Add(userRegistering);
                _context.SaveChanges();
                SendEmail(userRegistering.EmailAddress, userRegistering.Password);
                result = true;
            }
            catch(Exception error)
            {
                result = false;
                message = error.Message;
            }

            return result;
        }

        public bool CheckIfEmailExists(string username)
        {
            return _context.Users.Any(x => x.EmailAddress == username);
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress),
                new Claim("Id", userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public void SendEmail(string email, string password)
        {
            try
            {
                System.Net.Mail.SmtpClient server = new System.Net.Mail.SmtpClient("smtp.gmail.com", Convert.ToInt32(_configuration["EmailService:Port"]));
                server.EnableSsl = false;
                server.Credentials = new System.Net.NetworkCredential(_configuration["EmailService:Username"], _configuration["EmailService:Password"]);

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.From = new System.Net.Mail.MailAddress(_configuration["EmailService:Username"], "Podvest Investment Platform");
                message.Subject = "E-Games Hub Login Credentials";
                message.To.Add(new System.Net.Mail.MailAddress(email));
                message.IsBodyHtml = true;
                message.Body = "<html><head></head><body><h1>Dear User,</h1><br /><br /><h3>Congrats!, You just registered on E-Games Hub.</h3><h3>Your Username is: " + email + "</h3><h3>Your Password is:" + password + "</h3></body></html>";

                server.Send(message);
            }
            catch { }
        }

        public User GetUserByEmail(string email, out string message)
        {
            message = String.Empty;
            User user = null;

            try
            {
                if (String.IsNullOrWhiteSpace(email))
                {
                    message = "User Email Is Required";
                    return null;
                }

                user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.EmailAddress == email);
                if (user == null)
                {
                    message = "User with email " + email + " does not exists";
                }
            }
            catch (Exception error)
            {
                user = null;
                message = error.Message;
            }

            return user;
        }

        public User GetUserByID(long userId, out string message)
        {
            message = String.Empty;
            User user = null;

            try
            {
                if (userId <= 0)
                {
                    message = "Invalid User ID";
                    return null;
                }

                user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "User with ID " + userId + " does not exist";
                }
            }
            catch (Exception error)
            {
                user = null;
                message = error.Message;
            }

            return user;
        }

        public bool CheckUserAuthentication(long userId, string authenticationToken, out User loggedUser)
        {
            loggedUser = null;
            bool result = false;

            try
            {
                if (userId <= 0)
                {
                    return false;
                }

                if (String.IsNullOrWhiteSpace(authenticationToken))
                {
                    return false;
                }

                loggedUser = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(user => user.Id == userId && user.AuthenticationToken == authenticationToken);
                if (loggedUser == null)
                {
                    return false;
                }

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool LogoutUser(User user, out string message)
        {
            try
            {
                user.AuthenticationToken = String.Empty;
                _context.Users.Update(user);
                _context.SaveChanges();
                message = String.Empty;
                return true;
            }
            catch (Exception error)
            {
                message = error.Message;
                return false;
            }
        }

        public bool FundUserAccount(User fundedBy, User userFunded, double amount, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(fundedBy == null)
                {
                    message = "User funding the account is null";
                    return false;
                }

                if(userFunded == null)
                {
                    message = "User to be funded is null";
                    return false;
                }

                if(amount <= 0)
                {
                    message = "Amount to fund must be greater than zero";
                    return false;
                }

                userFunded.Balance = userFunded.Balance + amount;
                TransactionHistory transactionHistory = new TransactionHistory()
                {
                    UserFunded = userFunded,
                    FundedBy = fundedBy,
                    AmountFunded = amount,
                    DateFunded = DateTime.Now,
                    Narration = "Funding User " + userFunded.EmailAddress + " with " + amount + " by " + fundedBy.EmailAddress
                };
                _context.TransactionHistories.Add(transactionHistory);
                _context.Users.Update(userFunded);
                _context.SaveChanges();
                result = true;
            }
            catch(Exception err)
            {
                message = err.Message;
                result = false;
            }

            return result;
        }

        public bool Withdraw(long userId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if(userId <= 0)
                {
                    message = "Invalid ID";
                    return false;
                }

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "User does not exist";
                    return false;
                }

                if(user.WithdrawableAmount <= 3000)
                {
                    message = "Minimum amount that can be withdrawn at a time is 3,000 Naira";
                    return false;
                }

                if(user.PendingWithdrawalAmount > 0)
                {
                    message = "User currently has a pending withdrawal";
                    return false;
                }

                user.PendingWithdrawalAmount = user.WithdrawableAmount;
                user.WithdrawableAmount = 0;
                user.IsWithdrawing = true;
                _context.Users.Update(user);
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

        public List<User> GetAllUsersPendingWIthdrawal()
        {
            return _context.Users.Include(x => x.BingoProfile).Where(x => x.IsWithdrawing == true).ToList();
        }

        public bool IsPaid(long userId, out string message)
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

                User user = _context.Users.Include(x => x.BingoProfile).FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "User does not exist";
                    return false;
                }

                if(user.PendingWithdrawalAmount <= 0)
                {
                    message = "Invalid Pending withdrawal amount, Requires investigation.";
                    return false;
                }

                user.TotalAmountWon = user.TotalAmountWon + user.PendingWithdrawalAmount;
                user.PendingWithdrawalAmount = 0;
                user.IsWithdrawing = false;
                _context.Users.Update(user);
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

        public List<GameHistory> GetAllUsersGameHistories(long userId)
        {
            return _context.GameHistories.Include(x => x.User).Where(x => x.User.Id == userId).ToList();
        }
    }
}
