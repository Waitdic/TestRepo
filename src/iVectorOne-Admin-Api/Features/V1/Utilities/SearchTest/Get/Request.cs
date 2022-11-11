using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest.Get
{
    public record Request : IRequest<ResponseBase>
    {
        public int AccountID { get; set; }

        public string RequestKey { get; set; } = string.Empty;
    }
}
