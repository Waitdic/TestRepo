namespace iVectorOne.Models
{
    /// <summary>The de-duplicating strategy for property results</summary>
    public enum DedupeMethod
    {
        /// <summary> If the dedupe method speficied in the request is not valid</summary>
        unknown = 0,

        /// <summary> Selects all property results from the supplier and does not dedupe</summary>
        none = 1,

        /// <summary> Selects property results from the supplier with the cheapest room cost for that property</summary>
        cheapestleadin = 2
    }
}
