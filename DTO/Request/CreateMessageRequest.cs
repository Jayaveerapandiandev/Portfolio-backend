namespace Portfolio_Api.DTO.Request
{
    public class CreateMessageRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Category { get; set; }  // Freelance Project, Hiring, Consultation
        public string Message { get; set; }
    }

}
