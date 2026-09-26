using ApiLayer.Authorization;
using BusinessLayer.Lesson;
using BusinessLayer.Lesson.LessonDto;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiLayer.Controllers.Lesson
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILogger<LessonController> _logger;
        private readonly BusinessLayer.Lesson.Lesson _lessonService;
        private readonly FileService _fileService;

        public LessonController(
            ILogger<LessonController> logger,
            BusinessLayer.Lesson.Lesson lessonService,
            FileService fileService)
        {
            _logger = logger;
            _lessonService = lessonService;
            _fileService = fileService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{lessonId}", Name = "GetLessonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetLessonDto> GetLessonById(int lessonId)
        {
            if (lessonId <= 0) return BadRequest(new { message = "Invalid ID" });

            bool isStudent = User.IsInRole("Student"); 

            var lesson = _lessonService.GetLessonById(lessonId, isStudent);

            if (lesson == null) return NotFound(new { message = "Lesson Not Found" });

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            if (!string.IsNullOrWhiteSpace(lesson.VideoUrl))
                lesson.VideoUrl = $"{baseUrl}{lesson.VideoUrl}";

            if (!string.IsNullOrWhiteSpace(lesson.PdfUrl))
                lesson.PdfUrl = $"{baseUrl}{lesson.PdfUrl}";

            return Ok(lesson);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Assignment/{assignmentId}", Name = "GetLessonsByAssignment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetLessonDto>> GetLessonsByAssignment(int assignmentId)
        {
            if (assignmentId <= 0) return BadRequest(new { message = "Invalid Assignment ID" });

            bool isStudent = User.IsInRole("Student");
            var lessons = _lessonService.GetLessonsByAssignmentId(assignmentId, isStudent);

            if (lessons.IsNullOrEmpty()) return NotFound(new { message = "No Lessons Found" });

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var lesson in lessons)
            {
                if (!string.IsNullOrWhiteSpace(lesson.VideoUrl))
                    lesson.VideoUrl = $"{baseUrl}{lesson.VideoUrl}";

                if (!string.IsNullOrWhiteSpace(lesson.PdfUrl))
                    lesson.PdfUrl = $"{baseUrl}{lesson.PdfUrl}";
            }

            return Ok(lessons);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [TeacherAuthorize]
        [HttpGet("Teacher/{teacherId}", Name = "GetLessonsByTeacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetLessonDto>> GetLessonsByTeacher(int teacherId)
        {
            if (teacherId <= 0) return BadRequest(new { message = "Invalid Teacher ID" });

            var lessons = _lessonService.GetLessonsByTeacherId(teacherId);

            if (lessons.IsNullOrEmpty()) return NotFound(new { message = "No Lessons Found" });

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var lesson in lessons)
            {
                if (!string.IsNullOrWhiteSpace(lesson.VideoUrl))
                    lesson.VideoUrl = $"{baseUrl}{lesson.VideoUrl}";

                if (!string.IsNullOrWhiteSpace(lesson.PdfUrl))
                    lesson.PdfUrl = $"{baseUrl}{lesson.PdfUrl}";
            }

            return Ok(lessons);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPost("Add", Name = "AddLesson")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
        public async Task<IActionResult> AddLesson([FromForm] AddLessonWithFilesDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Title) || dto.AssignmentId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            string? videoUrl = null;
            string? pdfUrl = null;

            if (dto.VideoFile != null)
            {
                string[] allowedVideoExtensions = { ".mp4", ".mov", ".avi", ".mkv" };
                videoUrl = await _fileService.UploadFileAsync(dto.VideoFile, "uploads/videos", allowedVideoExtensions);
                if (videoUrl == null)
                    return BadRequest(new { message = "Invalid Video Format" });
            }

            if (dto.PdfFile != null)
            {
                string[] allowedPdfExtensions = { ".pdf" };
                pdfUrl = await _fileService.UploadFileAsync(dto.PdfFile, "uploads/pdfs", allowedPdfExtensions);
                if (pdfUrl == null)
                    return BadRequest(new { message = "Invalid PDF Format" });
            }

            var addLessonDto = new AddLessonDto
            {
                AssignmentId = dto.AssignmentId,
                Title = dto.Title,
                VideoUrl = videoUrl,
                PdfUrl = pdfUrl,
                IsPublished = dto.IsPublished
            };

            int lessonId = _lessonService.AddNewLesson(addLessonDto);

            if (lessonId > 0)
            {
                return Ok(new
                {
                    message = "Lesson Added Successfully",
                    lessonId = lessonId,
                    videoUrl = videoUrl,
                    pdfUrl = pdfUrl
                });
            }

            return BadRequest(new { message = "Failed to Add Lesson. Please check Assignment existence." });
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpPut("Update", Name = "UpdateLesson")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
        public async Task<IActionResult> UpdateLesson([FromForm] UpdateLessonWithFilesDto dto)
        {
            if (dto == null || dto.LessonId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            var currentLesson = _lessonService.GetLessonById(dto.LessonId);
            if (currentLesson == null)
            {
                return NotFound(new { message = "Lesson Not Found" });
            }

            string? newVideoUrl = currentLesson.VideoUrl;
            string? newPdfUrl = currentLesson.PdfUrl;

            if (dto.VideoFile != null)
            {
                string[] allowedVideoExtensions = { ".mp4", ".mov", ".avi", ".mkv" };
                var uploadedUrl = await _fileService.UploadFileAsync(dto.VideoFile, "uploads/videos", allowedVideoExtensions);
                if (uploadedUrl == null)
                    return BadRequest(new { message = "Invalid Video Format" });

                if (!string.IsNullOrWhiteSpace(currentLesson.VideoUrl))
                {
                    _fileService.DeleteFile(currentLesson.VideoUrl);
                }
                newVideoUrl = uploadedUrl;
            }

            if (dto.PdfFile != null)
            {
                string[] allowedPdfExtensions = { ".pdf" };
                var uploadedUrl = await _fileService.UploadFileAsync(dto.PdfFile, "uploads/pdfs", allowedPdfExtensions);
                if (uploadedUrl == null)
                    return BadRequest(new { message = "Invalid PDF Format" });

                if (!string.IsNullOrWhiteSpace(currentLesson.PdfUrl))
                {
                    _fileService.DeleteFile(currentLesson.PdfUrl);
                }
                newPdfUrl = uploadedUrl;
            }

            var updateLessonDto = new UpdateLessonDto
            {
                LessonId = dto.LessonId,
                AssignmentId = dto.AssignmentId,
                Title = dto.Title,
                VideoUrl = newVideoUrl,
                PdfUrl = newPdfUrl,
                IsPublished = dto.IsPublished
            };

            bool isUpdated = _lessonService.UpdateLesson(updateLessonDto);

            if (isUpdated)
                return Ok(new { message = "Lesson Updated Successfully" });

            return BadRequest(new { message = "Lesson Update Failed. Lesson or Assignment ID not found." });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{lessonId}", Name = "DeleteLesson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteLesson(int lessonId)
        {
            if (lessonId <= 0) return BadRequest(new { message = "Invalid ID" });

            bool isDeleted = _lessonService.DeleteLesson(lessonId, _fileService);

            if (isDeleted)
                return Ok(new { message = "Lesson Deleted Successfully" });

            return NotFound(new { message = "Lesson Delete Failed,or not Found" });
        }
    }
}