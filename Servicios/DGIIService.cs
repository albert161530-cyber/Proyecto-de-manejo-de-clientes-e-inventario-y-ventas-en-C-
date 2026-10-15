using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using DGIIWebService;  // aqui la api de DGII

namespace PLUNES
{
    public class DGIIService
    {
        public class DGIIRespuesta
        {
            public string TrackId { get; set; }
            public string Estado { get; set; }
            public string Mensaje { get; set; }
        }

        public static DGIIRespuesta Enviar(string xmlFirmado)
        {
            try
            {
                string trackIdSimulado = Guid.NewGuid().ToString();

                string carpeta = @"C:\FacturasEnviadas\";

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string ruta = carpeta + trackIdSimulado + ".xml";
                File.WriteAllText(ruta, xmlFirmado);

                return new DGIIRespuesta
                {
                    TrackId = trackIdSimulado,
                    Estado = "Aceptado",
                    Mensaje = "Documento recibido en modo LOCAL"
                };
            }
            catch (Exception ex)
            {
                return new DGIIRespuesta
                {
                    TrackId = null,
                    Estado = "Error",
                    Mensaje = ex.Message
                };
            }
        }

        //public static string EnviarFactura(string xmlFirmado)
        //{
        //    try
        //    {
        //        // Ejemplo estructural
        //        // Aqui vendra la API de la DGII

        //        //var cliente = new DGIIWebService.FacturacionElectronicaPortTypeClient();

        //        //var respuesta = cliente.RecepcionFactura(xmlFirmado);

        //        //return respuesta.Estado;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error: " + ex.Message;
        //    }
        //}


    }

    
}
