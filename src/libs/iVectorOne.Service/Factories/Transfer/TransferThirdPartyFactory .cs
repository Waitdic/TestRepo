namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Factory that takes in a source or supplier and returns the correct third party search or booking class</summary>
    public class TransferThirdPartyFactory : ITransferThirdPartyFactory
    {
        private readonly Dictionary<string, IThirdPartyTransferSearch> _transferSearchServices;
        //private readonly Dictionary<string, IThirdParty> _propertyBookServices;

        public TransferThirdPartyFactory(
            IEnumerable<IThirdPartyTransferSearch> transferSearchServices)
        {
            _transferSearchServices = GetThirdPartyDictionary(transferSearchServices);
            //_propertyBookServices = GetThirdPartyDictionary(propertyBookServices);
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
        //public IThirdParty CreateFromSource(string source, ThirdPartyConfiguration config)
        //{
        //    _propertyBookServices.TryGetValue(source, out var supplierBook);
        //    return supplierBook;
        //}

        /// <inheritdoc />
        public IThirdPartyTransferSearch CreateSearchTPFromSupplier(string source)
        {
            _transferSearchServices.TryGetValue(source.ToLower(), out var supplierSearch);
            return supplierSearch;
        }
    }
}
