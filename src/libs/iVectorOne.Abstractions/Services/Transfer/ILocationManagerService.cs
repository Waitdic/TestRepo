using iVectorOne.Search.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Services.Transfer
{
    public interface ILocationManagerService
    {
        /// <summary>
        /// Check the locations exist or not 
        /// </summary>
        /// <param name="uniqueLocationList"></param>
        /// <param name="searchDetails"></param>
        void CheckLocations(List<string> uniqueLocationList, TransferSearchDetails searchDetails);
    }
}
