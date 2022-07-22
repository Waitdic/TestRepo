namespace iVectorOne.CSSuppliers.DerbySoft
{
    using System;
    using DerbySoftBookingUsbV4;
    using DerbySoftShoppingEngineV4;
    using Intuitive;
    using iVectorOne;

    public class SearchFactory
    {
        private readonly IDerbySoftSettings _settings;
        private readonly string _source;

        private readonly ISearchRequestBuilder _shoppingEngineRequestBuilder;
        private readonly ISearchRequestBuilder _bookingUsbRequestBuilder;

        private readonly ISearchResponseTransformer _shoppingEngineResponseTransformer;
        private readonly ISearchResponseTransformer _bookingUsbResponseTransformer;

        public SearchFactory(IDerbySoftSettings settings, string source, Guid guid)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _source = source;

            _shoppingEngineRequestBuilder = new ShoppingEngineRequestBuilder(_settings, source, guid);
            _bookingUsbRequestBuilder = new BookingUsbV4AvailabilityRequestBuilder(_settings, source, guid);

            _shoppingEngineResponseTransformer = new ShoppingEngineResponseTransformer(_settings, source);
            _bookingUsbResponseTransformer = new BookingUsbV4ResponseTransformer(_settings, source);
        }

        public ISearchRequestBuilder GetSearchRequestBuilder(
            IThirdPartyAttributeSearch thirdPartyAttributeSearch)
        {
            return _settings.EnableUtilitySearch(thirdPartyAttributeSearch, _source)
                ? _shoppingEngineRequestBuilder
                : _bookingUsbRequestBuilder;
        }

        public ISearchResponseTransformer GetSearchResponseTransformer(
            IThirdPartyAttributeSearch thirdPartyAttributeSearch)
        {
            return _settings.EnableUtilitySearch(thirdPartyAttributeSearch, _source)
                ? _shoppingEngineResponseTransformer
                : _bookingUsbResponseTransformer;
        }
    }
}