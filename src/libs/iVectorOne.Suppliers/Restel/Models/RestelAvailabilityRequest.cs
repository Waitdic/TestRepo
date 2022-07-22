namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [XmlRoot("peticion")]
    public class RestelAvailabilityRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "110";

        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros : IXmlSerializable
        {
            public string Hotel { get; set; } = string.Empty;

            public string Pais { get; set; } = string.Empty;

            public string Provincia { get; set; } = string.Empty;

            public string Poblacion { get; set; } = string.Empty;

            public int Categoria { get; set; }

            public int Radio { get; set; }

            public string Fechaentrada { get; set; } = string.Empty;

            public string Fechasalida { get; set; } = string.Empty;

            public string Afiliacion { get; set; } = "RS";

            public string Usuario { get; set; } = string.Empty;

            public List<int> Numhab { get; set; } = new();

            public List<string> Paxes { get; set; } = new();

            public int Restricciones { get; set; }

            public int Idioma { get; set; }

            public int Duplicidad { get; set; } = 1;

            public int Comprimido { get; set; } = 2;

            // custom serialization because there are fields numhab1 numhab2 numhab3 etc.
            public void WriteXml(XmlWriter writer)
            {
                writer.WriteElementString("hotel", Hotel);
                writer.WriteElementString("pais", Pais);
                writer.WriteElementString("provincia", Provincia);
                writer.WriteElementString("poblacion", Poblacion);
                writer.WriteElementString("categoria", Categoria.ToString());
                writer.WriteElementString("radio", Radio.ToString());
                writer.WriteElementString("fechaentrada", Fechaentrada);
                writer.WriteElementString("fechasalida", Fechasalida);
                writer.WriteElementString("afiliacion", Afiliacion);
                writer.WriteElementString("usuario", Usuario);

                for (int i = 1; i <= Numhab.Count; i++)
                {
                    writer.WriteElementString($"numhab{i}", Numhab[i - 1].ToString());
                    writer.WriteElementString($"paxes{i}", Paxes[i - 1]);
                }

                writer.WriteElementString("restricciones", Restricciones.ToString());
                writer.WriteElementString("idioma", Idioma.ToString());
                writer.WriteElementString("duplicidad", Duplicidad.ToString());
                writer.WriteElementString("comprimido", Comprimido.ToString());
            }

            public void ReadXml(XmlReader reader)
            {
                throw new NotSupportedException();
            }

            public XmlSchema GetSchema()
            {
                throw new NotSupportedException();
            }
        }
    }
}
