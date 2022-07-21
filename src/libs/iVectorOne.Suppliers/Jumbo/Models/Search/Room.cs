namespace ThirdParty.CSSuppliers.Jumbo.Models
{
    using System.Collections.Generic;

    public class Room
    {
        public int adults { get; set; }
        public int children { get; set; }
        public List<int> childrenAges { get; set; } = new();
        public int infants { get; set; }
        public string typeCode { get; set; }
        public string typeName { get; set; }
    }
}