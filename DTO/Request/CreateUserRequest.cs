namespace Portfolio_Api.DTO.Request
{
    public class CreateUserRequest
    {
        // required: userid, password, name
        public string UserId { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
