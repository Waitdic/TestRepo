namespace iVectorOne.Services
{
    using System.Threading.Tasks;
    using Cancel = SDK.V2.PropertyCancel;
    using Precancel = SDK.V2.PropertyPrecancel;

    /// <summary>Defines the service that handles the cancellation</summary>
    public interface ICancellationService
    {
        /// <summary>Processes the cancellation request and attempts to cancel with the third party</summary>
        /// <param name="cancelRequest">The cancel request.</param>
        /// <returns>The cancellation response</returns>
        Task<Cancel.Response> CancelAsync(Cancel.Request cancelRequest);

        /// <summary>Processes the cancellation request and attempts to cancel with the third party</summary>
        /// <param name="cancelRequest">The cancel request.</param>
        /// <returns>The cancellation response</returns>
        Task<Precancel.Response> GetCancellationFeesAsync(Precancel.Request cancelRequest);
    }
}