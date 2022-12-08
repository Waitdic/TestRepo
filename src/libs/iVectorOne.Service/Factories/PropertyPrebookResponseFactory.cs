namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertyPrebook;
    using iVectorOne.Services;
    using Search = SDK.V2.PropertySearch;
    using iVectorOne.Utility;
    using System;

    /// <summary>
    /// A factory that creates property pre book responses using the provided property details
    /// </summary>
    /// <seealso cref="IPropertyPrebookResponseFactory" />
    public class PropertyPrebookResponseFactory : IPropertyPrebookResponseFactory
    {
        private readonly ITPSupport _support;

        /// <summary>The token service</summary>
        private readonly ITokenService _tokenService;

        /// <summary>Repository for retrieving third party meal basis</summary>
        private readonly IMealBasisLookupRepository _mealbasisRepository;

        /// <summary>Initializes a new instance of the <see cref="PropertyPrebookResponseFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens</param>
        /// <param name="mealbasisRepository">Repository for retrieving third party meal basis</param>
        /// <param name="support">The third party support interface</param>
        public PropertyPrebookResponseFactory(
            ITokenService tokenService,
            IMealBasisLookupRepository mealbasisRepository,
            ITPSupport support)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _mealbasisRepository = Ensure.IsNotNull(mealbasisRepository, nameof(mealbasisRepository));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        /// <summary>
        /// Creates a pre book response using information from the property details
        /// </summary>
        /// <param name="propertyDetails">The property details which contain all the information from the third party pre book</param>
        /// <returns>A pre book response</returns>
        public async Task<Response> CreateAsync(PropertyDetails propertyDetails)
        {
            var errata = new List<string>();
            var cancellationTerms = new List<Search.CancellationTerm>();
            var roomBookings = new List<RoomBooking>();
            int isoCurrencyId = !string.IsNullOrEmpty(propertyDetails.ISOCurrencyCode) ?
                await _support.ISOCurrencyIDLookupAsync(propertyDetails.ISOCurrencyCode) :
                0;

            foreach (var erratum in propertyDetails.Errata)
            {
                errata.Add(string.Join(": ", erratum.Title, erratum.Text));
            }

            foreach (var cancellation in propertyDetails.Cancellations)
            {
                var cancellationTerm = new Search.CancellationTerm()
                {
                    Amount = cancellation.Amount + 0.00M,
                    StartDate = cancellation.StartDate,
                    EndDate = cancellation.EndDate,
                };
                cancellationTerms.Add(cancellationTerm);
            }

            foreach (var room in propertyDetails.Rooms)
            {
                int mealbasisId = await _mealbasisRepository.GetMealBasisIDfromTPMealbasisCodeAsync(
                    propertyDetails.Source,
                    room.MealBasisCode,
                    propertyDetails.AccountID);

                var roomToken = new RoomToken()
                {
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                    ChildAges = room.ChildAges,
                    PropertyRoomBookingID = room.PropertyRoomBookingID,
                    LocalCost = room.LocalCost,
                    MealBasisID = mealbasisId,
                };

                var roomBooking = new RoomBooking()
                {
                    TotalCost = room.LocalCost + 0.00M,
                    CommissionPercentage = Math.Round(room.CommissionPercentage + 0.00M, 2),
                    RoomBookingToken = _tokenService.EncodeRoomToken(roomToken),
                    SupplierReference = room.ThirdPartyReference,
                    Supplier = propertyDetails.Source,
                };

                roomBookings.Add(roomBooking);
            }

            var bookingToken = new PropertyToken()
            {
                ArrivalDate = propertyDetails.ArrivalDate,
                Duration = propertyDetails.Duration,
                PropertyID = propertyDetails.PropertyID,
                Rooms = propertyDetails.Rooms.Count,
                ISOCurrencyID = isoCurrencyId,
            };

            var response = new Response()
            {
                BookingToken = _tokenService.EncodePropertyToken(bookingToken),
                TotalCost = propertyDetails.LocalCost + 0.00M,
                RoomBookings = roomBookings,
                CancellationTerms = cancellationTerms,
                Errata = errata,
                SupplierReference1 = propertyDetails.TPRef1,
                SupplierReference2 = propertyDetails.TPRef2,
            };

            return response;
        }
    }
}