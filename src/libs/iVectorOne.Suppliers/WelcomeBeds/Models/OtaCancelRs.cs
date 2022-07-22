namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class OtaCancelRs : SoapContent
    {
        [XmlAttribute("Success")]
        public string Success { get; set; } = "default";

        public bool IsSuccess => string.IsNullOrEmpty(Success);

        [XmlElement("CancelInfoRs")]
        public CancelInfoRs CancelInfoRs { get; set; } = new();
    }
}
