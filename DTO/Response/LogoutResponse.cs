namespace Portfolio_Api.DTO.Response
{
    public class LogoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public DateTime? SessionEndedAt { get; set; }
    }
}
