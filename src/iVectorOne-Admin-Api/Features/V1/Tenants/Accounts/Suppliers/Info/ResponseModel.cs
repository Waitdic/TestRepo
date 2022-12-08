using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Info
{
    public record ResponseModel : ResponseModelBase
    {
        public int SupplierID { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public List<ConfigurationDTO> Configurations { get; set; } = new();
    }

    #region DTO

    public enum ConfigurationType
    {
        String,
        Boolean,
        Uri,
        Email,
        Password,
        Dropdown,
        Number
    }

    public class DropDownOptionDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ConfigurationDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int AccountSupplierAttributeID { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SupplierAttributeID { get; set; }

        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Order { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ConfigurationType Type { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DefaultValue { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Minimum { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Maximum { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinLength { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxLength { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Required { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DropDownOptionDTO>? DropDownOptions { get; set; }
    }

    #endregion
}