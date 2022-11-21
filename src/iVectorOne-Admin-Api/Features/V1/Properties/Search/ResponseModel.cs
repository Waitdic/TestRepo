namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public record ResponseModel : ResponseModelBase
    {
        public List<PropertyDto> Properties { get; set; } = new List<PropertyDto>();
    }

    #region DTO

    public record PropertyDto
    {
        public int PropertyId { get; set; }

        public string Name { get; set; } = string.Empty;

    }

    #endregion
}
