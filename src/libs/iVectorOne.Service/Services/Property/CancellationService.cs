namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models;
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
        private readonly IAPILogRepository _logRepository;

        /// <summary>Factory responsible for creating the correct Third party class</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        /// <summary>Builds the cancellation response</summary>
        private readonly ICancelPropertyResponseFactory _responseFactory;

        /// <summary>The reference validator</summary>
        private readonly ISuppierReferenceValidator _referenceValidator;

        /// <summary>The supplier log repository</summary>
        private readonly ISupplierLogRepository _supplierLogRepository;

        /// <summary>The supplier log repository</summary>
        private readonly IBookingRepository _bookingRepository;

        /// <summary>Initializes a new instance of the <see cref="CancellationService" /> class.</summary>
        /// <param name="propertyDetailsFactory">Factory used for building a property details from the request</param>
        /// <param name="logRepository">Repository responsible for logging</param>
        /// <param name="thirdPartyFactory">Factory responsible for creating the correct Third party class.</param>
        /// <param name="responseFactory">Factory responsible for creating the response.</param>
        /// <param name="referenceValidator">Validates if the right supplier references have been sent for the supplier</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        /// <param name="bookingRepository">Repository for saving the booking to the database</param>
        public CancellationService(
            IPropertyDetailsFactory propertyDetailsFactory,
            IAPILogRepository logRepository,
            IThirdPartyFactory thirdPartyFactory,
            ICancelPropertyResponseFactory responseFactory,
            ISuppierReferenceValidator referenceValidator,
            ISupplierLogRepository supplierLogRepository,
            IBookingRepository bookingRepository)
        {
            _propertyDetailsFactory = Ensure.IsNotNull(propertyDetailsFactory, nameof(propertyDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _referenceValidator = Ensure.IsNotNull(referenceValidator, nameof(referenceValidator));
            _supplierLogRepository = Ensure.IsNotNull(supplierLogRepository, nameof(supplierLogRepository));
            _bookingRepository = Ensure.IsNotNull(bookingRepository, nameof(bookingRepository));
        }

        /// <inheritdoc/>
        public async Task<Cancel.Response> CancelAsync(Cancel.Request cancelRequest)
        {
            Cancel.Response response = null!;
            bool requestValid = true;
            bool success = false;
            var propertyDetails = new PropertyDetails();

            try
            {
                var booking = await _bookingRepository.GetBookingAsync(cancelRequest.SupplierBookingReference, cancelRequest.Account);

                if (booking is null || booking.Status != BookingStatus.Live)
                {
                    propertyDetails.Warnings.AddNew("Unknown Booking", "No Booking could be found for the given supplier booking reference or the booking was not live.");
                }
                else
                {
                    propertyDetails = await _propertyDetailsFactory.CreateAsync(cancelRequest);
                    _referenceValidator.ValidateCancel(propertyDetails);
                }

                if (propertyDetails.Warnings.Any())
                {
                    response = new Cancel.Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    requestValid = false;
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
                            booking!.Status = BookingStatus.Cancelled;
                            await _bookingRepository.StoreBookingAsync(booking);
                        }
                        else
                        {
                            response = new Cancel.Response()
                            {
                                Warnings = propertyDetails.Warnings.Select(w => $"{w.Title}, {w.Text}").ToList()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Warnings.Add(ex.ToString());
            }
            finally
            {
                await _logRepository.LogCancelAsync(cancelRequest, response!, success);
                await _supplierLogRepository.LogBookRequestsAsync(propertyDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Supplier cancellation failed" };
                }
                else if (requestValid)
                {
                    response.Warnings = null!;
                }
            }

            return response!;
        }

        /// <inheritdoc/>
        public async Task<Precancel.Response> GetCancellationFeesAsync(Precancel.Request preCancelRequest)
        {
            Precancel.Response response = null!;
            bool requestValid = true;
            bool success = false;
            var propertyDetails = new PropertyDetails();

            try
            {
                propertyDetails = await _propertyDetailsFactory.CreateAsync(preCancelRequest);
                _referenceValidator.ValidateCancel(propertyDetails);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Precancel.Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    requestValid = false;
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        preCancelRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

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
                            response = new Precancel.Response()
                            {
                                Warnings = propertyDetails.Warnings.Select(w => $"{w.Title}, {w.Text}").ToList()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Warnings.Add(ex.ToString());
            }
            finally
            {
                await _logRepository.LogPrecancelAsync(preCancelRequest, response!, success);
                await _supplierLogRepository.LogBookRequestsAsync(propertyDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Supplier pre-cancellation failed" };
                }
                else if (requestValid)
                {
                    response.Warnings = null!;
                }
            }

            return response!;
        }
    }
}