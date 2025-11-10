namespace Portfolio_Api.DTO.Response
{
    public class CompanyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CompanyItemDto>? Data { get; set; }
    }

    public class CompanyItemDto
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
