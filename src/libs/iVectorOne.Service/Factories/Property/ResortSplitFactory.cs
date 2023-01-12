namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using iVector.Search.Property;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;

    /// <summary>
    /// A factory that creates resort splits from a data row
    /// </summary>
    /// <seealso cref="ThirdParty.Service.Repositories.IResortSplitFactory" />
    public class ResortSplitFactory : IResortSplitFactory
    {
        /// <summary>
        /// Creates the specified data table.
        /// </summary>
        /// <param name="properties">The data table.</param>
        /// <returns>A list of resort splits</returns>
        public List<SupplierResortSplit> Create(List<CentralProperty> properties)
        {
            var supplierSplits = new List<SupplierResortSplit>();

            foreach (var property in properties)
            {
                var supplierSplit = this.GetOrCreateSupplierResortSplit(supplierSplits, property.Source);
                var resortSplit = this.GetOrCreateResortSplit(property.Source, property.Code, supplierSplit);

                var hotel = new Hotel()
                {
                    MasterID = 0,
                    TPKey = property.TPKey,
                    PropertyReferenceID = property.CentralPropertyID,
                    PropertyID = property.PropertyID,
                    PropertyName = property.Name
                };

                if (!resortSplit.Hotels.Any(h => h.TPKey == hotel.TPKey))
                {
                    resortSplit.Hotels.Add(hotel);
                }
            }

            return supplierSplits;
        }

        /// <summary>Checks if a matching supplier resort split exists, if not it makes one and adds it to the list</summary>
        /// <param name="supplierSplits">The current list of resort splits</param>
        /// <param name="source">The source used for filtering the list</param>
        /// <returns>The matching, or newly created supplier resort split.</returns>
        private SupplierResortSplit GetOrCreateSupplierResortSplit(List<SupplierResortSplit> supplierSplits, string source)
        {
            var supplierSplit = supplierSplits.FirstOrDefault(ss => ss.Supplier == source);
            if (supplierSplit == null)
            {
                supplierSplit = new SupplierResortSplit()
                {
                    Supplier = source
                };
                supplierSplits.Add(supplierSplit);
            }

            return supplierSplit;
        }

        /// <summary>Checks if there is already a resort split for the provided resort code, if there is not one then create it, 
        /// add it to the supplier resort split and return it</summary>
        /// <param name="source">The source.</param>
        /// <param name="resortCode">The resort code used for find the correct resort split</param>
        /// <param name="supplierSplit">The supplier split.</param>
        /// <returns>A newly created or matching resort split.</returns>
        private ResortSplit GetOrCreateResortSplit(string source, string resortCode, SupplierResortSplit supplierSplit)
        {
            var resortSplit = supplierSplit.ResortSplits.FirstOrDefault(rs => rs.ResortCode == resortCode);
            if (resortSplit == null)
            {
                resortSplit = new ResortSplit()
                {
                    ThirdPartySupplier = source,
                    ResortCode = resortCode
                };
                supplierSplit.ResortSplits.Add(resortSplit);
            }

            return resortSplit;
        }
    }
}
