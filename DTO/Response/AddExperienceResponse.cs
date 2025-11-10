namespace Portfolio_Api.DTO.Response
{
    public class AddExperienceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ExperienceCreatedData? Data { get; set; }
    }

    public class ExperienceCreatedData
    {
        public int CompanyId { get; set; }
        public int ExperienceId { get; set; }
        public CompanySummary Company { get; set; } = new();
        public ExperienceSummary Experience { get; set; } = new();
    }

    public class CompanySummary
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class ExperienceSummary
    {
        public string Designation { get; set; } = string.Empty;
        public string? PositionTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentCompany { get; set; }
        public string? Description { get; set; }
        public string? TechnologiesUsed { get; set; }
    }
}
