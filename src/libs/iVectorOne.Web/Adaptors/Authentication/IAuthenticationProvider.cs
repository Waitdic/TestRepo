namespace IVectorOne.Web.Adaptors.Authentication
{
    using System.Threading.Tasks;
    using ThirdParty.Models;

    public interface IAuthenticationProvider
    {
        Task<User> Authenticate(string username, string password);
    }
}