namespace iVectorOne.Suppliers.PremierInn.Models.Cancel
{
    using iVectorOne.Suppliers.PremierInn.Models.Common;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.PremierInn.Models.Soap;

    public class ConfirmationNumberValidationResponse : SoapContent
    {
        public CancelResponseParameters Parameters { get; set; } = new();
    }

    public class CancelResponseParameters : Parameters
    {
        [XmlAttribute]
        public string Route { get; set; } = string.Empty;

        public Session Session { get; set; } = new();

        public BookerDetails BookerDetails { get; set; } = new();

        public ErrorCode ErrorCode { get; set; } = new();
    }
}
