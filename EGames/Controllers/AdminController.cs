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
        private readonly IBrainGameQuestionService _brainGameQuestionService;
        private readonly IWordPuzzleService _wordPuzzleService;
        private readonly IChallengeService _challengeService;

        public AdminController(ILogger<HomeController> logger, IUserService userService, IBingoService bingoService, INotificationService notificationService, IBrainGameQuestionService brainGameQuestionService, IWordPuzzleService wordPuzzleService, IChallengeService challengeService)
        {
            _logger = logger;
            _userService = userService;
            _notificationService = notificationService;
            _bingoService = bingoService;
            _brainGameQuestionService = brainGameQuestionService;
            _wordPuzzleService = wordPuzzleService;
            _challengeService = challengeService;
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
                getAllGameHistories = new List<GameHistory>(),
                AgentCode = String.Empty,
                getAllChallenges = _challengeService.GetListOfUserChallenges(loggedUser.Id),
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

            if (!String.IsNullOrWhiteSpace(loggedUser.AgentCode))
            {
                vm.IsAgent = true;
                vm.TotalUsersReferredByAgent = _userService.GetTotalUsersReferred(loggedUser.AgentCode);
                vm.AgentCode = loggedUser.AgentCode;
            }

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
                return RedirectToAction("AdminPanel", "Admin");
            }

            if (data.Amount <= 0 && loggedUser.EmailAddress != "salamibolarinwa16@gmail.com")
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Amount");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Amount");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            User userToFund = _userService.GetUserByEmail(data.EmailAddress, out string msg);
            if (userToFund == null)
            {
                HttpContext.Session.SetString("DisplayMessage", "User To Fund does not exist | " + msg);
                HttpContext.Session.SetString("DashboardErrMsg", "User To Fund does not exist | " + msg);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            bool isFunded = _userService.FundUserAccount(loggedUser, userToFund, data.Amount, out string message);
            if (!isFunded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "User " + data.EmailAddress + " Funded Successfully with " + data.Amount + " Naira.");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "User " + data.EmailAddress + " Funded Successfully with " + data.Amount + " Naira.");
            return RedirectToAction("AdminPanel", "Admin");
        }

        public IActionResult WordPuzzle(bool isPlaying = false, string stakeAmt = null)
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

            AdminWordPuzzleViewModel vm = new AdminWordPuzzleViewModel()
            {
                IsAdmin = loggedUser.isAdmin,
                DisplayMessage = _displayMessage,
                SuccessMessage = _successMessage,
                ErrorMessage = _errorMessage,
                IsPlaying = false
            };

            if (isPlaying)
            {
                if (String.IsNullOrWhiteSpace(stakeAmt))
                {
                    vm.ErrorMessage = "Error, No Amount has been staked.";
                    vm.DisplayMessage = "Error, No Amount has been staked.";
                    return View(vm);
                }

                double amountStaked = 0;
                try
                {
                    amountStaked = Convert.ToDouble(stakeAmt);
                }
                catch
                {
                    vm.ErrorMessage = "Error, Invalid Stake Amount.";
                    vm.DisplayMessage = "Error, Invalid Stake Amount.";
                    return View(vm);
                }

                WordPuzzle wordPuzzleGame = _wordPuzzleService.StartGame(loggedUser.Id, amountStaked, out string message);
                if(wordPuzzleGame == null)
                {
                    vm.ErrorMessage = message;
                    vm.DisplayMessage = message;
                    return View(vm);
                }

                vm.IsPlaying = true;
                vm.QuestionExplanation = wordPuzzleGame.Explanation;
                vm.SuccessMessage = "Word Puzzle Game Started, You have 10 seconds to input the word and win cash prize.";
                HttpContext.Session.SetString("WordPuzzleID", wordPuzzleGame.Id.ToString());
            }

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            return View(vm);
        }

        public IActionResult BrainGame(bool isPlaying = false, string stakeAmt = null, bool movieChk = false, bool footballChk = false, bool musicChk = false, bool marvelMovieChk = false, int percentage = 0)
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
                SuccessMessage = _successMessage,
                ErrorMessage = _errorMessage,
                Timer = 1000
            };

            //Continue from here
            if (isPlaying)
            {
                if(!movieChk && !footballChk && !musicChk && !marvelMovieChk)
                {
                    vm.ErrorMessage = "Error, No Game Category has been selected.";
                    vm.DisplayMessage = "Error, No Game Category has been selected.";
                    return View(vm);
                }

                if (String.IsNullOrWhiteSpace(stakeAmt))
                {
                    vm.ErrorMessage = "Error, No Amount has been staked.";
                    vm.DisplayMessage = "Error, No Amount has been staked.";
                    return View(vm);
                }

                if(percentage < 10)
                {
                    vm.ErrorMessage = "Error, Please select your time range which will determine your percentage interest (The shorter the time, the higher the percentage).";
                    vm.DisplayMessage = "Error, Please select your time range which will determine your percentage interest (The shorter the time, the higher the percentage).";
                    return View(vm);
                }

                double amountStaked = 0;
                try
                {
                    amountStaked = Convert.ToDouble(stakeAmt);
                }
                catch
                {
                    vm.ErrorMessage = "Error, Invalid Stake Amount.";
                    vm.DisplayMessage = "Error, Invalid Stake Amount.";
                    return View(vm);
                }

                BrainGameCategory gameCategory = movieChk ? BrainGameCategory.DCMovies : footballChk ? BrainGameCategory.Football : musicChk ? BrainGameCategory.Music : BrainGameCategory.MarvelMovies;
                List<BrainGameQuestion> brainGameStarted = _brainGameQuestionService.StartGame(loggedUser.Id, amountStaked, gameCategory, out string message);
                if(brainGameStarted.Count <= 0)
                {
                    vm.ErrorMessage = message;
                    vm.DisplayMessage = message;
                    return View(vm);
                }
                vm.Question1 = brainGameStarted[0];
                vm.Question2 = brainGameStarted[1];
                vm.Question3 = brainGameStarted[2];
                vm.Question4 = brainGameStarted[3];
                vm.Question5 = brainGameStarted[4];
                vm.IsPlaying = true;
                switch (percentage)
                {
                    case 10:
                        vm.Timer = 120000;
                        break;
                    case 25:
                        vm.Timer = 90000;
                        break;
                    case 50:
                        vm.Timer = 60000;
                        break;
                    case 100:
                        vm.Timer = 30000;
                        break;
                    default:
                        vm.Timer = 1000;
                        break;
                }

                //Store Brain Game Questions
                HttpContext.Session.SetString("BrainQuestionIDs", vm.Question1.Id + ";" + vm.Question2.Id + ";" + vm.Question3.Id + ";" + vm.Question4.Id + ";" + vm.Question5.Id);
                HttpContext.Session.SetString("TimerPercentage", percentage.ToString());
            }
            //Set up the UI for Brain Game (Stake amount, game category,End Game)

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            return View(vm);
        }

        public IActionResult AdminPanel()
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

            AdminPanelViewModel vm = new AdminPanelViewModel()
            {
                IsAdmin = loggedUser.isAdmin,
                DisplayMessage = _displayMessage,
                SuccessMessage = _successMessage,
                ErrorMessage = _errorMessage,
                AvailableBrainGameQuestions = _brainGameQuestionService.GetAllBrainGameQuestions(),
                TotalUsersRegistered = _userService.TotalUserRegistered(),
                UsersPendingPayout = _userService.GetAllUsersPendingWIthdrawal(),
                AgentList = new Dictionary<User, int>(),
                AvailableWordPuzzleQuestions = _wordPuzzleService.GetWordPuzzles()
            };

            List<User> agents = _userService.GetAgentsDetails();
            if(agents.Count > 0)
            {
                foreach(var agent in agents)
                {
                    int usersReferred = _userService.GetTotalUsersReferred(agent.AgentCode);
                    vm.AgentList.Add(agent, usersReferred);
                }
            }

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
                    //vm.ThirdColor = availableOptions[2];
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
                return RedirectToAction("Index", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Withdrawal made successfully (Please note there is a 50 naira fee for each withdrawal).");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Withdrawal made successfully (Please note there is a 50 naira fee for each withdrawal).");
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public IActionResult AddWordPuzzleQuestion(AdminPanelViewModel data)
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

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Not Authorized");
                HttpContext.Session.SetString("DashboardErrMsg", "Not Authorized");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            if (String.IsNullOrWhiteSpace(data.Question))
            {
                HttpContext.Session.SetString("DisplayMessage", "Explanation Is Required");
                HttpContext.Session.SetString("DashboardErrMsg", "Explanation Is Required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            if (String.IsNullOrWhiteSpace(data.Answer))
            {
                HttpContext.Session.SetString("DisplayMessage", "Answer to question Is Required");
                HttpContext.Session.SetString("DashboardErrMsg", "Answer to question Is Required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            bool isAdded = _wordPuzzleService.AddWordPuzzle(loggedUser.Id, data.Question, data.Answer, out string message);
            if (!isAdded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Word Puzzle Question added successfully");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Word Puzzle Question added successfully");
            return RedirectToAction("AdminPanel", "Admin");
        }

        [HttpPost]
        public IActionResult AddBrainQuestion(AdminPanelViewModel data)
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

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Not Authorized");
                HttpContext.Session.SetString("DashboardErrMsg", "Not Authorized");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            if (String.IsNullOrWhiteSpace(data.Question))
            {
                HttpContext.Session.SetString("DisplayMessage", "Question Is Required");
                HttpContext.Session.SetString("DashboardErrMsg", "Question Is Required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            if (String.IsNullOrWhiteSpace(data.Answer))
            {
                HttpContext.Session.SetString("DisplayMessage", "Answer to question Is Required");
                HttpContext.Session.SetString("DashboardErrMsg", "Answer to question Is Required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            BrainGameQuestion brainGameQuestion = new BrainGameQuestion()
            {
                AddedBy = loggedUser,
                BrainGameCategory = data.BrainGameCategory,
                Answers = data.Answer,
                Question = data.Question
            };
            bool isAdded = _brainGameQuestionService.AddBrainGameQuestion(brainGameQuestion, out string message);
            if (!isAdded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("AdminPanel", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Brain Game Question added successfully");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Brain Game Question added successfully");
            return RedirectToAction("AdminPanel", "Admin");
        }

        [HttpPost]
        public IActionResult CheckUserAccountDetails(AdminDashboardViewModel data)
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

            bool isChecked = _userService.GetUserAccountDetails(data.EmailAddress, out string message);
            if (isChecked)
            {
                HttpContext.Session.SetString("DisplayMessage", "User Balance Check Passed | Results: " + message);
                HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                HttpContext.Session.SetString("DashboardSuccessMsg", "User Balance Check Passed | Results: " + message);
            }
            else
            {
                HttpContext.Session.SetString("DisplayMessage", "User Balance Check failed for " + data.EmailAddress + " with error message: " + message);
                HttpContext.Session.SetString("DashboardErrMsg", "User Balance Check failed for " + data.EmailAddress + " with error message: " + message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }

            return RedirectToAction("AdminPanel", "Admin");
        }

        [HttpPost]
        public IActionResult CreateNewChallenge(AdminDashboardViewModel data)
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

            if (String.IsNullOrWhiteSpace(data.UserChallengedEmailAddress))
            {
                HttpContext.Session.SetString("DisplayMessage", "User Challenged Email Address Is Required.");
                HttpContext.Session.SetString("DashboardErrMsg", "User Challenged Email Address Is Required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            if (String.IsNullOrWhiteSpace(data.ChallengeName))
            {
                HttpContext.Session.SetString("DisplayMessage", "User Challenged Name Is Required.");
                HttpContext.Session.SetString("DashboardErrMsg", "User Challenged Name Is Required");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            if (data.StakeAmount < 500)
            {
                HttpContext.Session.SetString("DisplayMessage", "Stake Amount cannot be less than 500 Naira for a game challenge");
                HttpContext.Session.SetString("DashboardErrMsg", "Stake Amount cannot be less than 500 Naira for a game challenge");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            try
            {
                double testAmt = Convert.ToDouble(data.StakeAmount);
            }
            catch
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Stake Amount");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Stake Amount");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            bool isCreated = _challengeService.CreateChallenge(loggedUser.Id, data.UserChallengedEmailAddress, data.StakeAmount, data.ChallengeName, data.GameType, data.BrainGameCategory, out string message);
            if (!isCreated)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("Index", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Challenge sent successfully!...Hold for user to accept challenge.");
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Challenge Sent Successfully!...Hold for user to accept challenge.");
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult ChallengersRealm(long challengeID = 0)
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");
            string _errorMessage = HttpContext.Session.GetString("DashboardErrMsg");
            string _successMessage = HttpContext.Session.GetString("DashboardSuccessMsg");

            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            ChallengersRealmViewModel vm = new ChallengersRealmViewModel()
            {
                DisplayMessage = _displayMessage,
                ErrorMessage = _errorMessage,
                SuccessMessage = _successMessage,
                IsChallengeValid = true,
                Username = loggedUser.EmailAddress,
                IsGameHost = false,
                IsUserChallenged = false,
                GameType = null
            };

            if (challengeID <= 0)
            {
                vm.ErrorMessage = "Error, Invalid Challenge Data Passed.";
                vm.DisplayMessage = "Error, Invalid Challenge Data Passed.";
                vm.IsChallengeValid = false;
                return View(vm);
            }

            Challenge challenge = _challengeService.PlayChallenge(challengeID, loggedUser.Id, out string message);
            if (challenge == null)
            {
                vm.ErrorMessage = "Error, Challenge Does not exists. | " + message;
                vm.DisplayMessage = "Error, Challenge Does not exists. | " + message;
                vm.IsChallengeValid = false;
                return View(vm);
            }

            vm.GameType = challenge.GameType;

            HttpContext.Session.SetString("ChallengeID", challengeID.ToString());
            if (challenge.GameType == GameType.BrainGame)
            {
                vm.IsGameHost = (challenge.GameHost == loggedUser) ? true : false;
                vm.IsUserChallenged = (challenge.UserChallenged == loggedUser) ? true : false;
                vm.IsGameStarted = challenge.IsChallengeStarted;
                vm.IsGameEnded = challenge.IsChallengeEnded;
                vm.IsChallengedJoined = challenge.IsUserJoined;
                vm.UserWon = (challenge.WinningUser != null) ? challenge.WinningUser.EmailAddress : "Awaiting Results";
                vm.UserLost = (challenge.WinningUser == null) ? "Awaiting Results" : (challenge.GameHost == challenge.WinningUser) ? challenge.UserChallenged.EmailAddress : challenge.GameHost.EmailAddress;
                vm.AmountWon = challenge.AmountToBeWon;
                vm.ChallengedPoints = challenge.UserChallengedPoints;
                vm.HostPoints = challenge.GameHostPoints;
                vm.HostFinishedGame = challenge.TimeGameHostEnded;
                vm.ChallengedFinishedGame = challenge.TimeUserChallengeEnded;
                vm.Question1 = challenge.BrainGameQuestion1;
                vm.Question2 = challenge.BrainGameQuestion2;
                vm.Question3 = challenge.BrainGameQuestion3;
                vm.Question4 = challenge.BrainGameQuestion4;
                vm.Question5 = challenge.BrainGameQuestion5;
            }
            else
            {
                //Still In Implementation Phase
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult MakeAgent(AdminDashboardViewModel data)
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

            bool isNowAgent = _userService.MakeUserAnAgent(data.EmailAddress, out string message);
            if (isNowAgent)
            {
                HttpContext.Session.SetString("DisplayMessage", "Operation Successful: User " + data.EmailAddress + " is now an agent.");
                HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                HttpContext.Session.SetString("DashboardSuccessMsg", "Operation Successful: User " + data.EmailAddress + " is now an agent.");
            }
            else
            {
                HttpContext.Session.SetString("DisplayMessage", "Operation failed for " + data.EmailAddress + " with error message: " + message);
                HttpContext.Session.SetString("DashboardErrMsg", "Operation failed for " + data.EmailAddress + " with error message: " + message);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }

            return RedirectToAction("AdminPanel", "Admin");
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

            return RedirectToAction("AdminPanel", "Admin");
        }

        public IActionResult UserPaid(long userId)
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

            if (userId <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Brain Game Question ID.");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Brain Game Question ID.");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }
            else
            {
                bool isUserPaid = _userService.IsPaid(userId, out string message);
                if (isUserPaid)
                {
                    HttpContext.Session.SetString("DisplayMessage", "User updated to paid.");
                    HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                    HttpContext.Session.SetString("DashboardSuccessMsg", "User updated to paid.");
                }
                else
                {
                    HttpContext.Session.SetString("DisplayMessage", message);
                    HttpContext.Session.SetString("DashboardErrMsg", message);
                    HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                }
            }

            return RedirectToAction("AdminPanel", "Admin");
        }

        public IActionResult RemoveWordPuzzleQuestion(long wordPuzzleQuestionId)
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

            if (wordPuzzleQuestionId <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Word Puzzle Game Question ID.");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Word Puzzle Game Question ID.");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }
            else
            {
                bool isWordPuzzleQuestionRemoved = _wordPuzzleService.RemoveWordPuzzle(wordPuzzleQuestionId, out string message);
                if (isWordPuzzleQuestionRemoved)
                {
                    HttpContext.Session.SetString("DisplayMessage", "Word Puzzle Game Question Removed Successfully.");
                    HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                    HttpContext.Session.SetString("DashboardSuccessMsg", "Word Puzzle Game Question Removed Successfully.");
                }
                else
                {
                    HttpContext.Session.SetString("DisplayMessage", message);
                    HttpContext.Session.SetString("DashboardErrMsg", message);
                    HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                }
            }

            return RedirectToAction("AdminPanel", "Admin");
        }

        public IActionResult RemoveBrainGameQuestion(long brainGameQuestionId)
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

            if (brainGameQuestionId <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Brain Game Question ID.");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Brain Game Question ID.");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }
            else
            {
                bool isBrainGameQuestionRemoved = _brainGameQuestionService.RemoveBrainGameQuestion(brainGameQuestionId, out string message);
                if (isBrainGameQuestionRemoved)
                {
                    HttpContext.Session.SetString("DisplayMessage", "Brain Game Question Removed Successfully.");
                    HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                    HttpContext.Session.SetString("DashboardSuccessMsg", "Brain Game Question Removed Successfully.");
                }
                else
                {
                    HttpContext.Session.SetString("DisplayMessage", message);
                    HttpContext.Session.SetString("DashboardErrMsg", message);
                    HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                }
            }

            return RedirectToAction("AdminPanel", "Admin");
        }

        public IActionResult AcceptDeclineChallenge(long challengeID, bool IsAccepted)
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

            if (challengeID <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid Challenge ID.");
                HttpContext.Session.SetString("DashboardErrMsg", "Invalid Challenge ID.");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
            }
            else
            {
                bool isAcceptedOrDeclined = _challengeService.AcceptOrDeclineChallenge(challengeID, loggedUser.Id, IsAccepted, out string message);
                if (isAcceptedOrDeclined)
                {
                    HttpContext.Session.SetString("DisplayMessage", "Operation Successful: " + message);
                    HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
                    HttpContext.Session.SetString("DashboardSuccessMsg", "Operation Successful: " + message);
                }
                else
                {
                    HttpContext.Session.SetString("DisplayMessage", "Operation Failed: " + message);
                    HttpContext.Session.SetString("DashboardErrMsg", "Operation Failed: " + message);
                    HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                }
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
        public IActionResult EndWordPuzzleGame(string answer = null)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            string wordPuzzle = HttpContext.Session.GetString("WordPuzzleID");

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(wordPuzzle))
            {
                HttpContext.Session.SetString("DisplayMessage", "Error, Unable to retrieve word puzzle question. Please retry!, don't worry you were not debited.");
                HttpContext.Session.SetString("DashboardErrMsg", "Error, Unable to retrieve word puzzle question. Please retry!, don't worry you were not debited.");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("WordPuzzle", "Admin");
            }

            long wordPuzzleId = Convert.ToInt64(wordPuzzle);
            bool isGameEnded = _wordPuzzleService.EndGame(loggedUser.Id, wordPuzzleId, answer, out string message);
            if (!isGameEnded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("WordPuzzleID", String.Empty);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("WordPuzzle", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Word Puzzle Game Ended Successfully |" + message);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("WordPuzzleID", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Word Puzzle Game Ended Successfully |" + message);
            return RedirectToAction("WordPuzzle", "Admin");
        }

        [HttpGet]
        public IActionResult EndChallengeGame(string answer1 = null, string answer2 = null, string answer3 = null, string answer4 = null, string answer5 = null)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            long challengeID = Convert.ToInt64(HttpContext.Session.GetString("ChallengeID"));

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            List<string> answers = new List<string>()
            {
                answer1,
                answer2,
                answer3,
                answer4,
                answer5
            };

            bool IsChallengeEnded = _challengeService.EndChallenge(challengeID, answers, loggedUser.Id, out string message);
            if (!IsChallengeEnded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("BrainQuestionIDs", String.Empty);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("ChallengersRealm", "Admin", new { challengeID = challengeID });
            }

            HttpContext.Session.SetString("DisplayMessage", "Challenge Game Ended Successfully |" + message);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("ChallengeID", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Challenge Game Ended Successfully |" + message);
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public IActionResult EndBrainGame(string answer1 = null, string answer2 = null, string answer3 = null, string answer4 = null, string answer5 = null)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            string brainQuestionsIds = HttpContext.Session.GetString("BrainQuestionIDs");
            int timerPercentage = Convert.ToInt32(HttpContext.Session.GetString("TimerPercentage"));

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Session Expired, Kindly Log In");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(brainQuestionsIds))
            {
                HttpContext.Session.SetString("DisplayMessage", "Error, Unable to retrieve questions set. Please retry!, don't worry you were not debited.");
                HttpContext.Session.SetString("DashboardErrMsg", "Error, Unable to retrieve questions set. Please retry!, don't worry you were not debited.");
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("BrainGame", "Admin");
            }

            bool isGameEnded = _brainGameQuestionService.EndGame(loggedUser.Id, brainQuestionsIds, timerPercentage, answer1, answer2, answer3, answer4, answer5, out string message);
            if (!isGameEnded)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                HttpContext.Session.SetString("DashboardErrMsg", message);
                HttpContext.Session.SetString("BrainQuestionIDs", String.Empty);
                HttpContext.Session.SetString("DashboardSuccessMsg", String.Empty);
                return RedirectToAction("BrainGame", "Admin");
            }

            HttpContext.Session.SetString("DisplayMessage", "Brain Question Game Ended Successfully |" + message);
            HttpContext.Session.SetString("DashboardErrMsg", String.Empty);
            HttpContext.Session.SetString("BrainQuestionIDs", String.Empty);
            HttpContext.Session.SetString("TimerPercentage", String.Empty);
            HttpContext.Session.SetString("DashboardSuccessMsg", "Brain Question Game Ended Successfully |" + message);
            return RedirectToAction("BrainGame", "Admin");
        }

        [HttpGet]
        public IActionResult EndColorBingoGame(string amount, bool firstColorCheckBox, bool secondColorCheckBox)
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

            if(!firstColorCheckBox && !secondColorCheckBox)
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

            int key = firstColorCheckBox ? 0 : 1;
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
