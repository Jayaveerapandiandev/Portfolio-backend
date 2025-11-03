using Portfolio_Api.DTO.Data;

namespace Portfolio_Api.DTO.Response
{
    public class AdminHomeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AdminHomeData? Data { get; set; }
    }
}
