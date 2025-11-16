namespace Portfolio_Api.DTO.Request
{
    public class UpdateSkillRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Proficiency { get; set; }
        public int ExperienceYears { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsHighlighted { get; set; }
    }

}
