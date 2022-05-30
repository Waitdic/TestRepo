namespace ThirdParty.Services
{
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using Cancel = SDK.V2.PropertyCancel;
    using Precancel = SDK.V2.PropertyPrecancel;

    /// <summary>Defines the service that handles the cancellation</summary>
    public interface ICancellationService
    {
        /// <summary>Processes the cancellation request and attempts to cancel with the third party</summary>
        /// <param name="cancelRequest">The cancel request.</param>
        /// <param name="user">The user.</param>
        /// <returns>The cancellation response</returns>
        Task<Cancel.Response> CancelAsync(Cancel.Request cancelRequest, User user);

        /// <summary>Processes the cancellation request and attempts to cancel with the third party</summary>
        /// <param name="cancelRequest">The cancel request.</param>
        /// <param name="user">The user.</param>
        /// <returns>The cancellation response</returns>
        Task<Precancel.Response> GetCancellationFeesAsync(Precancel.Request cancelRequest, User user);
    }
}