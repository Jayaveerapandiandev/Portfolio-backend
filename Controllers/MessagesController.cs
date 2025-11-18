using Microsoft.AspNetCore.Mvc;
using Portfolio_Api.Bll;
using Portfolio_Api.DTO.Request;

namespace Portfolio_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        MessageBLL _bll = new MessageBLL();

        [HttpPost("create")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid request data" });

            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var response = await _bll.CreateMessageAsync(request, ip);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMessages()
        {
            var response = await _bll.GetAllMessagesAsync();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPatch("mark-seen/{id}")]
        public async Task<IActionResult> MarkSeen(int id)
        {
            var response = await _bll.MarkMessageAsSeenAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var response = await _bll.DeleteMessageAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }



    }

}
