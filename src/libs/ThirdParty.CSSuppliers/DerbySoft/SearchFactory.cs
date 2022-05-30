namespace ThirdPartyInterfaces.DerbySoft
{
    using System;
    using DerbySoftBookingUsbV4;
    using DerbySoftShoppingEngineV4;
    using global::ThirdParty;
    using global::ThirdParty.CSSuppliers;

    public class SearchFactory
    {
        private readonly IDerbySoftSettings _settings;

        private readonly ISearchRequestBuilder _shoppingEngineRequestBuilder;
        private readonly ISearchRequestBuilder _bookingUsbRequestBuilder;

        private readonly ISearchResponseTransformer _shoppingEngineResponseTransformer;
        private readonly ISearchResponseTransformer _bookingUsbResponseTransformer;

        public SearchFactory(IDerbySoftSettings settings, string source, Guid guid)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _shoppingEngineRequestBuilder = new ShoppingEngineRequestBuilder(_settings, source, guid);
            _bookingUsbRequestBuilder = new BookingUsbV4AvailabilityRequestBuilder(_settings, source, guid);

            _shoppingEngineResponseTransformer = new ShoppingEngineResponseTransformer(_settings);
            _bookingUsbResponseTransformer = new BookingUsbV4ResponseTransformer(_settings);
        }

        public ISearchRequestBuilder GetSearchRequestBuilder(
            IThirdPartyAttributeSearch thirdPartyAttributeSearch)
        {
            return _settings.UseShoppingEngineForSearch(thirdPartyAttributeSearch)
                ? _shoppingEngineRequestBuilder
                : _bookingUsbRequestBuilder;
        }

        public ISearchResponseTransformer GetSearchResponseTransformer(
            IThirdPartyAttributeSearch thirdPartyAttributeSearch)
        {
            return _settings.UseShoppingEngineForSearch(thirdPartyAttributeSearch)
                ? _shoppingEngineResponseTransformer
                : _bookingUsbResponseTransformer;
        }
    }
}
