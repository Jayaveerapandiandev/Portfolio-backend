using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_Api.Bll;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;

namespace Portfolio_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserBLL _userbll; // 🔹 Make it a field

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _userbll = new UserBLL(_configuration); // 🔹 Initialize with configuration
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new CreateUserResponse
                {
                    Success = false,
                    Message = "Invalid input data."
                });
            }

            var response = await _userbll.CreateUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> ValidateLogin([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _userbll.ValidateLoginAsync(request); // 🔹 Use _userbll

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _userbll.LogoutUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _userbll.DeleteUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _userbll.ChangePasswordAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}