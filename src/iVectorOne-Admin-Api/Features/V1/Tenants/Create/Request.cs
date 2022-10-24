using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Create
{
    public record Request : IRequest<ResponseBase>
    {
        public string UserKey { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string ContactName { get; set; } = "";

        public string ContactTelephone { get; set; } = "";

        public string ContactEmail { get; set; } = "";
    }
}
