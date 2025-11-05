namespace Portfolio_Api.DTO.Response
{
    public class AboutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public object? Data { get; set; }
    }

}
