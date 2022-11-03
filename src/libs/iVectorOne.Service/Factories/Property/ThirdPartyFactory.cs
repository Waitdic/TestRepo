namespace iVectorOne.Factories
{
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;

    /// <summary>Factory that takes in a source or supplier and returns the correct third party search or booking class</summary>
    public class ThirdPartyFactory : IThirdPartyFactory
    {
        private readonly Dictionary<string, IThirdPartySearch> _propertySearchServices;
        private readonly Dictionary<string, IThirdParty> _propertyBookServices;

        public ThirdPartyFactory(
            IEnumerable<IThirdPartySearch> propertySearchServices,
            IEnumerable<IThirdParty> propertyBookServices)
        {
            _propertySearchServices = GetThirdPartyDictionary(propertySearchServices);
            _propertyBookServices = GetThirdPartyDictionary(propertyBookServices);
        }

        private Dictionary<string, T> GetThirdPartyDictionary<T>(IEnumerable<T> servces)
        {
            Ensure.IsNotNull(servces, nameof(servces));
            return servces
                .Where(x => x is ISingleSource)
                .Select(x => ((x as ISingleSource)!.Source, x))
                .Concat(servces
                    .Where(x => x is IMultiSource)
                    .SelectMany(x => (x as IMultiSource)!.Sources.Select(s => (s, x))))
                .ToDictionary(x => x.Item1, x => x.Item2);
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