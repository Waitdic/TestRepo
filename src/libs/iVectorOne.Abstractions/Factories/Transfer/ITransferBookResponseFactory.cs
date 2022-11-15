namespace iVectorOne.Factories
{
    using iVectorOne.Models.Transfer;
    using iVectorOne.SDK.V2.TransferBook;

    /// <summary>A factory that creates transfer book responses using the provided transfer details</summary>
    public interface ITransferBookResponseFactory
    {
        /// <summary>
        /// Creates a book response using information from the transfer details
        /// </summary>
        /// <param name="transferDetails">The transfer details which contain all the information from the third party pre book</param>
        /// <returns>A book response</returns>
        Response Create(TransferDetails transferDetails);
    }
}