namespace iVectorOne_Admin_Api.Features.V1.Users.List
{
    public record ResponseModel : ResponseModelBase
    {
        public List<UserDto> Users { get; set; } = new();
    }

    #region DTO

    public record UserDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;
    }

    #endregion
}