namespace Portfolio_Api.DTO.Response
{
    public class DeleteExperienceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DeletedExperienceData? Data { get; set; }
    }

    public class DeletedExperienceData
    {
        public int ExperienceId { get; set; }
    }
}
