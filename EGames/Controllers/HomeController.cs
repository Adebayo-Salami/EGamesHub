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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");

            LoginRegisterViewModel vm = new LoginRegisterViewModel()
            {
                DisplayMessage = _displayMessage
            };

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            return View(vm);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Login(LoginRegisterViewModel data)
        {
            if (String.IsNullOrWhiteSpace(data.Email))
            {
                HttpContext.Session.SetString("DisplayMessage", "Email Is Required");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.Password))
            {
                HttpContext.Session.SetString("DisplayMessage", "Password Is Required");
                return RedirectToAction("Index", "Home");
            }

            User loggedInUser = _userService.Login(data.Email, data.Password, out string message);
            if (loggedInUser == null)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Index", "Home");
            }

            //Setting Authentication Session
            HttpContext.Session.SetString("AuthorizationToken", loggedInUser.AuthenticationToken);
            HttpContext.Session.SetString("UserID", loggedInUser.Id.ToString());

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public IActionResult Register(LoginRegisterViewModel data)
        {
            if (String.IsNullOrWhiteSpace(data.Email))
            {
                HttpContext.Session.SetString("DisplayMessage", "Email Is Required");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.Password))
            {
                HttpContext.Session.SetString("DisplayMessage", "Password Is Required");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.BankName))
            {
                HttpContext.Session.SetString("DisplayMessage", "Bank Name Is Required");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.AccountNumber))
            {
                HttpContext.Session.SetString("DisplayMessage", "Account Number Is Required");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.ReTypePassword))
            {
                HttpContext.Session.SetString("DisplayMessage", "Re-typed Password Is Required");
                return RedirectToAction("Index", "Home");
            }

            if(data.Password != data.ReTypePassword)
            {
                HttpContext.Session.SetString("DisplayMessage", "Passwords do not match.");
                return RedirectToAction("Index", "Home");
            }

            if (!data.AgreeToLicense)
            {
                HttpContext.Session.SetString("DisplayMessage", "You Must Agree To License Terms & Agreements");
                return RedirectToAction("Index", "Home");
            }

            bool isRegistered = _userService.Register(data.Email, data.Password, data.BankName, data.AccountNumber, out string message);
            if (!isRegistered)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("DisplayMessage", "Registration Successful!, Kindly Login Now.");
            return RedirectToAction("Index", "Home");
        }
    }
}
