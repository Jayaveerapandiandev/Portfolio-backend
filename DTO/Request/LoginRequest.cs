namespace Portfolio_Api.DTO.Request
{
  
        public class LoginRequest
        {
            public string UserId { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty; // AES-encrypted
            public string? IpAddress { get; set; }                // optional
            public string? UserAgent { get; set; }                // optional
        }
  

}
