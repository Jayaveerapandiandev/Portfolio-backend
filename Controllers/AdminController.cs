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
        [HttpPost("Save")]
        public async Task<ActionResult<SaveAdminHomeResponse>> SaveHome([FromBody] SaveAdminHomeRequest request)
        {
            var result = await adminhome.SaveAdminHomeAsync(request);
            return Ok(result);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var response = await adminhome.GetAdminHomeAsync();
            return Ok(response);
        }
    }
}
