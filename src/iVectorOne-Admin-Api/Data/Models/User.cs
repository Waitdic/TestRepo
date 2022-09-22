namespace iVectorOne_Admin_Api.Data
{
    public partial class User
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string Key { get; set; } = null!;

        public virtual ICollection<UserTenant> UserTenants { get; set; } = new HashSet<UserTenant>();
    }
}