namespace iVectorOne.Factories
{
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Models;

    /// <summary>
    /// Defines a factory that takes in a third party transfer search request and creations a location map
    /// </summary>
    public interface ITransferLocationMappingFactory
    {
        /// <summary>Creates the transfer location mapping.</summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="account">The account</param>
        /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <returns>A transfer location map</returns>
        LocationMapping Create(TransferSearchDetails searchDetails, Account account);
    }
}