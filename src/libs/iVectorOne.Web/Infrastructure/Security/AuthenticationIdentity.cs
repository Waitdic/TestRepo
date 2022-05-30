namespace IVectorOne.Web.Infrastructure.Security
{
    using System.Security.Principal;
    using ThirdParty.Models;

    public class AuthenticationIdentity : GenericIdentity
    {
        public AuthenticationIdentity(User user) : base(user.Login, "Basic")
        {
            User = user;
        }

        public User User { get; set; }
    }
}