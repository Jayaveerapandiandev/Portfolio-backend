using global::Portfolio_Api.Bll;
using global::Portfolio_Api.DTO.Request;
using global::Portfolio_Api.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio_Api.Controllers
{
    

   
    
        [Route("api/[controller]")]
        [ApiController]
        public class ExperienceController : ControllerBase
        {
            private readonly ExperienceBLL _bll;

            public ExperienceController()
            {
                _bll = new ExperienceBLL();
            }

        [HttpPost("add")]
        public async Task<IActionResult> AddExperience([FromBody] AddExperienceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AddExperienceResponse
                {
                    Success = false,
                    Message = "Invalid request format."
                });
            }
            var result = await _bll.AddExperienceAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<ActionResult<CompanyWithExperiencesResponse>> GetAll()
        {
            var response = await _bll.GetAllCompaniesWithExperiencesAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("addForExistingCompany")]
        public async Task<IActionResult> AddExperienceForExistingCompany([FromBody] AddExperienceForExistingCompanyRequest req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AddExperienceForExistingCompanyResponse
                {
                    Success = false,
                    Message = "Invalid request format."
                });
            }

            var result = await _bll.AddExperienceForExistingCompanyAsync(req);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("delete/{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var result = await _bll.DeleteCompanyAsync(companyId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


    }
}

