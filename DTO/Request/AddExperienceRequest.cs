namespace Portfolio_Api.DTO.Request
{
    public class AddExperienceRequest
    {
        public CompanyRequest Company { get; set; } = new();
        public ExperienceRequest Experience { get; set; } = new();
    }

    public class CompanyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class ExperienceRequest
    {
        public string Designation { get; set; } = string.Empty;
        public string? PositionTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentCompany { get; set; }
        public string? Description { get; set; }
        public string? TechnologiesUsed { get; set; }
        public int? ParentExperienceId { get; set; }
    }
}
