using BusinessLayer.Comment;
using BusinessLayer.Comment.CommentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace ApiLayer.Controllers.Comment
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly BusinessLayer.Comment.Comment _commentService;

        public CommentsController(
            ILogger<CommentsController> logger,
            BusinessLayer.Comment.Comment commentService)
        {
            _logger = logger;
            _commentService = commentService;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("", Name = "GetAllComments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCommentDto>> GetAllComments()
        {
            var comments = _commentService.GetAllComments();

            if (comments.IsNullOrEmpty()) return NotFound(new { message = "No Comments Found" });

            return Ok(comments);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("{commentId}", Name = "GetCommentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetCommentDto> GetCommentById(int commentId)
        {
            if (commentId <= 0) return BadRequest(new { message = "Invalid ID" });

            var comment = _commentService.GetCommentById(commentId);

            if (comment == null) return NotFound(new { message = "Comment Not Found" });

            return Ok(comment);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Lesson/{lessonId}", Name = "GetCommentsByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCommentDto>> GetCommentsByLessonId(int lessonId)
        {
            if (lessonId <= 0) return BadRequest(new { message = "Invalid Lesson ID" });

            var comments = _commentService.GetCommentsByLessonId(lessonId);

            if (comments.IsNullOrEmpty()) return NotFound(new { message = "No Comments Found for this Lesson" });

            return Ok(comments);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Lesson/{lessonId}/Full-Details", Name = "GetCommentsWithFullDetailsByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCommentWithFullDetailsDto>> GetCommentsWithFullDetailsByLessonId(int lessonId)
        {
            if (lessonId <= 0) return BadRequest(new { message = "Invalid Lesson ID" });

            var comments = _commentService.GetCommentsWithFullDetailsByLessonId(lessonId);

            if (comments.IsNullOrEmpty()) return NotFound(new { message = "No Full Details Comments Found for this Lesson" });

            return Ok(comments);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("Full-Details", Name = "GetAllCommentsWithFullDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetCommentWithFullDetailsDto>> GetAllCommentsWithFullDetails()
        {
            var comments = _commentService.GetAllCommentsWithFullDetails();

            if (comments.IsNullOrEmpty()) return NotFound(new { message = "No Comments Found" });

            return Ok(comments);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpPost("Add", Name = "AddNewComment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddNewComment([FromBody] AddCommentDto addDto)
        {
            if (addDto == null || addDto.LessonId <= 0 || addDto.UserId <= 0 || string.IsNullOrWhiteSpace(addDto.Content))
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            int commentId = _commentService.AddNewComment(addDto);

            if (commentId == -1)
                return BadRequest(new { message = "Lesson ID not found." });

            if (commentId == -2)
                return BadRequest(new { message = "User ID not found." });

            if (commentId > 0)
            {
                return Ok(new
                {
                    message = "Comment Added Successfully",
                    commentId = commentId
                });
            }

            return BadRequest(new { message = "Failed to Add Comment." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpPut("Update", Name = "UpdateComment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateComment([FromBody] UpdateCommentDto updateDto)
        {
            if (updateDto == null || updateDto.CommentId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            bool isUpdated = _commentService.UpdateComment(updateDto);

            if (isUpdated)
                return Ok(new { message = "Comment Updated Successfully" });

            return BadRequest(new { message = "Comment Update Failed. Comment ID not found." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpDelete("{commentId}", Name = "DeleteComment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteComment(int commentId)
        {
            if (commentId <= 0) return BadRequest(new { message = "Invalid ID" });

            bool isDeleted = _commentService.DeleteComment(commentId);

            if (isDeleted)
                return Ok(new { message = "Comment Deleted Successfully" });

            return NotFound(new { message = "Comment Delete Failed. Comment ID not found." });
        }
    }
}