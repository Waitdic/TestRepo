namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using iVectorOne.Models.Transfer;
    using iVectorOne.SDK.V2.TransferPrebook;

    /// <summary>A factory that creates transfer pre book responses using the provided transfer details</summary>
    public interface ITransferPrebookResponseFactory
    {
        /// <summary>Creates a pre book response using information from the transfer details</summary>
        /// <param name="transferDetails">The transfer details which contain all the information from the third party pre book</param>
        /// <returns>A pre book response</returns>
        Task<Response> CreateAsync(TransferDetails transferDetails);
    }
}