namespace Portfolio_Api.DTO.Response
{
   
        public class AddExperienceForExistingCompanyResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public ExperienceAddedData? Data { get; set; }
        }

        public class ExperienceAddedData
        {
            public int ExperienceId { get; set; }
            public int CompanyId { get; set; }
            public string Designation { get; set; } = string.Empty;
            public string? PositionTitle { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsCurrentCompany { get; set; }
        }
}
