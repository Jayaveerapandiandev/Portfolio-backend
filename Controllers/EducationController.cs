using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_Api.Bll;
using Portfolio_Api.DTO.Request;

namespace Portfolio_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : ControllerBase
    {
        private readonly EducationBLL _educationBll = new EducationBLL();

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddEducation([FromBody] EducationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _educationBll.AddEducationAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetEducation")]
        public async Task<IActionResult> GetEducation()
        {
            var response = await _educationBll.GetEducationAsync();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("DeleteEducation/{id}")]
        public async Task<IActionResult> DeleteEducation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { success = false, message = "Education id is required" });

            var response = await _educationBll.DeleteEducationAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("UpdateEducation/{id}")]
        public async Task<IActionResult> UpdateEducation(string id, [FromBody] EducationRequest request)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { success = false, message = "Education id is required" });

            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _educationBll.UpdateEducationAsync(id, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }

}
