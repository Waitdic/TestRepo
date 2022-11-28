namespace iVectorOne.Suppliers.GowaySydneyTransfers
{
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using System;

    public class GowaySydneyTransfersSearch : TourPlanTransfersSearchBase
    {
        public override string Source => ThirdParties.GOWAYSYDNEYTRANSFERS;
    }
}
