namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class GetBookingChargeResponse : SoapContent
    {
        [XmlElement("GETBOOKINGCHARGEResult")]
        public GetBookingChargeResult? GetBookingChargeResult { get; set; }
    }
}
