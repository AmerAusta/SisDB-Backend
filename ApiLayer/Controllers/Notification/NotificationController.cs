using ApiLayer.Authorization;
using BusinessLayer.Notification;
using BusinessLayer.Notification.NotificationDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace ApiLayer.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly BusinessLayer.Notification.Notification _notificationService;

        public NotificationsController(
            ILogger<NotificationsController> logger,
            BusinessLayer.Notification.Notification notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("", Name = "GetAllNotifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetNotificationDto>> GetAllNotifications()
        {
            var notifications = _notificationService.GetAllNotifications();

            if (notifications.IsNullOrEmpty()) return NotFound(new { message = "No Notifications Found" });

            return Ok(notifications);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("AllWithUserDetails", Name = "GetAllNotificationsWithUserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetUserNotificationWithDetialsDto>> GetAllNotificationsWithUserDetails()
        {
            var notifications = _notificationService.GetAllNotificationsWithUserDetails();

            if (notifications.IsNullOrEmpty())
                return NotFound(new { message = "No Notifications Found" });

            return Ok(notifications);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("Class/{classId}", Name = "GetNotificationsByClassId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetUserNotificationWithDetialsDto>> GetNotificationsByClassId(int classId)
        {
            if (classId <= 0) return BadRequest(new { message = "Invalid Class ID" });

            var notifications = _notificationService.GetNotificationsByClassId(classId);

            if (notifications.IsNullOrEmpty())
                return NotFound(new { message = "No Notifications Found for this Class" });

            return Ok(notifications);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{notificationId}", Name = "GetNotificationById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetNotificationDto> GetNotificationById(int notificationId)
        {
            if (notificationId <= 0) return BadRequest(new { message = "Invalid ID" });

            var notification = _notificationService.GetNotificationById(notificationId);

            if (notification == null) return NotFound(new { message = "Notification Not Found" });

            return Ok(notification);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Student,Teacher")]
        [HttpGet("User/{userId}", Name = "GetNotificationsByUserId")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetUserNotificationDto>> GetNotificationsByUserId(int userId)
        {
            if (userId <= 0) return BadRequest(new { message = "Invalid User ID" });

            var notifications = _notificationService.GetNotificationsByUserId(userId);

            if (notifications.IsNullOrEmpty()) return NotFound(new { message = "No Notifications Found for this User" });

            return Ok(notifications);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPost("Add", Name = "AddNewNotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddNewNotification([FromBody] AddNotificationDto addDto)
        {
            if (addDto == null || string.IsNullOrWhiteSpace(addDto.Title) || addDto.UserIds.IsNullOrEmpty())
            {
                return BadRequest(new { message = "Invalid Data or Recipients List is Empty" });
            }

            int notificationId = _notificationService.AddNewNotification(addDto);

            if (notificationId > 0)
            {
                return Ok(new
                {
                    message = "Notification Sent Successfully",
                    notificationId = notificationId
                });
            }

            return BadRequest(new { message = "Failed to Send Notification." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpPut("MarkAsRead", Name = "MarkNotificationAsRead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult MarkNotificationAsRead([FromBody] MarkAsReadDto readDto)
        {
            if (readDto == null || readDto.NotificationId <= 0 || readDto.UserId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            bool isUpdated = _notificationService.MarkNotificationAsRead(readDto);

            if (isUpdated)
                return Ok(new { message = "Notification marked as read successfully" });

            return NotFound(new { message = "Notification recipient record not found." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpDelete("{notificationId}", Name = "DeleteNotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteNotification(int notificationId)
        {
            if (notificationId <= 0) return BadRequest(new { message = "Invalid ID" });

            bool isDeleted = _notificationService.DeleteNotification(notificationId);

            if (isDeleted)
                return Ok(new { message = "Notification Deleted Successfully" });

            return NotFound(new { message = "Notification Delete Failed. Notification ID not found." });
        }
    }
}