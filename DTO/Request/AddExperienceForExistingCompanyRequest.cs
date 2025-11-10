namespace Portfolio_Api.DTO.Request
{
    public class AddExperienceForExistingCompanyRequest
    {
        public int CompanyId { get; set; }

        public ExistingExperienceData Experience { get; set; } = new();
    }

    public class ExistingExperienceData
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
