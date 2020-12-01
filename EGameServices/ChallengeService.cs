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
                _context.Challenges.Update(challenge);
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

        public bool CreateChallenge(long userId, string challengedEmail, double amountToStake, out string message)
        {
            throw new NotImplementedException();
        }

        public bool EndChallenge(long challengeId, out string message)
        {
            throw new NotImplementedException();
        }

        public bool PlayChallenge(long challengeId, long userId, out string message)
        {
            throw new NotImplementedException();
        }
    }
}
