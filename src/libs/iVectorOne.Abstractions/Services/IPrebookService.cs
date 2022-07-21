namespace ThirdParty.Services
{
    using System.Threading.Tasks;
    using ThirdParty.SDK.V2.PropertyPrebook;

    /// <summary>
    /// a service for pre booking components
    /// </summary>
    public interface IPrebookService
    {
        /// <summary>
        /// Pre books the specified pre book request.
        /// </summary>
        /// <param name="prebookRequest">The pre book request.</param>
        /// <returns>a pre book response for the property</returns>
        Task<Response> PrebookAsync(Request prebookRequest);
    }
}