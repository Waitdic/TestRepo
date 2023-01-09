namespace iVectorOne.Services
{
    using iVectorOne.SDK.V2.LocationContent;
    using System.Threading.Tasks;

    public interface ILocationContentService
    {
        public Task<Response> GetAllLocations(Request locationRequest);
    }
}
