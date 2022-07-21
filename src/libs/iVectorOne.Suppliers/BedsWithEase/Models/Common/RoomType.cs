namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomType
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public NumberOfRoomsAvailable NumberOfRoomsAvailable { get; set; } = new();

        [XmlArray("RatesWithBoard")]
        [XmlArrayItem("RateWithBoard")]
        public List<RateWithBoard> RatesWithBoard { get; set; } = new();

    }
}
