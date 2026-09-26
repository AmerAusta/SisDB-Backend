using BusinessLayer.Notification.NotificationDto;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Notification
{
    public class Notification
    {
        private readonly SiSDBDbContext _context;

        public Notification(SiSDBDbContext context)
        {
            _context = context;
        }

        public List<GetNotificationDto> GetAllNotifications()
        {
            return _context.Notifications
                .Select(n => new GetNotificationDto
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }

        public List<GetUserNotificationWithDetialsDto> GetAllNotificationsWithUserDetails()
        {
            return _context.NotificationRecipients
                .Join(_context.Notifications,
                    nr => nr.NotificationId,
                    n => n.NotificationId,
                    (nr, n) => new { nr, n })
                .Join(_context.Users,
                    combined => combined.nr.UserId,
                    u => u.UserId,
                    (combined, u) => new GetUserNotificationWithDetialsDto
                    {
                        NotificationId = combined.n.NotificationId,
                        Title = combined.n.Title,
                        Message = combined.n.Message,
                        CreatedAt = combined.n.CreatedAt,
                        IsRead = combined.nr.IsRead,
                        UserId = combined.nr.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    })
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public List<GetUserNotificationWithDetialsDto> GetNotificationsByClassId(int classId)
        {
            if (classId <= 0) return new List<GetUserNotificationWithDetialsDto>();

            return _context.NotificationRecipients
                .Join(_context.Notifications,
                    nr => nr.NotificationId,
                    n => n.NotificationId,
                    (nr, n) => new { nr, n })
                .Join(_context.Users,
                    combined => combined.nr.UserId,
                    u => u.UserId,
                    (combined, u) => new { combined.nr, combined.n, u })
                .Join(_context.Students,
                    x => x.u.UserId,
                    s => s.UserId,
                    (x, s) => new { x.nr, x.n, x.u, s })
                .Where(x => x.s.ClassId == classId) 
                .Select(x => new GetUserNotificationWithDetialsDto
                {
                    NotificationId = x.n.NotificationId,
                    Title = x.n.Title,
                    Message = x.n.Message,
                    CreatedAt = x.n.CreatedAt,
                    IsRead = x.nr.IsRead,
                    UserId = x.nr.UserId,
                    FirstName = x.u.FirstName,
                    LastName = x.u.LastName
                })
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public GetNotificationDto? GetNotificationById(int notificationId)
        {
            if (notificationId <= 0) return null;

            return _context.Notifications
                .Where(n => n.NotificationId == notificationId)
                .Select(n => new GetNotificationDto
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt
                })
                .FirstOrDefault();
        }

        public List<GetUserNotificationDto> GetNotificationsByUserId(int userId)
        {
            if (userId <= 0) return new List<GetUserNotificationDto>();

            return _context.NotificationRecipients
                .Where(nr => nr.UserId == userId)
                .Join(_context.Notifications, nr => nr.NotificationId, n => n.NotificationId, (nr, n) => new GetUserNotificationDto
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = nr.IsRead,
                    UserId = nr.UserId
                })
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public int AddNewNotification(AddNotificationDto addDto)
        {
            if (addDto == null || addDto.UserIds == null || !addDto.UserIds.Any()) return 0;

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var notification = new DataLayer.Models.Entities.Notification
                {
                    Title = addDto.Title,
                    Message = addDto.Message,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
                _context.SaveChanges(); 

                if (notification.NotificationId <= 0)
                {
                    transaction.Rollback();
                    return 0;
                }

                foreach (var userId in addDto.UserIds.Distinct())
                {
                    bool userExists = _context.Users.Any(u => u.UserId == userId);
                    if (userExists)
                    {
                        var recipient = new DataLayer.Models.Entities.NotificationRecipient
                        {
                            NotificationId = notification.NotificationId,
                            UserId = userId,
                            IsRead = false
                        };
                        _context.NotificationRecipients.Add(recipient);
                    }
                }

                bool isSaved = _context.SaveChanges() > 0;

                if (isSaved)
                {
                    transaction.Commit(); 
                    return notification.NotificationId;
                }
                else
                {
                    transaction.Rollback(); 
                    return 0;
                }
            }
            catch (Exception)
            {
                transaction.Rollback(); 
                return 0;
            }
        }

        public bool MarkNotificationAsRead(MarkAsReadDto readDto)
        {
            if (readDto == null || readDto.NotificationId <= 0 || readDto.UserId <= 0) return false;

            var recipient = _context.NotificationRecipients
                .FirstOrDefault(nr => nr.NotificationId == readDto.NotificationId && nr.UserId == readDto.UserId);

            if (recipient == null) return false;

            recipient.IsRead = true;
            _context.NotificationRecipients.Update(recipient);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteNotification(int notificationId)
        {
            if (notificationId <= 0) return false;

            var notification = _context.Notifications.Find(notificationId);
            if (notification == null) return false;

            var recipients = _context.NotificationRecipients.Where(nr => nr.NotificationId == notificationId);
            _context.NotificationRecipients.RemoveRange(recipients);

            _context.Notifications.Remove(notification);
            return _context.SaveChanges() > 0;
        }
    }
}