namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class Deadline
    {
        [XmlAttribute("AbsoluteDeadline")]
        public string AbsoluteDeadline { get; set; } = string.Empty;
    }
}
