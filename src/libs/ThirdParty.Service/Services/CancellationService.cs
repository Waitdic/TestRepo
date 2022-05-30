namespace ThirdParty.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ThirdParty;
    using ThirdParty.Factories;
    using ThirdParty.Models;
    using ThirdParty.Repositories;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Cancel = SDK.V2.PropertyCancel;

    /// <summary>Cancellation service responsible for cancelling third party bookings.</summary>
    public class CancellationService : ICancellationService
    {
        /// <summary>Factory used for building a property details from the request</summary>
        private readonly IPropertyDetailsFactory propertyDetailsFactory;

        /// <summary>Repository responsible for logging</summary>
        private readonly IBookingLogRepository logRepository;

        /// <summary>Factory responsible for creating the correct Third party class</summary>
        private readonly IThirdPartyFactory thirdPartyFactory;

        /// <summary>Builds the cancellation response</summary>
        private readonly ICancelPropertyResponseFactory responseFactory;

        /// <summary>The reference validator</summary>
        private readonly ISuppierReferenceValidator referenceValidator;

        /// <summary>Initializes a new instance of the <see cref="CancellationService" /> class.</summary>
        /// <param name="propertyDetailsFactory">Factory used for building a property details from the request</param>
        /// <param name="logRepository">Repository responsible for logging</param>
        /// <param name="thirdPartyFactory">Factory responsible for creating the correct Third party class.</param>
        /// <param name="responseFactory">Factory responsible for creating the response.</param>
        /// <param name="referenceValidator">Validates if the right supplier references have been sent for the supplier</param>
        public CancellationService(
            IPropertyDetailsFactory propertyDetailsFactory,
            IBookingLogRepository logRepository,
            IThirdPartyFactory thirdPartyFactory,
            ICancelPropertyResponseFactory responseFactory,
            ISuppierReferenceValidator referenceValidator)
        {
            this.propertyDetailsFactory = propertyDetailsFactory;
            this.logRepository = logRepository;
            this.thirdPartyFactory = thirdPartyFactory;
            this.responseFactory = responseFactory;
            this.referenceValidator = referenceValidator;
        }

        /// <summary>Cancels the specified property booking</summary>
        /// <param name="cancelRequest">The cancel request.</param>
        /// <param name="user">The user.</param>
        /// <returns>A cancellation response</returns>
        public async Task<Cancel.Response> CancelAsync(Cancel.Request cancelRequest, User user)
        {
            Cancel.Response response = null!;
            var exceptionString = string.Empty;

            try
            {
                var propertyDetails = await this.propertyDetailsFactory.CreateAsync(cancelRequest, user);
                this.referenceValidator.ValidateCancel(propertyDetails);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Cancel.Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };
                }
                else
                {
                    IThirdParty thirdParty = this.thirdPartyFactory.CreateFromSource(
                    propertyDetails.Source,
                    user.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var thirdPartyReponse = thirdParty.CancelBooking(propertyDetails);
                        response = this.responseFactory.Create(thirdPartyReponse);
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionString = ex.ToString();
            }
            finally
            {
                this.logRepository.LogCancel(cancelRequest, response!, user, exceptionString);
            }

            return response!;
        }

        /// <summary>Cancels the specified property booking</summary>
        /// <param name="cancelRequest">The cancel request.</param>
        /// <param name="user">The user.</param>
        /// <returns>A cancellation response</returns>
        public async Task<Precancel.Response> GetCancellationFeesAsync(Precancel.Request cancelRequest, User user)
        {
            Precancel.Response response = null!;
            var exceptionString = string.Empty;

            try
            {
                var propertyDetails = await this.propertyDetailsFactory.CreateAsync(cancelRequest, user);
                this.referenceValidator.ValidateCancel(propertyDetails);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Precancel.Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };
                }
                else
                {
                    IThirdParty thirdParty = this.thirdPartyFactory.CreateFromSource(
                    propertyDetails.Source,
                    user.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var cancellationFees = thirdParty.GetCancellationCost(propertyDetails);

                        if (!propertyDetails.Warnings.Any())
                        {
                            response = await this.responseFactory.CreateFeesResponseAsync(cancellationFees, propertyDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionString = ex.ToString();
            }

            return response!;
        }
    }
}
