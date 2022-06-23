using MediatR;
namespace iVectorOne_Admin_Api.Config.Requests
{
    public class UserRequest : IRequest<UserResponse>
    {
        public UserRequest(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}
