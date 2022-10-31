namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class PackageEstimateResponse : SoapContent
    {
        [XmlElement("PACKAGEESTIMATEResult")]
        public PackageEstimateResult PackageEstimateResult { get; set; } = new();
    }
}
