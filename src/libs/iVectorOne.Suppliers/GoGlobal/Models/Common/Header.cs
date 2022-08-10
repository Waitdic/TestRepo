namespace iVectorOne.Suppliers.GoGlobal.Models
{
    public class Header
    {
        public string Agency { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string OperationType { get; set; } = string.Empty;
        public Statistics Statistics { get; set; } = new();
        public bool ShouldSerializeStatistics() => !string.IsNullOrEmpty(Statistics?.ResultsQty);

    }
}
