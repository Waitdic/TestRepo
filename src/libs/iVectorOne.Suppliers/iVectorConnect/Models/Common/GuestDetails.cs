namespace iVectorOne.CSSuppliers.iVectorConnect.Models.Common
{
    using iVectorOne.Models;

    public class GuestDetail
    {
        public int GuestID { get; set; }

        public PassengerType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int Age { get; set; }

        public string DateOfBirth { get; set; } = string.Empty;
    }
}
