namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class BookCodesToConfirm
    {
        [XmlElement("ReservedGroup")]
        public List<ReservedGroup> ReservedGroup { get; set; } = new();
    }
}