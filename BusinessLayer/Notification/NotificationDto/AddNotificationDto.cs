using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Notification.NotificationDto
{
    public class AddNotificationDto
    {
        public required string Title { get; set; } 
        public required string Message { get; set; } 
        public required List<int> UserIds { get; set; }  // المستخدمين المستلمين للإشعار
    }
}
