namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Transfer;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Factory that takes in a source or supplier and returns the correct third party search or booking class</summary>
    public class TransferThirdPartyFactory : ITransferThirdPartyFactory
    {
        private readonly Dictionary<string, IThirdPartySearch> _transferSearchServices;
        private readonly Dictionary<string, IThirdParty> _transferBookServices;

        public TransferThirdPartyFactory(
            IEnumerable<IThirdPartySearch> searchServices,
            IEnumerable<IThirdParty> bookServices)
        {
            _transferSearchServices = GetThirdPartyDictionary(searchServices);
            _transferBookServices = GetThirdPartyDictionary(bookServices);
        }

        private Dictionary<string, T> GetThirdPartyDictionary<T>(IEnumerable<T> services)
        {
            Ensure.IsNotNull(services, nameof(services));
            return services
                //.Where(x => x is ISingleSource)
                .Select(x => ((x as ISingleSource)!.Source, x))
                //.Concat(servces
                //    .Where(x => x is IMultiSource)
                //    .SelectMany(x => (x as IMultiSource)!.Sources.Select(s => (s, x))))
                .ToDictionary(x => x.Item1.ToLower(), x => x.Item2);
        }

        /// <inheritdoc />
        public IThirdParty CreateFromSource(string source, ThirdPartyConfiguration config)
        {
            _transferBookServices.TryGetValue(source, out var supplierBook);
            return supplierBook;
        }

        /// <inheritdoc />
        public IThirdPartySearch CreateSearchTPFromSupplier(string source)
        {
            _transferSearchServices.TryGetValue(source.ToLower(), out var supplierSearch);
            return supplierSearch;
        }
    }
}
