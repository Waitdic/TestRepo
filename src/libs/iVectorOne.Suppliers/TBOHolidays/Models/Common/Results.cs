namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    public class Results
    {
        public HotelResponse HotelResponse { get; set; } = new();

        public HotelCombinations HotelCombinations { get; set; } = new();
    }
}