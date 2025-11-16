using Portfolio_Api.DTO.Data;

namespace Portfolio_Api.DTO.Response
{
        public class CreateSkillResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public SkillData? Data { get; set; }
        }

    
}
