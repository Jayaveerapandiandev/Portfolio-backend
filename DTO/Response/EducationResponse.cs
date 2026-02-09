namespace Portfolio_Api.DTO.Response
{
    public class EducationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
