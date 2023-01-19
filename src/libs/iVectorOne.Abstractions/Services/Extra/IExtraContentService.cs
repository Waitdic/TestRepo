namespace iVectorOne.Services
{
    using iVectorOne.SDK.V2.ExtraContent;
    using System.Threading.Tasks;

    /// <summary>
    /// A service for returning extras from the repository
    /// </summary>
    public interface IExtraContentService
    {
        public Task<Response> GetAllExtras(Request locationRequest);
    }
}
