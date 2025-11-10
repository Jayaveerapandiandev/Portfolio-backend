namespace Portfolio_Api.DTO.Response
{
  
        public class CompanyWithExperiencesResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public List<CompanyExperienceData>? DataList { get; set; }
        }

        public class CompanyExperienceData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Location { get; set; }
            public string? Website { get; set; }
            public string? LogoUrl { get; set; }
            public List<ExperienceDetailData> Experiences { get; set; } = new();
        }

        public class ExperienceDetailData
        {
            public int Id { get; set; }
            public string Designation { get; set; } = string.Empty;
            public string? PositionTitle { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsCurrentCompany { get; set; }
            public string? Description { get; set; }
            public string? TechnologiesUsed { get; set; }
        }
}

