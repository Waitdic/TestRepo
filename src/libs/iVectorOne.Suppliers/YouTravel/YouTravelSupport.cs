namespace iVectorOne.Suppliers.YouTravel
{
    using System;
    using System.Collections.Generic;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;

    public class YouTravelSupport
    {

        public static List<string> GetDistinctDestinations(List<ResortSplit> oResorts)
        {

            var oReturn = new List<string>();
            string sDestination;

            foreach (ResortSplit oResort in oResorts)
            {

                sDestination = oResort.ResortCode.Split('_')[0];
                if (!oReturn.Contains(sDestination))
                {
                    oReturn.Add(sDestination);
                }

            }

            return oReturn;
        }

        public static string FormatDate(DateTime dDate)
        {
            return dDate.ToString("dd/MM/yyyy");
        }

    }
}