using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewerDetail
{
    public record Request : IRequest<ResponseBase>
    {
        public int AccountID { get; set; }

        public int Key { get; set; }


    }
}
