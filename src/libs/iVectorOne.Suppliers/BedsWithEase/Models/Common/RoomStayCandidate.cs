namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomStayCandidate
    {
        [XmlArray("GuestCounts")]
        [XmlArrayItem("PerRoom")]
        public List<PerRoom> GuestCounts { get; set; } = new();
    }
}
