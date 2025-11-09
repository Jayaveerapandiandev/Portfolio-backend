using Portfolio_Api.DTO.Data;

namespace Portfolio_Api.DTO.Response
{

        public class ProjectResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public ProjectData Data { get; set; }
            public List<ProjectData> DataList { get; set; }
        }

 }
