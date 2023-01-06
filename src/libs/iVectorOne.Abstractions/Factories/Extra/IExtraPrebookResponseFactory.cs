
namespace iVectorOne.Factories
{
    using iVectorOne.Models.Extra;
    using iVectorOne.SDK.V2.ExtraPrebook;
    using System.Threading.Tasks;

    /// <summary>A factory that creates extra pre book responses using the provided extra details</summary>
    public interface IExtraPrebookResponseFactory
    {
        /// <summary>Creates a pre book response using information from the extra details</summary>
        /// <param name="extraDetails">The extra details which contain all the information from the third party pre book</param>
        /// <returns>A pre book response</returns>
        Task<Response> CreateAsync(ExtraDetails extraDetails);
    }
}
