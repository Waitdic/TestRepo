using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public record Request : IRequest<ResponseBase>
    {
        public int AccountID { get; set; }

       public string? Query { get; set; } 
    }
}
