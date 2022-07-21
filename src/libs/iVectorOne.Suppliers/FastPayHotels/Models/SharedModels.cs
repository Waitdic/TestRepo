namespace iVectorOne.Suppliers.FastPayHotels.Models
{
    using System.Collections.Generic;

    public  class SharedModels
    {
        public class Occupancy
        {
            public int adults { get; set; }
            public int children { get; set; }
            public List<int> childrenAges { get; set; } = new List<int>();
        }
    }
}