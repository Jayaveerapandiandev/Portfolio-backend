using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_Api.Bll;
using Portfolio_Api.DTO.Request;

namespace Portfolio_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : ControllerBase
    {
        AboutBLL aboutbll = new AboutBLL();

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> SaveAbout([FromBody] AboutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await aboutbll.SaveAboutAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetAbout()
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await aboutbll.GetAboutAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }

}
