namespace iVectorOne_Admin_Api.Data
{
    public partial class AccountSupplier
    {
        public int AccountSupplierId { get; set; }
        public int AccountId { get; set; }
        public short SupplierId { get; set; }
        public bool Enabled { get; set; }

        public Account Account { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
    }
}