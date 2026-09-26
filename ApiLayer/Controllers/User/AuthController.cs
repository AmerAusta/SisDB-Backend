using BusinessLayer.User.Login;
using BusinessLayer.User.Login.LoginDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly Auth _authService;

        public AuthController(ILogger<AuthController> logger, Auth authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

           
            if (!request.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Only Gmail accounts are allowed." });
            }


            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _authService.LoginAsync(request);

            if (!result.IsAuthenticated)
            {
                _logger.LogWarning("Failed login attempt. Email={Email}, IP={IP}", request.Email, ip);

                return Unauthorized("Invalid credentials,Failed login check Email and Password"); 
            }

            _logger.LogInformation("Successful login. Email={Email}, IP={IP}", request.Email, ip);

            return Ok(new
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            });
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {

            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email is required." });
            }


            if (!request.Email.Contains("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Only Gmail accounts are allowed." });
            }


            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _authService.RefreshTokenAsync(request);

            if (!result.IsAuthenticated)
            {
                _logger.LogWarning("Failed Refresh attempt. Email={Email}, IP={IP}", request.Email, ip);

                return Unauthorized("Invalid credentials,Failed login check Email and Password");
            }

            _logger.LogInformation("Successful Refrsh. Email={Email}, IP={IP}", request.Email, ip);

            return Ok(new
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            if (_authService.Logout(request))
            {
                return Ok("Logged out successfully");
            }
            else
            {
                return Ok("Logged out Faild");
            }
        }
    }
}
