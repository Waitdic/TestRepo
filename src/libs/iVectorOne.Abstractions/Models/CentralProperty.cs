namespace ThirdParty.Models
{
    public class CentralProperty
    {
        public int CentralPropertyID { get; set; }
        public int PropertyID { get; set; }
        public string Source { get; set; } = string.Empty;
        public string TPKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}