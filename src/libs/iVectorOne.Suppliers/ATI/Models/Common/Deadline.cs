namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Deadline
    {
        [XmlAttribute("AbsoluteDeadline")]
        public DateTime AbsoluteDeadline { get; set; }
    }
}
