namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public record ResponseModel : ResponseModelBase
    {
        public List<PropertyDto> Properties { get; set; } = new List<PropertyDto>();
    }
}
