using ApiLayer.Authorization;
using ApiLayer.Controllers.Quiz;
using BusinessLayer.Quiz;
using BusinessLayer.Quiz.QuizDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly ILogger<QuizController> _logger;
        private readonly BusinessLayer.Quiz.Quiz _quizService;

        public QuizController(ILogger<QuizController> logger, BusinessLayer.Quiz.Quiz quizService)
        {
            _logger = logger;
            _quizService = quizService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("lesson/{lessonId}", Name = "GetQuizzesByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetQuizDto>> GetQuizzesByLessonId(int lessonId)
        {
            if (lessonId <= 0) return BadRequest(new { message = "Invalid ID" });

            var quizzes = _quizService.GetQuizzesByLessonId(lessonId);
            if (quizzes.IsNullOrEmpty()) return NotFound(new { message = "No Quizzes Found" });

            return Ok(quizzes);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{quizId}", Name = "GetQuizById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetQuizDto> GetQuizById(int quizId)
        {
            if (quizId <= 0) return BadRequest(new { message = "Invalid ID" });

            var quiz = _quizService.GetQuizById(quizId);
            if (quiz == null) return NotFound(new { message = "Quiz Not Found" });

            return Ok(quiz);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [TeacherAuthorize]
        [HttpGet("Teacher/{teacherId}", Name = "GetQuizzesByTeacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetQuizDto>> GetQuizzesByTeacher(int teacherId)
        {
            if (teacherId <= 0) return BadRequest(new { message = "Invalid Teacher ID" });

            var quizzes = _quizService.GetQuizzesByTeacherId(teacherId);

            if (quizzes.IsNullOrEmpty()) return NotFound(new { message = "No Quizzes Found" });

            return Ok(quizzes);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPost("Add", Name = "AddQuiz")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddQuiz([FromBody] AddQuizDto addQuizDto)
        {
            if (addQuizDto == null || string.IsNullOrWhiteSpace(addQuizDto.Title) || addQuizDto.DurationMinutes <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            int quizId = _quizService.AddNewQuiz(addQuizDto);

            if (quizId == -1)
            {
                return BadRequest(new { message = "Lesson Not Found" });
            }

            if (quizId > 0)
            {
                return Ok(new
                {
                    message = "Add Quiz Successfully",
                    quizId = quizId
                });
            }

            return BadRequest(new { message = "Add Quiz Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPut("Update", Name = "UpdateQuiz")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateQuiz([FromBody] UpdateQuizDto updateQuizDto)
        {
            if (updateQuizDto == null || updateQuizDto.QuizId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            bool isUpdated = _quizService.UpdateQuiz(updateQuizDto);

            if (isUpdated)
                return Ok(new { message = "Update Quiz Successfully" });

            return NotFound(new { message = "Update Quiz Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpDelete("{quizId}", Name = "DeleteQuiz")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteQuiz(int quizId)
        {
            if (quizId <= 0) return BadRequest(new { message = "Invalid Data" });

            bool isDeleted = _quizService.DeleteQuizWithDependencies(quizId);

            if (isDeleted)
                return Ok(new { message = "Delete Quiz Successfully" });

            return NotFound(new { message = "Delete Quiz Failed,or not Found" });
        }
    }
}