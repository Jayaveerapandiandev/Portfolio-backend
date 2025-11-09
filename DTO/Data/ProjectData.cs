namespace Portfolio_Api.DTO.Data
{
    public class ProjectData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<string> Technologies { get; set; }
        public string GithubUrl { get; set; }
        public string LiveUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool Featured { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
