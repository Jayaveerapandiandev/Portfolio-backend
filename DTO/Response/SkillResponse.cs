using Portfolio_Api.DTO.Data;

namespace Portfolio_Api.DTO.Response
{
    public class SkillResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<SkillData> Data { get; set; } = new();
    }

}
