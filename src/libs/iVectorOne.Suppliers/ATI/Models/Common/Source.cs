namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class Source
    {
        [XmlAttribute("ERSP_UserID")]
        public string UserId { get; set; } = string.Empty;
    }
}
