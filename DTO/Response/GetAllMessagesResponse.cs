using Portfolio_Api.DTO.Data;

namespace Portfolio_Api.DTO.Response
{
    public class GetAllMessagesResponse
    {
        public bool Success { get; set; }
        public List<MessageData> Data { get; set; } = new List<MessageData>();
        public string? Message { get; set; }  // optional error message
    }

}
