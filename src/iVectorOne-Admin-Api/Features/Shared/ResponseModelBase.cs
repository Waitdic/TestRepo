using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features
{
    public record ResponseModelBase: IResponseModel
    {
        public bool Success { get; set; }
    }
}