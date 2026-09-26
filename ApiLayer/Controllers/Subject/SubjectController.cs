using BusinessLayer.Subject;
using BusinessLayer.Subject.SubjectDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.Subject
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ILogger<SubjectController> _logger;
        private readonly BusinessLayer.Subject.Subject _subjectService;
        private readonly BusinessLayer.Teacher.Teacher _teacherService;
        private readonly BusinessLayer.Assignments.Assignment _assignmentService;

        public SubjectController(ILogger<SubjectController> logger, BusinessLayer.Subject.Subject subjectService,
            BusinessLayer.Teacher.Teacher teacherService, BusinessLayer.Assignments.Assignment assignmentService)
        {
            _logger = logger;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _assignmentService = assignmentService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("All", Name = "GetAllSubjects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetSubjectDto>> GetAllSubjects()
        {
            var subjects = _subjectService.GetAllSubjects();
            if (subjects.IsNullOrEmpty()) return NotFound(new { message = "No Subjects Found" });

            return Ok(subjects);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{subjectId}", Name = "GetSubjectById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetSubjectDto> GetSubjectById(int subjectId)
        {
            if (subjectId <= 0) return BadRequest(new { message = "Invalid ID" });

            var subject = _subjectService.GetSubjectById(subjectId);
            if (subject == null) return NotFound(new { message = "Subject Not Found" });

            return Ok(subject);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Search/{subjectName}", Name = "GetSubjectsByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetSubjectDto>> GetSubjectsByName(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName))
                return BadRequest(new { message = "Invalid Data" });

            var subjects = _subjectService.GetSubjectsByName(subjectName);

            if (subjects.IsNullOrEmpty())
                return NotFound(new { message = "No Subjects Found" });

            return Ok(subjects);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Add", Name = "AddSubject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddSubject([FromBody] AddSubjectDto addSubjectDto)
        {
            if (addSubjectDto == null || string.IsNullOrWhiteSpace(addSubjectDto.SubjectName))
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            if (_subjectService.IsSubjectNameExists(addSubjectDto.SubjectName))
            {
                return BadRequest(new { message = "Subject Name already exists" });
            }

            int subjectId = _subjectService.AddNewSubject(addSubjectDto);

            if (subjectId > 0)
            {
                return Ok(new
                {
                    message = "Add Subject Successfully",
                    subjectId = subjectId
                });
            }

            return BadRequest(new { message = "Add Subject Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("Update", Name = "UpdateSubject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateSubject([FromBody] UpdateSubjectDto updateSubjectDto)
        {
            if (updateSubjectDto == null || updateSubjectDto.SubjectId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            if (!string.IsNullOrWhiteSpace(updateSubjectDto.SubjectName)&&_subjectService.IsSubjectNameExists(updateSubjectDto.SubjectName, updateSubjectDto.SubjectId))
            {
                return BadRequest(new { message = "Subject Name already exists" });
            }

            bool isUpdated = _subjectService.UpdateSubject(updateSubjectDto);

            if (isUpdated)
                return Ok(new { message = "Update Subject Successfully" });

            return NotFound(new { message = "Update Subject Failed" });
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{subjectId}", Name = "DeleteSubject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteSubject(int subjectId)
        {
            if (subjectId <= 0) return BadRequest(new { message = "Invalid Data" });

            if(_assignmentService.IsAssigmenthaveThisSubject(subjectId))
            {
                return BadRequest(new { message = "Can not delete this subject Because it is linked to one or more Assigment" });
            }

            if (_teacherService.IsTeacherhaveThisSubject(subjectId))
            {
                return BadRequest(new { message = "Can not delete this subject Because it is linked to one or more Teacher" });
            }

            bool isDeleted = _subjectService.DeleteSubject(subjectId);

            if (isDeleted)
                return Ok(new { message = "Delete Subject Successfully" });

            return NotFound(new { message = "Delete Subject Failed,or not Found" });
        }
    }
}