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

        public bool EndGame(long bingoId, string selectedColors, out string message)
        {
            throw new NotImplementedException();
        }

        public bool StartGame(long bingoId, out string message)
        {
            throw new NotImplementedException();
        }
    }
}
