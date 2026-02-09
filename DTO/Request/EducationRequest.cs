namespace Portfolio_Api.DTO.Request
{
    public class EducationRequest
    {
        public string EducationType { get; set; }
        public string InstituteName { get; set; }
        public string Department { get; set; }
        public string Percentage { get; set; }

        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string Location { get; set; }

        public string Description { get; set; }
        public List<string> Highlights { get; set; }

        public string InstituteLogoUrl { get; set; }
        public string InstituteLogoPublicId { get; set; }

        public int Order { get; set; }
        public bool IsVisible { get; set; }
    }

}
