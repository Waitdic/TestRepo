namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Collections.Generic;

    public class Results
    {
        public List<HotelResponse> HotelResponses { get; set; } = new();

        public HotelCombinations HotelCombinations { get; set; } = new();
    }
}