﻿namespace ThirdParty.Search.Results.Enums
{
    /// <summary>The de-duplicating strategy for property results</summary>
    public enum DedupeMethod
    {
        /// <summary> Selects property results from the supplier with the cheapest room cost for that property</summary>
        CheapestLeadin = 0,

        /// <summary>Selects property results for each meal basis from the supplier with the cheapest room cost for that property and meal basis</summary>
        CheapestMealBasis = 1
    }
}