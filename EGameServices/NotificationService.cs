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
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly EGamesContext _context;
        public NotificationService(IConfiguration configuration, EGamesContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool AddNotification(string notification, out string message)
        {
            message = String.Empty;
            bool result = false;

            try
            {
                if (String.IsNullOrWhiteSpace(notification))
                {
                    message = "Notification Message Is Required";
                    return false;
                }

                Notification newNotification = new Notification()
                {
                    Message = notification,
                    DatePosted = DateTime.Now
                };

                _context.Notifications.Add(newNotification);
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

        public List<Notification> GetAll()
        {
            return _context.Notifications.OrderByDescending(x => x.DatePosted).ToList();
        }

        public bool RemoveNotification(long notificationID, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (notificationID <= 0)
                {
                    message = "Invalid Notificarion ID";
                    return false;
                }

                Notification notification = _context.Notifications.FirstOrDefault(x => x.Id == notificationID);
                if (notification == null)
                {
                    message = "Notification does not exists";
                    return false;
                }

                _context.Notifications.Remove(notification);
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
