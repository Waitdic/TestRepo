namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
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

        public ReturnStatus(string sException)
        {
            if (!string.IsNullOrEmpty(sException))
            {
                Errors.Add(sException);
            }
            Success = string.IsNullOrEmpty(sException);
        }

        public ReturnStatus(ArrayList sExceptions)
        {
            foreach (string sException in sExceptions)
            {
                if (!string.IsNullOrEmpty(sException))
                {
                    Errors.Add(sException);
                }
            }
            Success = Errors.Count == 0;
        }

        public ReturnStatus(List<string> oExceptions) : this(new ArrayList(oExceptions))
        {
        }

    }
}