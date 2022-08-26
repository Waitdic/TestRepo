namespace iVectorOne_Admin_Api.Data.Models
{
    public partial class Authorisation
    {
        public int AuthorisationId { get; set; }

        public string User { get; set; } = string.Empty;

        public string Relationship { get; set; } = string.Empty;

        public string Object { get; set; } = string.Empty;
    }
}