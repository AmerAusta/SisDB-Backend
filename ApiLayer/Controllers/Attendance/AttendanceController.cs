using ApiLayer.Authorization;
using BusinessLayer.Attendance;
using BusinessLayer.Attendance.AttendanceDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace ApiLayer.Controllers.Attendance
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly ILogger<AttendancesController> _logger;
        private readonly BusinessLayer.Attendance.Attendance _attendanceService;

        public AttendancesController(
            ILogger<AttendancesController> logger,
            BusinessLayer.Attendance.Attendance attendanceService)
        {
            _logger = logger;
            _attendanceService = attendanceService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{attendanceId}", Name = "GetAttendanceById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetAttendanceDto> GetAttendanceById(int attendanceId)
        {
            if (attendanceId <= 0) return BadRequest(new { message = "Invalid ID" });

            var attendance = _attendanceService.GetAttendanceById(attendanceId);

            if (attendance == null) return NotFound(new { message = "Attendance Not Found" });

            return Ok(attendance);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Student/{studentId}", Name = "GetAttendancesByStudentId")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetAttendanceDto>> GetAttendancesByStudentId(int studentId)
        {
            if (studentId <= 0) return BadRequest(new { message = "Invalid Student ID" });

            var attendances = _attendanceService.GetAttendancesByStudentId(studentId);

            if (attendances.IsNullOrEmpty()) return NotFound(new { message = "No Attendances Found for this Student" });

            return Ok(attendances);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student,Parent")]
        [HttpGet("Student/{studentId}/Full-Details", Name = "GetAttendancesWithFullDetailsByStudentId")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetAttendanceWithFullDetailsDto>> GetAttendancesWithFullDetailsByStudentId(int studentId)
        {
            if (studentId <= 0) return BadRequest(new { message = "Invalid Student ID" });

            var attendances = _attendanceService.GetAttendancesWithFullDetailsByStudentId(studentId);

            if (attendances.IsNullOrEmpty()) return NotFound(new { message = "No Full Details Attendances Found for this Student" });

            return Ok(attendances);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Lesson/{lessonId}/Full-Details", Name = "GetAttendancesWithFullDetailsByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetAttendanceWithFullDetailsDto>> GetAttendancesWithFullDetailsByLessonId(int lessonId)
        {
            if (lessonId <= 0) return BadRequest(new { message = "Invalid Lesson ID" });

            var attendances = _attendanceService.GetAttendancesWithFullDetailsByLessonId(lessonId);

            if (attendances.IsNullOrEmpty()) return NotFound(new { message = "No Full Details Attendances Found for this Lesson" });

            return Ok(attendances);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("Full-Details", Name = "GetAllAttendancesWithFullDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetAttendanceWithFullDetailsDto>> GetAllAttendancesWithFullDetails()
        {
            var attendances = _attendanceService.GetAllAttendancesWithFullDetails();

            if (attendances.IsNullOrEmpty()) return NotFound(new { message = "No Attendances Found" });

            return Ok(attendances);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpPost("Record", Name = "AddNewAttendance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddNewAttendance([FromBody] AddAttendanceDto addDto)
        {
            if (addDto == null || addDto.StudentId <= 0 || addDto.LessonId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            int attendanceId = _attendanceService.AddNewAttendance(addDto);

            if (attendanceId == -1)
                return BadRequest(new { message = "Student ID not found." });

            if (attendanceId == -2)
                return BadRequest(new { message = "Lesson ID not found." });

            if (attendanceId > 0)
            {
                return Ok(new
                {
                    message = "Attendance Recorded Successfully",
                    attendanceId = attendanceId
                });
            }

            return BadRequest(new { message = "Failed to Record Attendance." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPut("Update", Name = "UpdateAttendance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateAttendance([FromBody] UpdateAttendanceDto updateDto)
        {
            if (updateDto == null || updateDto.AttendanceId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            bool isUpdated = _attendanceService.UpdateAttendance(updateDto);

            if (isUpdated)
                return Ok(new { message = "Attendance Updated Successfully" });

            return BadRequest(new { message = "Attendance Update Failed. Attendance ID, Student ID, or Lesson ID not found." });
        }


        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpDelete("{attendanceId}", Name = "DeleteAttendance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteAttendance(int attendanceId)
        {
            if (attendanceId <= 0) return BadRequest(new { message = "Invalid ID" });

            bool isDeleted = _attendanceService.DeleteAttendance(attendanceId);

            if (isDeleted)
                return Ok(new { message = "Attendance Deleted Successfully" });

            return NotFound(new { message = "Attendance Delete Failed. Attendance ID not found." });
        }
    }
}