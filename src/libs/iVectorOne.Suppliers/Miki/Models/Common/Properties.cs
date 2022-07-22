namespace iVectorOne.CSSuppliers.Miki.Models.Common
{
    using System.Collections.Generic;

    public class Properties
    {
        public List<Property> Property { get; set; } = new();

        public string Currency { get; set; } = string.Empty;

        public List<RoomDetail> Rooms { get; set; } = new();
    }
}
