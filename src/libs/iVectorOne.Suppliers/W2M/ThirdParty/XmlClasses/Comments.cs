using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Comments")]
    public class Comments
    {
        public Comments(List<Comment> comment)
        {
            Comment = comment;
        }

        public Comments()
        {
        }

        [XmlElement(ElementName = "Comment")]
        public List<Comment> Comment { get; set; }
    }
}
