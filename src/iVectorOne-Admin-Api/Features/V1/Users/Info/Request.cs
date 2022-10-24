using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    public class Request : IRequest<ResponseBase>
    {
        public string Key { get; set; } = string.Empty;
    }
}