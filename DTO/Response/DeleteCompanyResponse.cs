namespace Portfolio_Api.DTO.Response
{
    public class DeleteCompanyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? DeletedCompanyId { get; set; }
    }
}
