﻿namespace ThirdParty.Repositories
{
    using System.Collections.Generic;
    using ThirdParty.Models;

    /// <summary>
    /// A factory that creates resort splits
    /// </summary>
    public interface IResortSplitFactory
    {
        /// <summary>
        /// Creates the specified data table.
        /// </summary>
        /// <param name="properties">The data table.</param>
        /// <returns>A list of resort splits</returns>
        List<SupplierResortSplit> Create(List<CentralProperty> properties);
    }
}