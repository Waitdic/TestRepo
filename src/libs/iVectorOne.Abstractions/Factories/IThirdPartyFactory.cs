﻿namespace iVectorOne.Factories
{
    using iVectorOne;
    using iVectorOne.Search.Settings;

    /// <summary>Factory that takes in a source or supplier and returns the correct third party search or booking class</summary>
    public interface IThirdPartyFactory
    {
        /// <summary>Creates a third party that matches the provided source</summary>
        /// <param name="source">The source.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// The third party to use for booking
        /// </returns>
        IThirdParty CreateFromSource(string source, ThirdPartyConfiguration config);

        /// <summary>Creates the search third party from the provided supplier.</summary>
        /// <param name="supplier">The supplier.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// The third party to use for search
        /// </returns>
        IThirdPartySearch CreateSearchTPFromSupplier(string supplier);
    }
}