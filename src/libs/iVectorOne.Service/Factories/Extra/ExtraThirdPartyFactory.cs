
namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Extra;
    using System.Collections.Generic;
    using System.Linq;

    public class ExtraThirdPartyFactory : IExtraThirdPartyFactory
    {
        private readonly Dictionary<string, IThirdPartySearch> _extraSearchServices;
        private readonly Dictionary<string, IThirdParty> _extraBookServices;

        public ExtraThirdPartyFactory(
            IEnumerable<IThirdPartySearch> searchServices,
            IEnumerable<IThirdParty> bookServices)
        {
            _extraSearchServices = GetThirdPartyDictionary(searchServices);
            _extraBookServices = GetThirdPartyDictionary(bookServices);
        }
        private Dictionary<string, T> GetThirdPartyDictionary<T>(IEnumerable<T> services)
        {
            Ensure.IsNotNull(services, nameof(services));
            return services
                .Select(x => ((x as ISingleSource)!.Source, x))
                .ToDictionary(x => x.Item1.ToLower(), x => x.Item2);
        }
        public IThirdParty CreateFromSource(string source, ThirdPartyConfiguration config)
        {
            _extraBookServices.TryGetValue(source.ToLower(), out var supplierBook);
            return supplierBook;
        }

        public IThirdPartySearch CreateSearchTPFromSupplier(string source)
        {
            _extraSearchServices.TryGetValue(source.ToLower(), out var supplierSearch);
            return supplierSearch;
        }
    }
}
