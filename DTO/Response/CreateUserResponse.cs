namespace Portfolio_Api.DTO.Response
{
    public class CreateUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public DateTimeOffset? CreatedAt { get; set; }
        public string? UserId { get; set; }
    }

}
