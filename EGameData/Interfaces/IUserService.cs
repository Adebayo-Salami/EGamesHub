using EGamesData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EGamesData.Interfaces
{
    public interface IUserService
    {
        bool Register(string email, string password, string bankName, string accountNumber, out string message);
        User Login(string email, string password, out string message);
        User GetUserByEmail(string email, out string message);
        User GetUserByID(long userId, out string message);
        bool CheckUserAuthentication(long userId, string authenticationToken, out User loggedUser);
        bool LogoutUser(User user, out string message);
        bool FundUserAccount(User fundedBy, User userFunded, double amount, out string message);
        bool Withdraw(long userId, out string message);
        bool IsPaid(long userId, out string message);
        List<User> GetAllUsersPendingWIthdrawal();
    }
}
