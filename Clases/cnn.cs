using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLUNES
{
    public class cnn
    {
        public static string db = @"Data Source=RAPATUMADRE\SQLEXPRESS; Initial Catalog=BDPOSS; Integrated Security=true;";
        public static string ac = @"Provider=Microsoft.Jet.OleDb.4.0; Data Source=C:\ITLAS\DB\reportes.mdb;User Id=Admin;Password=;";

        public static string miSucursal;
        public static string miUsuario;
        public static string miNombre;
        public static string miCaja;
        public static string miCteContado = "00000001";

        public static string miRNC;
        public static string miRNCNombre;
        public static string miDireccion;
        public static string miTelefono;

        public static double miImpuesto = .18;
    }
}