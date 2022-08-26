namespace iVectorOne.Web.Infrastructure.Security
{
    using System.Security.Principal;
    using iVectorOne.Models;

    public class AuthenticationIdentity : GenericIdentity
    {
        public AuthenticationIdentity(Account account) : base(account.Login, "Basic")
        {
            Account = account;
        }

        public Account Account { get; set; }
    }
}