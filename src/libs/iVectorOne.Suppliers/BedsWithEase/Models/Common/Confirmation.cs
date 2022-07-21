namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    public class Confirmation
    {
        public string Failed { get; set; } = string.Empty;

        public Confirmed Confirmed { get; set; } = new();
    }
}