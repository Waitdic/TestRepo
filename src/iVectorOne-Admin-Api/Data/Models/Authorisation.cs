namespace iVectorOne_Admin_Api.Data.Models
{
    public partial class Authorisation
    {
        public int AuthorisationId { get; set; }

        public string User { get; set; } = "";

        public string Relationship { get; set; } = "";

        public string Object { get; set; } = "";
    }
}
