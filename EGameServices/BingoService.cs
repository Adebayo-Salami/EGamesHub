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

        public bool EndGame(long bingoId, double amount, string selectedColor, out string message)
        {
            throw new NotImplementedException();
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

                Bingo bingoProfile = _context.Bingos.FirstOrDefault(x => x.Id == bingoId);
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
    }
}
