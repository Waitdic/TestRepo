namespace iVectorOne_Admin_Api.Features.V1.Tenants.List
{
    public record TenantDto
    {
        public int TenantId { get; set; }

        public string CompanyName { get; set; } = "";

        public string ContactName { get; set; } = "";

        public string ContactTelephone { get; set; } = "";

        public string ContactEmail { get; set; } = "";

        public string Status { get; set; } = "";

        public bool IsActive
        {
            get
            {
                return Status.ToLower() == "active";
            }
        }

        public bool IsDeleted
        {
            get
            {
                return Status.ToLower() == "deleted";
            }
        }
    }
}
