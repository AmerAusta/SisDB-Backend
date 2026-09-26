using BusinessLayer.Role.RoleDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiLayer.Controllers.Role
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly BusinessLayer.Role.Role _roleService;

        public RoleController(ILogger<RoleController> logger, BusinessLayer.Role.Role roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("{roleId}", Name = "GetRoleById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetRoleDto> GetRoleById(int roleId)
        {
            if (roleId <= 0) return BadRequest(new { message = "Invalid Data" });

            var role = _roleService.GetRoleById(roleId);

            if (role == null) return NotFound(new { message = "Role Not Found" });

            return Ok(role);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("allIsActiveForAdd", Name = "GetAllRolesIsActiveForAdd")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<GetRoleDto>> GetAllRolesIsActiveForAdd()
        {
            var roles = _roleService.GetAllRolesIsActiveForAdd();
            return Ok(roles);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("allIsActive", Name = "GetAllRolesIsActive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<GetRoleDto>> GetAllRolesIsActive()
        {
            var roles = _roleService.GetAllRolesIsActive();
            return Ok(roles);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("all", Name = "GetAllRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<GetRoleDto>> GetAllRoles()
        {
            var roles = _roleService.GetAllRoles();
            return Ok(roles);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("Add", Name = "AddRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddRole([FromBody] AddRoleDto addRoleDto)
        {
            if (string.IsNullOrWhiteSpace(addRoleDto.RoleName))
                return BadRequest(new { message = "Invalid Data" });

            bool isAdded = _roleService.AddNewRole(addRoleDto);

            if (isAdded)
                return Ok(new { message = "Add Role Successfully" });

            return BadRequest(new { message = "Add Role Failed" });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("update", Name = "UpdateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            if (updateRoleDto.RoleId <= 0)
                return BadRequest(new { message = "Invalid Role ID" });

            if (string.IsNullOrWhiteSpace(updateRoleDto.RoleName))
                return BadRequest(new { message = "Invalid Data" });

            bool isUpdated = _roleService.UpdateRole(updateRoleDto);

            if (isUpdated)
                return Ok(new { message = "Update Role Successfully" });

            return NotFound(new { message = "Update Role Failed" });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id:int}", Name = "DeleteRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRole(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid Data" });

            if (_roleService.IsValidRoleId(id))
            {
                return BadRequest(new { message = "Role is not valid, cannot delete" });
            }

            bool isDeleted = _roleService.DeleteRole(id);

            if (isDeleted)
                return Ok(new { message = "Delete Role Successfully" });

            return NotFound(new { message = "Delete Role Failed" });
        }
    }
}

