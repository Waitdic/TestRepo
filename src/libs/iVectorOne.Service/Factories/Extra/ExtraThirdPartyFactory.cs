
namespace iVectorOne.Factories.Extra
{
    using Intuitive;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ExtraThirdPartyFactory : IExtraThirdPartyFactory
    {
        private readonly Dictionary<string, iVectorOne.Extra.IThirdPartySearch> _extraSearchServices;
        private readonly Dictionary<string, IThirdParty> _extraBookServices;

        public ExtraThirdPartyFactory(
            IEnumerable<iVectorOne.Extra.IThirdPartySearch> searchServices,
            IEnumerable<IThirdParty> bookServices)
        {
            _extraSearchServices = GetThirdPartyDictionary(searchServices);
            //_extraBookServices = GetThirdPartyDictionary(bookServices);
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

        public iVectorOne.Extra.IThirdPartySearch CreateSearchTPFromSupplier(string source)
        {
            _extraSearchServices.TryGetValue(source.ToLower(), out var supplierSearch);
            return supplierSearch;
        }
    }
}
