namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    public class RoomDetail
    {
        public int Id { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }

        public int Infants { get; set; }

        public string ChildAgeCSV { get; set; } = string.Empty;
    }
}
