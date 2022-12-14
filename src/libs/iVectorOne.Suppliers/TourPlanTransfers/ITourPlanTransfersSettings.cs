namespace iVectorOne.Suppliers
{
    public interface ITourPlanTransfersSettings
    {
        public string URL { get; set; }
        public string AgentId { get; set; }
        public string Password { get; set; }
        public bool AllowCancellation { get; set; }
    }
}
