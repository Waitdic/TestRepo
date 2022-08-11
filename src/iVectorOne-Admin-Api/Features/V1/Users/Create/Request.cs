namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    public record Request : IRequest<Response>
    {
        public string UserName { get; set; } = "";

        public string Subject { get; set; } = "";
    }
}
