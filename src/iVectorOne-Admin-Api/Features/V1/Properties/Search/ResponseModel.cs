namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public record ResponseModel : ResponseModelBase
    {
        public List<Property> Properties { get; set; } = new List<Property>();
    }
}
