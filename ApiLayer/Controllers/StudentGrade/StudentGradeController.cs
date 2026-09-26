using ApiLayer.Authorization;
using BusinessLayer.StudentGrade;
using BusinessLayer.StudentGrade.StudentGradeDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.StudentGrade
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentGradeController : ControllerBase
    {
        private readonly ILogger<StudentGradeController> _logger;
        private readonly BusinessLayer.StudentGrade.StudentGrade _gradeService;

        public StudentGradeController(ILogger<StudentGradeController> logger, BusinessLayer.StudentGrade.StudentGrade gradeService)
        {
            _logger = logger;
            _gradeService = gradeService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("All", Name = "GetAllGrades")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentGradeDto>> GetAllGrades()
        {
            var grades = _gradeService.GetAllGrades();
            if (grades.IsNullOrEmpty()) return NotFound(new { message = "No Grades Found" });

            return Ok(grades);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{gradeId}", Name = "GetGradeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetStudentGradeDto> GetGradeById(int gradeId)
        {
            if (gradeId <= 0) return BadRequest(new { message = "Invalid ID" });

            var grade = _gradeService.GetGradeById(gradeId);
            if (grade == null) return NotFound(new { message = "Grade Not Found" });

            return Ok(grade);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student,Parent")]
        [HttpGet("Student/{studentId}", Name = "GetGradesByStudentId")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentGradeDto>> GetGradesByStudentId(int studentId)
        {
            if (studentId <= 0) return BadRequest(new { message = "Invalid ID" });

            var grades = _gradeService.GetGradesByStudentId(studentId);
            if (grades.IsNullOrEmpty()) return NotFound(new { message = "No Grades Found for this Student" });

            return Ok(grades);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Quiz/{quizId}", Name = "GetGradesByQuizId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentGradeDto>> GetGradesByQuizId(int quizId)
        {
            if (quizId <= 0) return BadRequest(new { message = "Invalid ID" });

            var grades = _gradeService.GetGradesByQuizId(quizId);
            if (grades.IsNullOrEmpty()) return NotFound(new { message = "No Grades Found for this Quiz" });

            return Ok(grades);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Student")]
        [HttpPost("Add", Name = "AddGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddGrade([FromBody] AddStudentGradeDto addGradeDto)
        {
            if (addGradeDto == null || addGradeDto.StudentId <= 0 || addGradeDto.QuizId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            int gradeId = _gradeService.AddNewGrade(addGradeDto);

            if (gradeId == -1)
            {
                return BadRequest(new { message = "Student Not Found" });
            }
            if (gradeId == -2)
            {
                return BadRequest(new { message = "Quiz Not Found" });
            }

            if (gradeId > 0)
            {
                return Ok(new
                {
                    message = "Add Grade Successfully",
                    gradeId = gradeId
                });
            }

            return BadRequest(new { message = "Add Grade Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPut("Update", Name = "UpdateGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateGrade([FromBody] UpdateStudentGradeDto updateGradeDto)
        {
            if (updateGradeDto == null || updateGradeDto.GradeId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            bool isUpdated = _gradeService.UpdateGrade(updateGradeDto);

            if (isUpdated)
                return Ok(new { message = "Update Grade Successfully" });

            return NotFound(new { message = "Update Grade Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student,Parent")]
        [HttpGet("Student/{studentId}/WithQuizDetails", Name = "GetGradesWithQuizDetailsByStudentId")]
        [StudentAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetStudentGradeWithQuizDetailsDto>> GetGradesWithQuizDetailsByStudentId(int studentId)
        {
            if (studentId <= 0) return BadRequest(new { message = "Invalid Student ID" });

            var gradesDetails = _gradeService.GetGradesWithQuizDetailsByStudentId(studentId);

            if (gradesDetails.IsNullOrEmpty())
                return NotFound(new { message = "No Grades or Quiz Details Found for this Student" });

            return Ok(gradesDetails);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpDelete("{gradeId}", Name = "DeleteGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteGrade(int gradeId)
        {
            if (gradeId <= 0) return BadRequest(new { message = "Invalid Data" });

            bool isDeleted = _gradeService.DeleteGrade(gradeId);

            if (isDeleted)
                return Ok(new { message = "Delete Grade Successfully" });

            return NotFound(new { message = "Delete Grade Failed" });
        }
    }
}