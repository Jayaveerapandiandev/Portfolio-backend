namespace Portfolio_Api.DTO.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; } // 🔹 Add JWT Token
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public string? username { get; set; }
        public DateTime? SessionStartedAt { get; set; }
    }
}
