namespace ThirdParty.CSSuppliers.ChannelManager.Models.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public partial class ReturnStatus
    {
        public bool Success = true;
        [XmlArrayItem("Error")]
        public List<string> Errors = new List<string>();
        [XmlArrayItem("Warning")]
        public List<string> Warnings = new List<string>();

        public ReturnStatus()
        {
        }

        public ReturnStatus(string exception)
        {
            if (!string.IsNullOrEmpty(exception))
            {
                Errors.Add(exception);
            }
            Success = string.IsNullOrEmpty(exception);
        }

        public ReturnStatus(List<string> exceptions)
        {
            foreach (string exception in exceptions)
            {
                if (!string.IsNullOrEmpty(exception))
                {
                    Errors.Add(exception);
                }
            }
            Success = Errors.Count == 0;
        }
    }
}