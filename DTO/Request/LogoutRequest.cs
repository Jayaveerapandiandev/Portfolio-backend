namespace Portfolio_Api.DTO.Request
{
    public class LogoutRequest
    {
        public string SessionId { get; set; } = string.Empty; // UUID from session table
    }
}
