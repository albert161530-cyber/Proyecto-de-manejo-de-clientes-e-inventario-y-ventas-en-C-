using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PLUNES
{
    public class XMLFactura
    {
        public static string GenerarXML(string ncf, string cliente, double total)
        {
            var xml = new XElement("FacturaElectronica",
                new XElement("NCF", ncf),
                new XElement("Cliente", cliente),
                new XElement("Total", total),
                new XElement("Fecha", DateTime.Now)
            );

            return xml.ToString();
        }
    }
}
