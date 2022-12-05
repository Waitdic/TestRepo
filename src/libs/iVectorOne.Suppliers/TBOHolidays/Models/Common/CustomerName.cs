namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    public class CustomerName
    {
        public string Title { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public GuestType Type { get; set; }
    }
}
