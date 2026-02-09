using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_Api.Bll;
using Portfolio_Api.DTO.Request;

namespace Portfolio_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        SkillBll _bll = new SkillBll();

        [HttpGet("GetSkills")]
        public async Task<IActionResult> Get()
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _bll.GetAllSkillsAsync();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("AddNewSkill")]
        public async Task<IActionResult> Create([FromBody] CreateSkillRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _bll.CreateSkillAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("UpdateSkills/{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] UpdateSkillRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await _bll.UpdateSkillAsync(id, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("deleteSkill/{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var response = await _bll.DeleteSkillAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }




    }
}
