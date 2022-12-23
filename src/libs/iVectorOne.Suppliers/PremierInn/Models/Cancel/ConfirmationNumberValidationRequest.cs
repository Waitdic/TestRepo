namespace iVectorOne.Suppliers.PremierInn.Models.Cancel
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.PremierInn.Models.Common;
    using iVectorOne.Suppliers.PremierInn.Models.Soap;


    public class ConfirmationNumberValidationRequest : SoapContent
    {
        public Login Login { get; set; } = new();

        public CancelRequestParameters Parameters { get; set; } = new();
    }

    public class CancelRequestParameters : Parameters
    {
        [XmlAttribute]
        public string Route { get; set; } = string.Empty;

        public string ConfirmationNumber { get; set; } = string.Empty;

        public string ArrivalDate { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;
    }
}
