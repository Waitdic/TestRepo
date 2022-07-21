namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomStay
    {
        [XmlArray("AllocationVariants")]
        [XmlArrayItem("AllocationVariant")]
        public List<AllocationVariant> AllocationVariants { get; set; } = new();

        public int PerRoomRecordNumber { get; set; }
    }
}
