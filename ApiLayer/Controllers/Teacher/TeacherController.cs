using ApiLayer.Authorization;
using BusinessLayer.Teacher;
using BusinessLayer.Teacher.TeacherDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ApiLayer.Controllers.Teacher
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly BusinessLayer.Teacher.Teacher _teacherService;
        private readonly BusinessLayer.Role.Role _roleService;

        public TeacherController(ILogger<TeacherController> logger, BusinessLayer.Teacher.Teacher teacherService, BusinessLayer.Role.Role roleService)
        {
            _logger = logger;
            _teacherService = teacherService;
            _roleService = roleService;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet(Name = "GetAllTeachers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetTeacherDto>> GetAllTeachers()
        {
            var teachers = _teacherService.GetAllTeachers();

            if (teachers == null || !teachers.Any())
                return NotFound("No Teachers Found");

            return Ok(teachers);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("{TeacherId}", Name = "GetTeacherById")]
        [TeacherAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetTeacherDto>> GetTeacherById(int TeacherId)
        {
            if (TeacherId <= 0) return BadRequest(new { message = "Invalid Data" });

            var teacher = _teacherService.GetTeacherById(TeacherId);

            if (teacher == null) return NotFound("Teacher Not Found");

            return Ok(teacher);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("Name/{fullName}", Name = "GetTeacherByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetTeacherDto>> GetTeachersByName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return BadRequest(new { message = "Invalid Data" });

            var teachers = _teacherService.GetTeachersByName(fullName);

            if (teachers.IsNullOrEmpty())
                return NotFound(new { message = "Teacher Not Found" });

            return Ok(teachers);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("Subject/{subjectId}", Name = "GetTeachersBySubjectId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetTeacherDto>> GetTeachersBySubjectId(int subjectId)
        {
            if (subjectId <= 0)
                return BadRequest(new { message = "Invalid Data" });

            var teachers = _teacherService.GetTeachersBySubjectId(subjectId);

            if (teachers.IsNullOrEmpty())
                return NotFound(new { message = "Teachers Not Found for this Subject" });

            return Ok(teachers);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("User/{userId}", Name = "GetTeacherByUserId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetTeacherDto> GetTeacherByUserId(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid Data" });

            var teacher = _teacherService.GetTeacherByUserId(userId);

            if (teacher == null)
                return NotFound(new { message = "Teacher Not Found for this User" });

            return Ok(teacher);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Add", Name = "AddTeacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddTeacher([FromBody] AddTeacherDto addTeacherDto)
        {
            if (addTeacherDto == null ||
                string.IsNullOrWhiteSpace(addTeacherDto.FirstName) ||
                string.IsNullOrWhiteSpace(addTeacherDto.SecondName) ||
                string.IsNullOrWhiteSpace(addTeacherDto.LastName) ||
                string.IsNullOrWhiteSpace(addTeacherDto.Email) ||
                string.IsNullOrWhiteSpace(addTeacherDto.Password) ||
                string.IsNullOrWhiteSpace(addTeacherDto.PhoneNumber) ||
                addTeacherDto.SubjectId <= 0 ||
                addTeacherDto.Salary < 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            if (!_roleService.IsTeacherRole(addTeacherDto.RoleId))
            {
                return BadRequest(new { message = "Role must be teacher" });
            }

            if (!addTeacherDto.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid Data Email must contain @gmail.com" });
            }

            if (!Regex.IsMatch(addTeacherDto.PhoneNumber ?? "", @"^09\d{8}$"))
            {
                return BadRequest(new { message = "Phone number must be 09********" });
            }

            if (_teacherService.IsEmailExists(addTeacherDto.Email))
            {
                return BadRequest(new { message = "Email is already exists" });
            }

            if (_teacherService.IsPhoneNumberExists(addTeacherDto.PhoneNumber))
            {
                return BadRequest(new { message = "Phone Number is already exists" });
            }

            int teacherId = _teacherService.AddNewTeacher(addTeacherDto);

            if (teacherId == -1) return BadRequest(new { message = "Subject not found" });

            if (teacherId > 0)
            {
                return Ok(new
                {
                    message = "Add Teacher Successfully",
                    teacherId = teacherId
                });
            }

            return BadRequest(new { message = "Add Teacher Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("update", Name = "UpdateTeacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateTeacher(UpdateTeacherDto updateTeacherDto)
        {
            if (updateTeacherDto.TeacherId <= 0)
                return BadRequest(new { message = "Invalid ID" });

            if (!string.IsNullOrWhiteSpace(updateTeacherDto.Email) &&
                !updateTeacherDto.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid Email: must contain @gmail.com" });
            }

            if (!string.IsNullOrWhiteSpace(updateTeacherDto.PhoneNumber) &&
                !Regex.IsMatch(updateTeacherDto.PhoneNumber, @"^09\d{8}$"))
            {
                return BadRequest(new { message = "Phone number must be 09********" });
            }

            if (_teacherService.IsEmailExists(updateTeacherDto.Email,updateTeacherDto.UserID))
            {
                return BadRequest(new { message = "Email is already exists" });
            }

            if (_teacherService.IsPhoneNumberExists(updateTeacherDto.PhoneNumber,updateTeacherDto.UserID))
            {
                return BadRequest(new { message = "Phone Number is already exists" });
            }

            int updated = _teacherService.UpdateTeacher(updateTeacherDto);

            if (updated == -1) return BadRequest(new { message = "New subject not found" });

            if (updated == 1)
                return Ok(new { message = "Update teacher Successfully" });

            return NotFound(new { message = "Update teacher Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}", Name = "DeleteTeacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteTeacher(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid Data" });

            bool isDeleted = _teacherService.DeleteTeacher(id);

            if (isDeleted)
                return Ok(new { message = "Delete teacher Successfully" });

            return NotFound(new { message = "Delete teacher Failed" });
        }
    }
}