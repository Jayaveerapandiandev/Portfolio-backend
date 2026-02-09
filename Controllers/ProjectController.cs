using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_Api.Bll;
using Portfolio_Api.DTO.Request;

namespace Portfolio_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        ProjectBLL projectbll = new ProjectBLL();

        [HttpGet("all")]
        public async Task<IActionResult> GetProjects()
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });
            var response = await projectbll.GetProjectsAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
           
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddProject([FromBody] ProjectRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });
            var response = await projectbll.AddProjectAsync(req);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
          
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var response = await projectbll.UpdateProjectAsync(id , req);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
           
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });
            var response = await projectbll.DeleteProjectAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
           
        }
    }
}
