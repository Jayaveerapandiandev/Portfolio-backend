namespace Portfolio_Api.DTO.Data
{
    public class MessageData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSeen { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
