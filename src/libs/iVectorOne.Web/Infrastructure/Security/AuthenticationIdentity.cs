namespace iVectorOne.Web.Infrastructure.Security
{
    using System.Security.Principal;
    using iVectorOne.Models;

    public class AuthenticationIdentity : GenericIdentity
    {
        public AuthenticationIdentity(Subscription user) : base(user.Login, "Basic")
        {
            User = user;
        }

        public Subscription User { get; set; }
    }
}