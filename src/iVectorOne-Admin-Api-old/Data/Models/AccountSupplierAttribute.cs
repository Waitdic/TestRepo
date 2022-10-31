namespace iVectorOne_Admin_Api.Data
{
    public partial class AccountSupplierAttribute
    {
        public int AccountSupplierAttributeId { get; set; }
        public int AccountId { get; set; }
        public int SupplierAttributeId { get; set; }
        public string Value { get; set; } = null!;

        public virtual Account Account { get; set; } = null!;
        public virtual SupplierAttribute SupplierAttribute { get; set; } = null!;
    }
}