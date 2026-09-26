using ApiLayer.Authorization;
using ApiLayer.Controllers.User;
using BusinessLayer.Student;
using BusinessLayer.Student.StudentDto;
using BusinessLayer.User;
using BusinessLayer.User.Login;
using DataLayer.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ApiLayer.Controllers.Student
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly BusinessLayer.Student.Student _studentService;
        private readonly BusinessLayer.Role.Role _roleService;

        public StudentController(ILogger<StudentController> logger, BusinessLayer.Student.Student StudentService, BusinessLayer.Role.Role roleService)
        {
            _logger = logger;
            _studentService = StudentService;
            _roleService = roleService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet(Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentDto>> GetAllStudents()
        {
            var students = _studentService.GetAllStudents();

            if (students == null || !students.Any()) return NotFound("No Students Found");

            return Ok(students);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Parent")]
        [HttpGet("parent/{ParentId}", Name = "GetStudentsByParentId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentDto>> GetStudentsByParentId(int ParentId)
        {
            if (ParentId <= 0) return BadRequest(new { message = "Invalid Data" });

            var students = _studentService.GetStudentsByParentId(ParentId);

            if (students == null || !students.Any())
                return NotFound("No Students Found For This Parent");

            return Ok(students);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student,Parent")]
        [StudentAuthorize]
        [HttpGet("{StudentId}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentDto>> GetStudentById(int StudentId)
        {
            if (StudentId <= 0) return BadRequest(new { message = "Invalid Data" });

            var Student = _studentService.GetStudentById(StudentId);

            if (Student == null) return NotFound("Student Not Found");

            return Ok(Student);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student,Parent")]
        [HttpGet("user/{UserId}", Name = "GetStudentByUserId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetStudentDto> GetStudentByUserId(int UserId)
        {
            if (UserId <= 0) return BadRequest(new { message = "Invalid Data" });

            var student = _studentService.GetStudentByUserId(UserId);

            if (student == null) return NotFound("Student Not Found,or Faild");

            return Ok(student);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("Name/{fullName}", Name = "GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentDto>> GetStudentsByName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return BadRequest(new { message = "Invalid Data" });

            var Student = _studentService.GetStudentsByName(fullName);

            if (Student.IsNullOrEmpty())
                return NotFound(new { message = "Student Not Found" });

            return Ok(Student);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("Class/{classId}", Name = "GetStudentsByClassId")]
        public ActionResult<IEnumerable<GetStudentDto>> GetStudentsByClassId(int classId)
        {
            if (classId <= 0) return BadRequest(new { message = "Invalid Class ID" });

            var students = _studentService.GetStudentsByClassId(classId);

            if (students.IsNullOrEmpty())
                return NotFound(new { message = "No Students Found for this Class" });

            return Ok(students);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("Class/{classId}/Ids", Name = "GetStudentIdsByClassId")]
        public ActionResult<IEnumerable<int>> GetStudentIdsByClassId(int classId)
        {
            if (classId <= 0) return BadRequest(new { message = "Invalid Class ID" });

            var studentIds = _studentService.GetStudentIdsByClassId(classId);

            if (studentIds.IsNullOrEmpty())
                return NotFound(new { message = "No Students Found for this Class" });

            return Ok(studentIds);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Add", Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddStudent([FromBody] AddStudentDto addStudentDto)
        {

            if (addStudentDto == null ||
                string.IsNullOrWhiteSpace(addStudentDto.FirstName) ||
                string.IsNullOrWhiteSpace(addStudentDto.SecondName) ||
                string.IsNullOrWhiteSpace(addStudentDto.LastName) ||
                string.IsNullOrWhiteSpace(addStudentDto.Email) ||
                string.IsNullOrWhiteSpace(addStudentDto.Password) ||
                string.IsNullOrWhiteSpace(addStudentDto.PhoneNumber) ||
                addStudentDto.ClassId <= 0 ||
                addStudentDto.ParentId <= 0 ||
                addStudentDto.TotalContractAmount < 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }


            if (!_roleService.IsStudentRole(addStudentDto.RoleId))
            {
                return BadRequest(new { message = "Role must be student" });
            }

            if (!addStudentDto.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid Data Email must contain @gmail.com" });
            }

            if (!Regex.IsMatch(addStudentDto.PhoneNumber ?? "", @"^09\d{8}$"))
            {
                return BadRequest(new { message = "Phone number must be 09********" });
            }


            if (_studentService.IsEmailExists(addStudentDto.Email))
            {
                return BadRequest(new { message = "Email is already exists" });
            }

            if (_studentService.IsPhoneNumberExists(addStudentDto.PhoneNumber))
            {
                return BadRequest(new { message = "Phone Number is already exists" });
            }

            int studentId = _studentService.AddNewStudent(addStudentDto);

            //when return -1 class not found
            if (studentId == -1) return BadRequest(new { message = "class not found" });

            //when return -2 class full
            if (studentId == -2) return BadRequest(new { message = "class full" });

            //when return -3 Parent Found
            if (studentId == -3) return BadRequest(new { message = "Parent not Found" });

            if (studentId > 0)
            {
                return Ok(new
                {
                    message = "Add Student Successfully",
                    studentId = studentId
                });
            }

            return BadRequest(new { message = "Add Student Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("update", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateStudent(UpdateStudentDto updateStudentDto)
        {
            if (updateStudentDto.StudentId <= 0)
                return BadRequest(new { message = "Invalid ID" });

            if (!string.IsNullOrWhiteSpace(updateStudentDto.Email) &&
                !updateStudentDto.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid Email: must contain @gmail.com" });
            }

            if (!string.IsNullOrWhiteSpace(updateStudentDto.PhoneNumber) &&
                !Regex.IsMatch(updateStudentDto.PhoneNumber, @"^09\d{8}$"))
            {
                return BadRequest(new { message = "Phone number must be 09********" });
            }


            if (_studentService.IsEmailExists(updateStudentDto.Email,updateStudentDto.UserID))
            {
                return BadRequest(new { message = "Email is already exists" });
            }

            if (_studentService.IsPhoneNumberExists(updateStudentDto.PhoneNumber,updateStudentDto.UserID))
            {
                return BadRequest(new { message = "Phone Number is already exists" });
            }

            int Updated = _studentService.UpdateStudent(updateStudentDto);

            //when return -1 new class not found
            if (Updated == -1) return BadRequest(new { message = "new class not found" });

            //when return -2 new class full
            if (Updated == -2) return BadRequest(new { message = "new class full" });

            //when return -3 Parent Found
            if (Updated == -3) return BadRequest(new { message = "Parent Found" });

            if (Updated==1)
                return Ok(new { message = "Update student Succefully" });

            return NotFound(new { message = "Update student Faild" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}", Name = "Deletestudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Deletestudent(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid Data" });

            bool isDeleted = _studentService.DeleteStudent(id);

            if (isDeleted)
                return Ok(new { message = "Delete student Succefully" });

            return NotFound(new { message = "Delete student Faild" });
        }
  
    }
}
