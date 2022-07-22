namespace iVectorOne.CSSuppliers.iVectorConnect.Models.Common
{
    public class LoginDetails
    {
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string AgentReference { get; set; } = string.Empty;

        public int SellingCurrencyID { get; set; }
    }
}
