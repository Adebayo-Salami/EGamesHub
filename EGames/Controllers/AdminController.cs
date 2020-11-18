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
            string _errorMessage = HttpContext.Session.GetString("DashboardErrMsg");
            string _successMessage = HttpContext.Session.GetString("DashboardSuccessMsg");

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
                Notifications = new Dictionary<long, string>(),
                WithdrawableAmount = loggedUser.WithdrawableAmount,
                PendingWithdrawal = loggedUser.PendingWithdrawalAmount,
                Balance = loggedUser.Balance,
                IsAdmin = loggedUser.isAdmin,
                DisplayMessage = _displayMessage,
                isWithdrawing = loggedUser.IsWithdrawing,
                SuccessMessage = _successMessage,
                ErrorMessage = _errorMessage,
                getAllNotifications = new List<Notification>(),
                getAllGameHistories = new List<GameHistory>()
            };

            //Fetch all notifications
            List<Notification> notifications = _notificationService.GetAll();
            int keyNotify = 0;
            foreach (var notify in notifications)
            {
                vm.Notifications.Add(keyNotify, notify.Message);
                keyNotify++;
            }

            if (notifications.Count() > 0)
            {
                vm.getAllNotifications = notifications;
            }

            vm.getAllGameHistories = _userService.GetAllUsersGameHistories(loggedUser.Id);

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
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

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Not Authorized");
                HttpContext.Session.SetString("DashboardErrMsg", "Not Authorized");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            if (String.IsNullOrWhiteSpace(data.EmailAddress))
            {
                HttpContext.Session.SetString("DisplayMessage", "Email Is Required");
                HttpContext.Session.SetString("DashboardErrMsg", "Email Is Required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            if (data.Amount <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Amount");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Amount");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            User userToFund = _userService.GetUserByEmail(data.EmailAddress, out string msg);
            if (userToFund == null)
            {
                HttpContext.Session.SetString("DisplayMessage", "User To Fund does not exist | " + msg);
                HttpContext.Session.SetString("DashboardErrMsg", "User To Fund does not exist | " + msg);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            bool isFunded = _userService.FundUserAccount(loggedUser, userToFund, data.Amount, out string message);
            if (!isFunded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "User " + data.EmailAddress + " Funded Successfully with " + data.Amount + " Naira.");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "User " + data.EmailAddress + " Funded Successfully with " + data.Amount + " Naira.");
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult BrainGame(bool isPlaying = false, double stakeAmt = 0)
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");
            string _errorMessage = HttpContext.Session.GetString("DashboardErrMsg");
            string _successMessage = HttpContext.Session.GetString("DashboardSuccessMsg");

            //Check Authentication
            string userId = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(userId), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            AdminBrainGameViewModel vm = new AdminBrainGameViewModel()
            {
                IsAdmin = loggedUser.isAdmin,
                DisplayMessage = _displayMessage,
                IsPlaying = isPlaying,
                SuccessMessage = _successMessage,
                ErrorMessage = _errorMessage
            };

            //Continue from here
            //Set up the UI for Brain Game (Stake amount, game category,End Game)

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            return View(vm);
        }

        public IActionResult ColorBingo()
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");
            string _errorMessage = HttpContext.Session.GetString("DashboardErrMsg");
            string _successMessage = HttpContext.Session.GetString("DashboardSuccessMsg");

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
                Notifications = new Dictionary<long, string>(),
                WithdrawableAmount = loggedUser.WithdrawableAmount,
                PendingWithdrawal = loggedUser.PendingWithdrawalAmount,
                Balance = loggedUser.Balance,
                IsAdmin = loggedUser.isAdmin,
                DisplayMessage = _displayMessage,
                isWithdrawing = loggedUser.IsWithdrawing,
                IsPlaying = loggedUser.BingoProfile.IsPlaying,
                SuccessMessage = _successMessage,
                ErrorMessage = _errorMessage
            };

            //Fetch all notifications
            List<Notification> notifications = _notificationService.GetAll();
            int keyNotify = 0;
            foreach (var notify in notifications)
            {
                vm.Notifications.Add(keyNotify, notify.Message);
                keyNotify++;
            }

            //Set Color
            if (vm.IsPlaying)
            {
                List<string> availableOptions = loggedUser.BingoProfile.AvailableOptions.Split(";").ToList();
                if(availableOptions.Count > 1)
                {
                    vm.FirstColor = availableOptions[0];
                    vm.SecondColor = availableOptions[1];
                    vm.ThirdColor = availableOptions[2];
                }
            }

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Withdraw()
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

            bool isWithdrawalPlaced = _userService.Withdraw(loggedUser.Id, out string message);
            if (!isWithdrawalPlaced)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("ColorBingo", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Withdrawal made successfully");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Withdrawal made successfully");
            return RedirectToAction("ColorBingo", "Admin");
        }

        [HttpPost]
        public IActionResult AddNotification(AdminDashboardViewModel data)
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

            bool isNotificationAdded = _notificationService.AddNotification(data.Message, out string message);
            if (isNotificationAdded)
            {
                HttpContext.Session.SetString("DisplayMessage", "Notification Added Successfully.");
                HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                HttpContext.Session.SetString("DashboardSuccessMsg", "Notification Added Successfully.");
            }
            else
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult RemoveNotification(long notificationID)
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

            if (notificationID <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Notification ID.");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Notification ID.");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }
            else
            {
                bool isNotificationRemoved = _notificationService.RemoveNotification(notificationID, out string message);
                if (isNotificationRemoved)
                {
                    HttpContext.Session.SetString("DisplayMessage", "Notification Removed Successfully.");
                    HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                    HttpContext.Session.SetString("DashboardSuccessMsg", "Notification Removed Successfully.");
                }
                else
                {
                    HttpContext.Session.SetString("DisplayMessage", message);
                    HttpContext.Session.SetString("DashboardErrMsg", message);
                    HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                }
            }

            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public IActionResult StartColorBingoGame()
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
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("ColorBingo", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Color Bingo Game Started Successfully");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Color Bingo Game Started Successfully");
            return RedirectToAction("ColorBingo", "Admin");
        }

        [HttpGet]
        public IActionResult EndColorBingoGame(string amount, bool firstColorCheckBox, bool secondColorCheckBox, bool thirdColorCheckBox)
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

            if (String.IsNullOrWhiteSpace(amount))
            {
                HttpContext.Session.SetString("DisplayMessage", "Amount is required");
                HttpContext.Session.SetString("DashboardErrMsg", "Amount is required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("ColorBingo", "Admin");
            }

            if(!firstColorCheckBox && !secondColorCheckBox && !thirdColorCheckBox)
            {
                HttpContext.Session.SetString("DisplayMessage", "Select a color");
                HttpContext.Session.SetString("DashboardErrMsg", "Select a color");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("ColorBingo", "Admin");
            }

            double amt;
            try
            {
                amt = Convert.ToDouble(amount);
            }
            catch
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Amount");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Amount");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("ColorBingo", "Admin");
            }

            int key = firstColorCheckBox ? 0 : secondColorCheckBox ? 1 : 2;
            bool isEnded = _bingoService.EndGame(loggedUser.Id, amt, key, out string message);
            if (!isEnded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("ColorBingo", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Color Bingo Game Ended Successfully |" + message);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Color Bingo Game Ended Successfully |" + message);
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
