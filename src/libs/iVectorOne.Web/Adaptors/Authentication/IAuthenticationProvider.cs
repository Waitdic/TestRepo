namespace iVectorOne.Web.Adaptors.Authentication
{
    using System.Threading.Tasks;
    using iVectorOne.Models;

    public interface IAuthenticationProvider
    {
        Task<Subscription> Authenticate(string username, string password);
    }
}