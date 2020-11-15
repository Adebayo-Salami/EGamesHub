﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EGamesData.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool isAdmin { get; set; }
        public string AuthenticationToken { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public int TotalGamesPlayed { get; set; }
        public double TotalAmountWon { get; set; }
        public double Balance { get; set; }
        public double PendingWithdrawalAmount { get; set; }
        public double WithdrawableAmount { get; set; }
        public double IsWithdrawing { get; set; }
        public Bingo BingoProfile { get; set; }
    }
}