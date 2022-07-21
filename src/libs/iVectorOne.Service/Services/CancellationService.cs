namespace iVectorOne.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using iVectorOne.Factories;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Repositories;
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

        /// <inheritdoc/>
        public async Task<Cancel.Response> CancelAsync(Cancel.Request cancelRequest)
        {
            Cancel.Response response = null!;
            var exceptionString = string.Empty;
            bool success = true;
            var propertyDetails = new PropertyDetails();

            try
            {
                propertyDetails = await this.propertyDetailsFactory.CreateAsync(cancelRequest);
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
                    var thirdParty = this.thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        cancelRequest.User.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var thirdPartyReponse = await thirdParty.CancelBookingAsync(propertyDetails);
                        success = thirdPartyReponse?.Success ?? false;

                        if (success)
                        {
                            response = this.responseFactory.Create(thirdPartyReponse!);
                        }
                        else
                        {
                            exceptionString = "Suppplier cancellation failed";
                            response = new Cancel.Response()
                            {
                                Warnings = new System.Collections.Generic.List<string>() { exceptionString }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionString = ex.ToString();
            }
            finally
            {
                if (!success && propertyDetails.Warnings.Any())
                {
                    exceptionString += string.Join(Environment.NewLine, propertyDetails.Warnings);
                }

                await this.logRepository.LogCancelAsync(cancelRequest, response!, cancelRequest.User, exceptionString);
            }

            return response!;
        }

        /// <inheritdoc/>
        public async Task<Precancel.Response> GetCancellationFeesAsync(Precancel.Request cancelRequest)
        {
            Precancel.Response response = null!;
            var exceptionString = string.Empty;
            bool success = true;
            var propertyDetails = new PropertyDetails();

            try
            {
                propertyDetails = await this.propertyDetailsFactory.CreateAsync(cancelRequest);
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
                    var thirdParty = this.thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        cancelRequest.User.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var cancellationFees = await thirdParty.GetCancellationCostAsync(propertyDetails);
                        success = !propertyDetails.Warnings.Any();

                        if (success)
                        {
                            response = await this.responseFactory.CreateFeesResponseAsync(cancellationFees, propertyDetails);
                        }
                        else
                        {
                            exceptionString = "Suppplier pre-cancellation failed";
                            response = new Precancel.Response()
                            {
                                Warnings = new System.Collections.Generic.List<string>() { exceptionString }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionString = ex.ToString();
            }
            finally
            {
                if (!success && propertyDetails.Warnings.Any())
                {
                    exceptionString += string.Join(Environment.NewLine, propertyDetails.Warnings);
                }

                // todo - log precancel
            }

            return response!;
        }
    }
}