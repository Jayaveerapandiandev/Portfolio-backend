namespace Portfolio_Api.DTO.Response
{
    public class ChangePasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }

}
