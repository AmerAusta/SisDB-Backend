using BusinessLayer.QuestionChoice;
using BusinessLayer.QuestionChoice.QuestionChoiceDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.QuestionChoice
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionChoiceController : ControllerBase
    {
        private readonly ILogger<QuestionChoiceController> _logger;
        private readonly BusinessLayer.QuestionChoice.QuestionChoice _choiceService;

        public QuestionChoiceController(ILogger<QuestionChoiceController> logger, BusinessLayer.QuestionChoice.QuestionChoice choiceService)
        {
            _logger = logger;
            _choiceService = choiceService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("All", Name = "GetAllChoices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetQuestionChoiceDto>> GetAllChoices()
        {
            var choices = _choiceService.GetAllChoices();
            if (choices.IsNullOrEmpty()) return NotFound(new { message = "No Choices Found" });

            return Ok(choices);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{choiceId}", Name = "GetChoiceById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetQuestionChoiceDto> GetChoiceById(int choiceId)
        {
            if (choiceId <= 0) return BadRequest(new { message = "Invalid ID" });

            var choice = _choiceService.GetChoiceById(choiceId);
            if (choice == null) return NotFound(new { message = "Choice Not Found" });

            return Ok(choice);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Question/{questionId}", Name = "GetChoicesByQuestionId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetQuestionChoiceDto>> GetChoicesByQuestionId(int questionId)
        {
            if (questionId <= 0) return BadRequest(new { message = "Invalid ID" });

            var choices = _choiceService.GetChoicesByQuestionId(questionId);
            if (choices.IsNullOrEmpty()) return NotFound(new { message = "No Choices Found for this Question" });

            return Ok(choices);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPost("Add", Name = "AddChoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddChoice([FromBody] AddQuestionChoiceDto addChoiceDto)
        {
            if (addChoiceDto == null || string.IsNullOrWhiteSpace(addChoiceDto.ChoiceText))
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            int choiceId = _choiceService.AddNewChoice(addChoiceDto);

            if (choiceId == -1)
            {
                return BadRequest(new { message = "Question Not Found" });
            }

            if (choiceId > 0)
            {
                return Ok(new
                {
                    message = "Add Choice Successfully",
                    choiceId = choiceId
                });
            }

            return BadRequest(new { message = "Add Choice Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPut("Update", Name = "UpdateChoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateChoice([FromBody] UpdateQuestionChoiceDto updateChoiceDto)
        {
            if (updateChoiceDto == null || updateChoiceDto.ChoiceId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            bool isUpdated = _choiceService.UpdateChoice(updateChoiceDto);

            if (isUpdated)
                return Ok(new { message = "Update Choice Successfully" });

            return NotFound(new { message = "Update Choice Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpDelete("{choiceId}", Name = "DeleteChoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteChoice(int choiceId)
        {
            if (choiceId <= 0) return BadRequest(new { message = "Invalid Data" });

            bool isDeleted = _choiceService.DeleteChoice(choiceId);

            if (isDeleted)
                return Ok(new { message = "Delete Choice Successfully" });

            return NotFound(new { message = "Delete Choice Failed,or not Found" });
        }
    }
}