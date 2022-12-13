namespace iVectorOne.Suppliers.PremierInn.Models.Cancel
{
    using iVectorOne.Suppliers.PremierInn.Models.Common;
    using System.Xml.Serialization;

    public class ConfirmationNumberValidationResponse
    {
        public CancelResponseParameters Parameters { get; set; } = new();
    }

    public class CancelResponseParameters : Parameters
    {
        [XmlAttribute]
        public string Route { get; set; } = string.Empty;

        public Session Session { get; set; } = new();

        public ErrorCode ErrorCode { get; set; } = new();
    }
}
