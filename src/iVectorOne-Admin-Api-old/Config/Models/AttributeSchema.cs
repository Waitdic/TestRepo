namespace iVectorOne_Admin_Api.Config.Models
{
    public class AttributeSchema
    {
        public string? Name { get; set; }
        public string? Key { get; set; }
        public int? Order { get; set; }
        public bool? Required { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public List<DropDownOptionDTO> DropDownOptions { get; set; } = new List<DropDownOptionDTO>();
    }
}