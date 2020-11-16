using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EGames.Models;
using Microsoft.AspNetCore.Http;
using EGamesData.Interfaces;
using EGamesData.Models;

namespace EGames.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IBingoService _bingoService;
        private readonly INotificationService _notificationService;

        public AdminController(ILogger<HomeController> logger, IUserService userService, IBingoService bingoService, INotificationService notificationService)
        {
            _logger = logger;
            _userService = userService;
            _notificationService = notificationService;
            _bingoService = bingoService;
        }

        public IActionResult Index()
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");
            //Check Authentication
            string userId = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(userId), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            AdminDashboardViewModel vm = new AdminDashboardViewModel()
            {
                Username = loggedUser.EmailAddress,
                BankName = loggedUser.BankName,
                AccountNumber = loggedUser.AccountNumber,
                TotalGamesPlayed = loggedUser.TotalGamesPlayed,
                TotalAmountWon = loggedUser.TotalAmountWon,
                Notifications = new Dictionary<DateTime, string>(),
                WithdrawableAmount = loggedUser.WithdrawableAmount,
                PendingWithdrawal = loggedUser.PendingWithdrawalAmount,
                Balance = loggedUser.Balance,
                IsAdmin = loggedUser.isAdmin,
                DisplayMessage = _displayMessage,
                isWithdrawing = loggedUser.IsWithdrawing
            };

            //Fetch all notifications
            List<Notification> notifications = _notificationService.GetAll();
            foreach (var notify in notifications)
            {
                vm.Notifications.Add(notify.DatePosted, notify.Message);
            }

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            return View(vm);
        }

        [HttpPost]
        public IActionResult FundWalletManually(AdminDashboardViewModel data)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly Log In Again, unable to retrive user.");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.EmailAddress))
            {
                HttpContext.Session.SetString("DisplayMessage", "Email Is Required");
                return RedirectToAction("Index", "Admin");
            }

            if (data.Amount <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Amount");
                return RedirectToAction("Index", "Admin");
            }

            User userToFund = _userService.GetUserByEmail(data.EmailAddress, out string msg);
            if (userToFund == null)
            {
                HttpContext.Session.SetString("DisplayMessage", "User To Fund does not exist | " + msg);
                return RedirectToAction("Index", "Admin");
            }

            bool isFunded = _userService.FundUserAccount(loggedUser, userToFund, data.Amount, out string message);
            if (!isFunded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Index", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "User " + data.EmailAddress + " Funded Successfully with " + data.Amount + " Naira.");
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult ColorBingo()
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");
            //Check Authentication
            string userId = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(userId), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            AdminColorBingoViewModel vm = new AdminColorBingoViewModel()
            {
                Username = loggedUser.EmailAddress,
                TotalGamesPlayed = loggedUser.TotalGamesPlayed,
                TotalAmountWon = loggedUser.TotalAmountWon,
                Notifications = new Dictionary<DateTime, string>(),
                WithdrawableAmount = loggedUser.WithdrawableAmount,
                PendingWithdrawal = loggedUser.PendingWithdrawalAmount,
                Balance = loggedUser.Balance,
                IsAdmin = loggedUser.isAdmin,
                DisplayMessage = _displayMessage,
                isWithdrawing = loggedUser.IsWithdrawing,
                IsPlaying = loggedUser.BingoProfile.IsPlaying
            };

            //Fetch all notifications
            List<Notification> notifications = _notificationService.GetAll();
            foreach (var notify in notifications)
            {
                vm.Notifications.Add(notify.DatePosted, notify.Message);
            }

            //Set Color
            if (vm.IsPlaying)
            {
                List<string> availableOptions = loggedUser.BingoProfile.AvailableOptions.Split(";").ToList();
                if(availableOptions.Count > 1)
                {
                    vm.FirstColor = availableOptions[0];
                    vm.SecondColor = availableOptions[1];
                }
            }

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            ViewBag.DisplayMessage = _displayMessage;
            return View(vm);
        }

        [HttpGet]
        public IActionResult StartColorBingoGame(bool isUltimate)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            bool isStarted = _bingoService.StartGame(loggedUser.BingoProfile.Id, out string message);
            if (!isStarted)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("ColorBingo", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Color Bingo Game Started Successfully");
            return RedirectToAction("ColorBingo", "Admin");
        }

        public IActionResult Logout()
        {
            try
            {
                //Check Authentication
                string userId = HttpContext.Session.GetString("UserID");
                string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

                bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(userId), authenticationToken, out User loggedUser);
                if (userLogged)
                {
                    if (!_userService.LogoutUser(loggedUser, out string message))
                    {
                        throw new Exception(message);
                    }
                }

            }
            catch (Exception err)
            {
                _logger.LogError("An error occurred at Logout " + err);
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
