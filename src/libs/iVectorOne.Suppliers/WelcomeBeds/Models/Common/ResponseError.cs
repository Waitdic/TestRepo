namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class ResponseError
    {
        public ResponseError() { }
        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
        [XmlAttribute("Tag")]
        public string Tag { get; set; } = string.Empty;
    }

}
