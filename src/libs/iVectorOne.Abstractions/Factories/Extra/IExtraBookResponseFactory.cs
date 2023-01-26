namespace iVectorOne.Factories
{
    using iVectorOne.Models.Extra;
    using iVectorOne.SDK.V2.ExtraBook;

    /// <summary>A factory that creates extra book responses using the provided extra details</summary>
    public interface IExtraBookResponseFactory
    {
        /// <summary>
        /// Creates a book response using information from the extra details
        /// </summary>
        /// <param name="extraDetails">The extra details which contain all the information from the third party book</param>
        /// <returns>A book response</returns>
        Response Create(ExtraDetails extraDetails);
    }
}