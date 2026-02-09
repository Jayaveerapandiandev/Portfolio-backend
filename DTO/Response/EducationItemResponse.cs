namespace Portfolio_Api.DTO.Response
{
    public class EducationItemResponse
    {
        public string Id { get; set; }

        public string EducationType { get; set; }
        public string InstituteName { get; set; }
        public string Department { get; set; }
        public string Percentage { get; set; }

        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string Location { get; set; }

        public string Description { get; set; }
        public List<string> Highlights { get; set; }

        public InstituteLogoResponse InstituteLogo { get; set; }

        public int Order { get; set; }
        public bool IsVisible { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class InstituteLogoResponse
    {
        public string Url { get; set; }
        public string PublicId { get; set; }
    }

    public class GetEducationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<EducationItemResponse> Data { get; set; }
    }

    public class DeleteEducationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }


}
