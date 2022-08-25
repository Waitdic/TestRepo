namespace iVectorOne_Admin_Api.Features.V1.Users.List
{
    public record ResponseModel : ResponseModelBase
    {
        public List<UserDto> Users { get; set; } = new();
    }
}