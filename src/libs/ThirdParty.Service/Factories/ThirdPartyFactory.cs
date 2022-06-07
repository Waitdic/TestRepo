namespace ThirdParty.Factories
{
    using System.Collections.Generic;
    using Intuitive;
    using ThirdParty;
    using ThirdParty.Search.Settings;
    using System.Linq;

    /// <summary>Factory that takes in a source or supplier and returns the correct third party search or booking class</summary>
    public class ThirdPartyFactory : IThirdPartyFactory
    {
        private readonly Dictionary<string, IThirdPartySearch> _propertySearchServices;
        private readonly Dictionary<string, IThirdParty> _propertyBookServices;

        public ThirdPartyFactory(
            IEnumerable<IThirdPartySearch> propertySearchServices,
            IEnumerable<IThirdParty> propertyBookServices)
        {
            _propertySearchServices = Ensure.IsNotNull(propertySearchServices, nameof(propertySearchServices))
                .ToDictionary(x => x.Source, x => x);
            _propertyBookServices = Ensure.IsNotNull(propertyBookServices, nameof(propertyBookServices))
                .ToDictionary(x => x.Source, x => x);
        }

        /// <inheritdoc />
        public IThirdParty CreateFromSource(string source, ThirdPartyConfiguration config)
        {
            _propertyBookServices.TryGetValue(source, out var supplierBook);
            return supplierBook;
        }
        
        /// <inheritdoc />
        public IThirdPartySearch CreateSearchTPFromSupplier(string source)
        {
            _propertySearchServices.TryGetValue(source, out var supplierSearch);
            return supplierSearch;
        }
    }
}