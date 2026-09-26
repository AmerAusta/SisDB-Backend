using BusinessLayer.Question;
using BusinessLayer.Question.QuestionDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.Question
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ILogger<QuestionController> _logger;
        private readonly BusinessLayer.Question.Question _questionService;

        public QuestionController(ILogger<QuestionController> logger, BusinessLayer.Question.Question questionService)
        {
            _logger = logger;
            _questionService = questionService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("All", Name = "GetAllQuestions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetQuestionDto>> GetAllQuestions()
        {
            var questions = _questionService.GetAllQuestions();
            if (questions.IsNullOrEmpty()) return NotFound(new { message = "No Questions Found" });

            return Ok(questions);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{questionId}", Name = "GetQuestionById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetQuestionDto> GetQuestionById(int questionId)
        {
            if (questionId <= 0) return BadRequest(new { message = "Invalid ID" });

            var question = _questionService.GetQuestionById(questionId);
            if (question == null) return NotFound(new { message = "Question Not Found" });

            return Ok(question);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Quiz/{quizId}", Name = "GetQuestionsByQuizId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetQuestionDto>> GetQuestionsByQuizId(int quizId)
        {
            if (quizId <= 0) return BadRequest(new { message = "Invalid ID" });

            var questions = _questionService.GetQuestionsByQuizId(quizId);
            if (questions.IsNullOrEmpty()) return NotFound(new { message = "No Questions Found for this Quiz" });

            return Ok(questions);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPost("Add", Name = "AddQuestion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddQuestion([FromBody] AddQuestionDto addQuestionDto)
        {
            if (addQuestionDto == null || string.IsNullOrWhiteSpace(addQuestionDto.QuestionText) || addQuestionDto.Points <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            int questionId = _questionService.AddNewQuestion(addQuestionDto);

            if (questionId == -1)
            {
                return BadRequest(new { message = "Quiz Not Found" });
            }

            if (questionId > 0)
            {
                return Ok(new
                {
                    message = "Add Question Successfully",
                    questionId = questionId
                });
            }

            return BadRequest(new { message = "Add Question Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPut("Update", Name = "UpdateQuestion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateQuestion([FromBody] UpdateQuestionDto updateQuestionDto)
        {
            if (updateQuestionDto == null || updateQuestionDto.QuestionId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            bool isUpdated = _questionService.UpdateQuestion(updateQuestionDto);

            if (isUpdated)
                return Ok(new { message = "Update Question Successfully" });

            return NotFound(new { message = "Update Question Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpDelete("{questionId}", Name = "DeleteQuestion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteQuestion(int questionId)
        {
            if (questionId <= 0) return BadRequest(new { message = "Invalid Data" });

            bool isDeleted = _questionService.DeleteQuestionWithChoices(questionId);

            if (isDeleted)
                return Ok(new { message = "Delete Question Successfully" });

            return NotFound(new { message = "Delete Question Failed" });
        }
    }
}
