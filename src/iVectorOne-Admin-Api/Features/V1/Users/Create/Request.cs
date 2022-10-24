using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    public record Request : IRequest<ResponseBase>
    {
        public string UserName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;
    }
}