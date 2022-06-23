namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class User
    {
        public User()
        {
            UserTenants = new HashSet<UserTenant>();
        }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string Key { get; set; } = null!;

        public virtual ICollection<UserTenant> UserTenants { get; set; }
    }
}
