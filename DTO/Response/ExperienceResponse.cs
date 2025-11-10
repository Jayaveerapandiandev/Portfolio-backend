namespace Portfolio_Api.DTO.Response
{
    public class ExperienceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ExperienceId { get; set; }
        public int? CompanyId { get; set; }
    }
}
