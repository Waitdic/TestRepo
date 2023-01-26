namespace iVectorOne.Services.Transfer
{
    using System.Threading.Tasks;
    using iVectorOne.SDK.V2.TransferPrebook;

    /// <summary>
    /// a service for pre booking components
    /// </summary>
    public interface IPrebookService
    {
        /// <summary>
        /// Pre books the specified pre book request.
        /// </summary>
        /// <param name="prebookRequest">The pre book request.</param>
        /// <returns>a pre book response for the transfer</returns>
        Task<Response> PrebookAsync(Request prebookRequest);
    }
}