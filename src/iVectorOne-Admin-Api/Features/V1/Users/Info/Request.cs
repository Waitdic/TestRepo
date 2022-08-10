namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    public class Request : IRequest<Response>
    {
        public string Key { get; set; }
    }
}
