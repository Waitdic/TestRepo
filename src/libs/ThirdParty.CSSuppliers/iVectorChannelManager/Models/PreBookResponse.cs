namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    using System.Collections.Generic;

    public class PreBookResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();

        public int PropertyReferenceID { get; set; }
        public int CurrencyID { get; set; }
        public string ChannelManager { get; set; }
        public decimal NetCost { get; set; }
        public decimal Commission { get; set; }
        public List<SearchResponse.Room> Rooms { get; set; } = new();
    }
}