namespace Portfolio_Api.DTO.Data
{
    public class SkillData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Proficiency { get; set; }
        public int ExperienceYears { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsHighlighted { get; set; }
    }

}
