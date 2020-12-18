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
    public class PromoService : IPromoService
    {
        private readonly IConfiguration _configuration;
        private readonly EGamesContext _context;

        public PromoService(IConfiguration configuration, EGamesContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public Promo GetPromo(string promoUniqueCode, out string message)
        {
            Promo result = null;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(promoUniqueCode))
                {
                    message = "Promo Unique Code Is Requiredd";
                    return result;
                }

                Promo promo = _context.Promos.Include(x => x.UserWhoEnabledLast).FirstOrDefault(x => x.UniquePromoCode == promoUniqueCode);
                if(promo == null)
                {
                    message = "No Promo With this Code Exists";
                    return result;
                }

                result = promo;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = null;
            }

            return result;
        }

        public bool GetPromoStatus(string promoUniqueCode, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(promoUniqueCode))
                {
                    message = "Unique Promo Code Is Required";
                    return result;
                }

                Promo promo = _context.Promos.Include(x => x.UserWhoEnabledLast).FirstOrDefault(x => x.UniquePromoCode == promoUniqueCode);
                if(promo == null)
                {
                    message = "No Promo with this code exists";
                    return result;
                }

                result = (promo.Status == PromoStatus.Enabled) ? true : false;
            }
            catch(Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        public bool SwitchPromoStatus(string promoUniqueCode, long userId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(promoUniqueCode))
                {
                    message = "Unique Promo Code Is Required";
                    return result;
                }

                if(userId <= 0)
                {
                    message = "Invalid User Id";
                    return result;
                }

                User user = _context.Users.FirstOrDefault(x => x.Id == userId);
                if(user == null)
                {
                    message = "No User with this ID exists";
                    return result;
                }

                Promo promo = _context.Promos.Include(x => x.UserWhoEnabledLast).FirstOrDefault(x => x.UniquePromoCode == promoUniqueCode);
                if (promo == null)
                {
                    message = "No Promo with this code exists";
                    return result;
                }

                promo.Status = (promo.Status == PromoStatus.Disabled) ? PromoStatus.Enabled : PromoStatus.Disabled;
                if(promo.UniquePromoCode == _configuration["PromoUniqueCodes:SpecialSunday"])
                {
                    if(promo.Status == PromoStatus.Disabled)
                    {
                        List<Bingo> allBingoProfiles = _context.Bingos.Where(x => x.PromoTrial > 0).ToList();
                        foreach(var bingoProf in allBingoProfiles)
                        {
                            bingoProf.PromoTrial = 0;
                            _context.Bingos.Update(bingoProf);
                        }
                    }
                    else if(promo.Status == PromoStatus.Enabled)
                    {
                        promo.LastEnabledDate = DateTime.Now;
                        promo.UserWhoEnabledLast = user;
                    }
                }

                _context.Promos.Update(promo);
                _context.SaveChanges();
                message = "Promo " + promo.Status.ToString() + " Successfully.";
                result = true;
            }
            catch (Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }
    }
}
