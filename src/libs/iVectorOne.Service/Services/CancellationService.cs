namespace iVectorOne.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Repositories;
    using Cancel = SDK.V2.PropertyCancel;
    using Precancel = SDK.V2.PropertyPrecancel;

    /// <summary>Cancellation service responsible for cancelling third party bookings.</summary>
    public class CancellationService : ICancellationService
    {
        /// <summary>Factory used for building a property details from the request</summary>
        private readonly IPropertyDetailsFactory _propertyDetailsFactory;

        /// <summary>Repository responsible for logging</summary>
        private readonly IBookingLogRepository _logRepository;

        /// <summary>Factory responsible for creating the correct Third party class</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        /// <summary>Builds the cancellation response</summary>
        private readonly ICancelPropertyResponseFactory _responseFactory;

        /// <summary>The reference validator</summary>
        private readonly ISuppierReferenceValidator _referenceValidator;

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
            _propertyDetailsFactory = Ensure.IsNotNull(propertyDetailsFactory, nameof(propertyDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _referenceValidator = Ensure.IsNotNull(referenceValidator, nameof(referenceValidator));
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
                propertyDetails = await _propertyDetailsFactory.CreateAsync(cancelRequest);
                _referenceValidator.ValidateCancel(propertyDetails);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Cancel.Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        cancelRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var thirdPartyReponse = await thirdParty.CancelBookingAsync(propertyDetails);
                        success = thirdPartyReponse?.Success ?? false;

                        if (success)
                        {
                            response = _responseFactory.Create(thirdPartyReponse!);
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

                await _logRepository.LogCancelAsync(cancelRequest, response!, cancelRequest.Account, exceptionString);
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
                propertyDetails = await _propertyDetailsFactory.CreateAsync(cancelRequest);
                _referenceValidator.ValidateCancel(propertyDetails);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Precancel.Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        cancelRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var cancellationFees = await thirdParty.GetCancellationCostAsync(propertyDetails);
                        success = !propertyDetails.Warnings.Any();

                        if (success)
                        {
                            response = await _responseFactory.CreateFeesResponseAsync(cancellationFees, propertyDetails);
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