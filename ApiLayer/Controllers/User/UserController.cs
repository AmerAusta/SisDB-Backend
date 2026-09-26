using ApiLayer.Authorization;
using BusinessLayer.Role;
using BusinessLayer.User;
using BusinessLayer.User.Login;
using BusinessLayer.User.UserDto;
using DataLayer.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ApiLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly BusinessLayer.User.User _UserService;
        private readonly BusinessLayer.Role.Role _RoleServer;

        public UserController(ILogger<UserController> logger, BusinessLayer.User.User userService, BusinessLayer.Role.Role roleServer )
        {
            _logger = logger;
            _UserService = userService;
            _RoleServer = roleServer;
        }




        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student,Parent")]
        [UserAuthorize]
        [HttpGet("{UserId}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetUserDto>> GetUserById(int UserId)
        {
            if (UserId <= 0) return BadRequest(new { message = "Invalid Data" });

            var user= _UserService.GetUserById(UserId);

            if (user == null) return NotFound("User Not Found");

            return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("Name/{fullName}", Name = "GetUserByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetUserDto>> GetUserByName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return BadRequest(new { message = "Invalid Data" });

            var user = _UserService.GetUsersByName(fullName);

            if (user.IsNullOrEmpty())
                return NotFound(new { message = "User Not Found" });

            return Ok(user);
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Add" ,Name="AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddUser( AddUserDto addUserDto)
        {
            if (string.IsNullOrWhiteSpace(addUserDto.FirstName) ||
                string.IsNullOrWhiteSpace(addUserDto.SecondName) ||
                string.IsNullOrWhiteSpace(addUserDto.LastName) ||
                string.IsNullOrWhiteSpace(addUserDto.Email) ||
                string.IsNullOrWhiteSpace(addUserDto.Password) ||
                string.IsNullOrWhiteSpace(addUserDto.PhoneNumber)||
                addUserDto.RoleId <= 0)
                return BadRequest(new {message ="Invalid Data"});

            if(!_RoleServer.IsValidRoleId(addUserDto.RoleId))
            {
                return BadRequest(new { message = "Role is not found" });
            }

            if(addUserDto.RoleId == 1)
            {
                return BadRequest(new { message = "Can not Add Super Admin " });
            }

            if(!addUserDto.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid Data Email must contain @gmail.com" });
            }

            if (!Regex.IsMatch(addUserDto.PhoneNumber ?? "", @"^09\d{8}$"))
            {
                return BadRequest(new { message = "Phone number must be 09********" });
            }

            if(_UserService.IsEmailExists(addUserDto.Email))
            {
                return BadRequest(new { message = "Email is already exists" });
            }

            if (_UserService.IsPhoneNumberExists(addUserDto.PhoneNumber))
            {
                return BadRequest(new { message = "Phone Number is already exists" });
            }


            int UserId = _UserService.AddNewUser(addUserDto);

            if (UserId>0)
                return Ok(new
                {
                    message = "ADD User Successfully",
                    userId = UserId
                });

            return BadRequest(new { message = "Add User Faild" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("update",Name ="UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateUser(UpdateUserDto updateUserDto)
        {
            if(updateUserDto.UserID<=0) 
                return BadRequest(new { message = "Invalid ID" });

            if (updateUserDto.UserID <= 2)
                return BadRequest(new { message = "Can Update SuperAdmin and Admin" });

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email) &&
                !updateUserDto.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid Email: must contain @gmail.com" });
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber) &&
                !Regex.IsMatch(updateUserDto.PhoneNumber, @"^09\d{8}$"))
            {
                return BadRequest(new { message = "Phone number must be 09********" });
            }

            if (_UserService.IsEmailExists(updateUserDto.Email,updateUserDto.UserID))
            {
                return BadRequest(new { message = "Email is already exists" });
            }

            if (_UserService.IsPhoneNumberExists(updateUserDto.PhoneNumber, updateUserDto.UserID))
            {
                return BadRequest(new { message = "Phone Number is already exists" });
            }


            bool isUpdated = _UserService.UpdateUser(updateUserDto);

            if (isUpdated)
                return Ok(new { message = "Update User Succefully" });

            return NotFound(new { message = "Update User Faild" });
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}",Name="DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid Data" });

            if (id <= 2) return BadRequest(new { message = "Can Delete SuperAdmin and Admin" });

            bool isDeleted = _UserService.DeleteUser(id);

            if (isDeleted)
                return Ok(new { message = "Delete User Succefully" });

            return NotFound(new { message = "Delete User Faild" });
        }
    }
}
