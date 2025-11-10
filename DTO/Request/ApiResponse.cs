namespace Portfolio_Api.DTO.Request
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

    }

    public class ApiResponseWithId : ApiResponse
    {
        public int Id { get; set; }
    }


}
