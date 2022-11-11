namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class OtaCancelRs : SoapContent
    {
        [XmlElement("CancelInfoRS")]
        public CancelInfoRS CancelInfoRS { get; set; } = new();
    }
}
