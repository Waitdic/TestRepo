namespace iVectorOne.Suppliers.GowaySydneyTransfers
{
    using iVectorOne.Constants;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class InjectedGowaySydneyTransfersSettings : InjectedTourPlanTransfersSettings
    {
        protected override string Source => ThirdParties.GOWAYSYDNEYTRANSFERS;
    }
}
