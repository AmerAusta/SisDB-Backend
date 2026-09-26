using BusinessLayer.Classes;
using BusinessLayer.Classes.ClassDto;
using BusinessLayer.Subject.SubjectDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly ILogger<ClassController> _logger;
        private readonly Class _classService;

        public ClassController(ILogger<ClassController> logger, Class classService)
        {
            _logger = logger;
            _classService = classService;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("{classId}", Name = "GetClassById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetClassDto> GetClassById(int classId)
        {
            if (classId <= 0) return BadRequest(new { message = "Invalid Data" });

            var cls = _classService.GetClassById(classId);

            if (cls == null) return NotFound(new { message = "Class Not Found" });

            return Ok(cls);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("all", Name = "GetAllClasses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<GetClassDto>> GetAllClasses()
        {
            var classes = _classService.GetAllClasses();
            return Ok(classes);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("Name/{className}", Name = "GetClassByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetClassDto> GetClassByName(string className, string? academicYear = null)
        {
            if (string.IsNullOrWhiteSpace(className))
                return BadRequest(new { message = "Invalid Data" });

            var cls = _classService.GetClassByName(className,academicYear);

            if (cls == null)
                return NotFound(new { message = "Class Not Found" });

            return Ok(cls);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
        [HttpGet("Branch/{classBranch}", Name = "GetClassByBranch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetClassDto> GetClassByBranch(string classBranch, string? academicYear = null)
        {
            if (string.IsNullOrWhiteSpace(classBranch))
                return BadRequest(new { message = "Invalid Data" });

            var cls = _classService.GetClassByBranch(classBranch,academicYear);

            if (cls == null)
                return NotFound(new { message = "Class Not Found" });

            return Ok(cls);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Add", Name = "AddClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddClass([FromBody] AddClassDto addClassDto)
        {
            if (string.IsNullOrWhiteSpace(addClassDto.ClassName) ||
                string.IsNullOrWhiteSpace(addClassDto.Branch) ||
                string.IsNullOrWhiteSpace(addClassDto.AcademicYear) ||
                addClassDto.Capacity <= 0)
                return BadRequest(new { message = "Invalid Data" });

            if (_classService.IsClassNameExists(addClassDto.ClassName))
            {
                return BadRequest(new { message = "Subject Name already exists" });
            }

            bool isAdded = _classService.AddNewClass(addClassDto);

            if (isAdded)
                return Ok(new { message = "Add Class Successfully" });

            return BadRequest(new { message = "Add Class Failed" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("update", Name = "UpdateClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateClass([FromBody] UpdateClassDto updateClassDto)
        {
            if (updateClassDto.ClassId <= 0)
                return BadRequest(new { message = "Invalid ID" });

            if (!string.IsNullOrWhiteSpace(updateClassDto.ClassName)&&_classService.IsClassNameExists(updateClassDto.ClassName,updateClassDto.ClassId))
            {
                return BadRequest(new { message = "Subject Name already exists" });
            }


            if (updateClassDto.Capacity.HasValue && updateClassDto.Capacity.Value <= 0)
                return BadRequest(new { message = "Capacity must be greater than 0" });



            bool isUpdated = _classService.UpdateClass(updateClassDto);

            if (isUpdated)
                return Ok(new { message = "Update Class Successfully" });

            return NotFound(new { message = "Update Class Failed" });
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id:int}", Name = "DeleteClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteClass(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid Data" });

            if(!_classService.CheackCapacity(id))
            {
                return BadRequest(new { message = "Class have Student Can not delete " });
            }

            bool isDeleted = _classService.DeleteClass(id);

            if (isDeleted)
                return Ok(new { message = "Delete Class Successfully" });

            return NotFound(new { message = "Delete Class Failed" });
        }
    }
}