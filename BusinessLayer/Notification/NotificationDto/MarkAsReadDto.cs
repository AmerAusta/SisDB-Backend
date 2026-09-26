using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Notification.NotificationDto
{
    public class MarkAsReadDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
    }
}
