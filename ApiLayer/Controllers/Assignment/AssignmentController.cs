using ApiLayer.Authorization;
using BusinessLayer.Assignments;
using BusinessLayer.Assignments.AssignmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiLayer.Controllers.Assignment
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly ILogger<AssignmentController> _logger;
        private readonly BusinessLayer.Assignments.Assignment _assignmentService;
        private readonly BusinessLayer.User.User  _userServer;
        private readonly BusinessLayer.Classes.Class _classServer;
        private readonly BusinessLayer.Teacher.Teacher _teacherServer;
        private readonly BusinessLayer.Lesson.Lesson _lessonServer;

        public AssignmentController(ILogger<AssignmentController> logger, BusinessLayer.Assignments.Assignment assignmentService,
            BusinessLayer.User.User userServer, BusinessLayer.Classes.Class classServer
            , BusinessLayer.Teacher.Teacher teacherServer, BusinessLayer.Lesson.Lesson lessonServer)
        {
            _logger = logger;
            _assignmentService = assignmentService;
            _userServer = userServer;
            _classServer = classServer;
            _teacherServer = teacherServer;
            _lessonServer = lessonServer;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("All", Name = "GetAllAssignments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetAssignmentDto>> GetAllAssignments()
        {
            var assignments = _assignmentService.GetAllAssignments();
            if (assignments.IsNullOrEmpty()) return NotFound(new { message = "No Assignments Found" });

            return Ok(assignments);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("{assignmentId}", Name = "GetAssignmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetAssignmentDto> GetAssignmentById(int assignmentId)
        {
            if (assignmentId <= 0) return BadRequest(new { message = "Invalid ID" });

            var assignment = _assignmentService.GetAssignmentById(assignmentId);
            if (assignment == null) return NotFound(new { message = "Assignment Not Found" });

            return Ok(assignment);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("Teacher/{teacherId}", Name = "GetAssignmentsByTeacher")]
        [TeacherAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetAssignmentDto>> GetAssignmentsByTeacher(int teacherId)
        {
            if (teacherId <= 0) return BadRequest(new { message = "Invalid Teacher ID" });

            if (!_teacherServer.IsTeacher(teacherId))
            {
                return BadRequest(new { message = "Id is not for teacher" });
            }


            var assignments = _assignmentService.GetAssignmentsByTeacherId(teacherId);
            if (assignments.IsNullOrEmpty()) return NotFound(new { message = "No Assignments Found For This Teacher" });

            return Ok(assignments);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        [HttpGet("Class/{classId}", Name = "GetAssignmentsByClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetAssignmentDto>> GetAssignmentsByClass(int classId)
        {
            if (classId <= 0) return BadRequest(new { message = "Invalid Class ID" });

            var cls=_classServer.GetClassById(classId);
            if(cls==null)
            {
                return BadRequest(new { message = "Class not Found" });
            }

            var assignments = _assignmentService.GetAssignmentsByClassId(classId);
            if (assignments.IsNullOrEmpty()) return NotFound(new { message = "No Assignments Found For This Class" });

            return Ok(assignments);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Add", Name = "AddAssignment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddAssignment([FromBody] AddAssignmentDto addAssignmentDto)
        {
            if (addAssignmentDto == null || addAssignmentDto.TeacherId <= 0 || addAssignmentDto.SubjectId <= 0 || addAssignmentDto.ClassId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }


           if(!_teacherServer.IsTeacher(addAssignmentDto.TeacherId))
           {
                return BadRequest(new { message = "Teacher Id is invaled" });
           }

            if (_assignmentService.IsAssignmentExists(addAssignmentDto.TeacherId, addAssignmentDto.SubjectId, addAssignmentDto.ClassId))
            {
                return BadRequest(new { message = "This assignment already exists" });
            }

            int assignmentId = _assignmentService.AddNewAssignment(addAssignmentDto);

            if (assignmentId > 0)
            {
                return Ok(new
                {
                    message = "Assignment Added Successfully",
                    assignmentId = assignmentId
                });
            }

            return BadRequest(new { message = "Failed to Add Assignment. Please check Teacher, Subject, or Class existence." });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("Update", Name = "UpdateAssignment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateAssignment([FromBody] UpdateAssignmentDto updateAssignmentDto)
        {
            if (updateAssignmentDto == null || updateAssignmentDto.AssignmentId <= 0)
            {
                return BadRequest(new { message = "Invalid Data" });
            }

            if (updateAssignmentDto.TeacherId.HasValue)
            {
                if (updateAssignmentDto.TeacherId.Value <= 0 || !_teacherServer.IsTeacher(updateAssignmentDto.TeacherId.Value))
                {
                    return BadRequest(new { message = "Teacher Id is invalid" });
                }
            }

            bool isUpdated = _assignmentService.UpdateAssignment(updateAssignmentDto);

            if (isUpdated)
                return Ok(new { message = "Assignment Updated Successfully" });

            return BadRequest(new { message = "Assignment Update Failed. Duplicate assignment or invalid IDs." });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{assignmentId}", Name = "DeleteAssignment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteAssignment(int assignmentId)
        {
            if (assignmentId <= 0) return BadRequest(new { message = "Invalid ID" });

            if(_lessonServer.AssignmentExists(assignmentId))
            {
                return BadRequest(new
                {
                    message = "Can not delete this assignment now, Because it is linked to one or more lessons."
                });
            }

            bool isDeleted = _assignmentService.DeleteAssignment(assignmentId);

            if (isDeleted)
                return Ok(new { message = "Assignment Deleted Successfully" });

            return NotFound(new { message = "Assignment Delete Failed,or not Found" });
        }
    }
}