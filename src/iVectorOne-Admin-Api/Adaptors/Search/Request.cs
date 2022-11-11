namespace iVectorOne_Admin_Api.Adaptors.Search
{
    public record Request
    {
        public Guid RequestKey { get; set; }

        public string Properties { get; set; } = string.Empty;

        public DateTime Searchdate { get; set; }

        public string RoomRequest { get; set; } = string.Empty;

        public string DedupeMethod { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
