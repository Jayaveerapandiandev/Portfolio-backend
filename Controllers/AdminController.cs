using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_Api.Bll;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;

namespace Portfolio_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
     
    public class AdminController : ControllerBase
    {
        AdminHomeBLL adminhome = new AdminHomeBLL();

        [Authorize]
        [HttpPost("Save")]
        public async Task<ActionResult<SaveAdminHomeResponse>> SaveHome([FromBody] SaveAdminHomeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var result = await adminhome.SaveAdminHomeAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await adminhome.GetAdminHomeAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
