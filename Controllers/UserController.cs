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
        UserBLL userbll = new UserBLL();
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

            var response = await userbll.CreateUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> ValidateLogin([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            
            var response = await userbll.ValidateLoginAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            
            var response = await userbll.LogoutUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> delete([FromBody] DeleteUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });


            var response = await userbll.DeleteUserAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await userbll.ChangePasswordAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


    }


}
