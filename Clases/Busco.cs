using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLUNES
{
    public class Busco
    {
        public static string BuscarPuestoDeTrabajo(string _posicion, out bool found)
        {
            found = false;

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open(); // abre la base de datos

                string query = @"SELECT NOMBREDEPOSICION FROM POSICIONES WHERE IDPOSICION = @A1";

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _posicion);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    found = true;

                    return dr["NOMBREDEPOSICION"].ToString();
                }
            }

            return null;
        }

        public static string BuscarSucursal(string _sucursal, out bool found)
        {
            found = false;

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open(); // abre la base de datos

                string query = @"SELECT NOMBREDESUCURSAL FROM SUCURSALES WHERE IDSUCURSAL = @A1";

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _sucursal);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    found = true;

                    return dr["NOMBREDESUCURSAL"].ToString();
                }
            }

            return null;
        }

        public static string ClienteAlContado(string articulo)
        {

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open(); // abre la base de datos

                string query = @"SELECT * FROM INFOCIA WHERE IDINFOCIA = @A1";

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", articulo);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    cnn.miRNC = dr["RNC"].ToString();
                    cnn.miDireccion = dr["Direccion"].ToString();
                    cnn.miTelefono = dr["Telefono"].ToString();
                    cnn.miRNCNombre = dr["CIANombre"].ToString();
                    cnn.miCteContado = dr["ClienteContado"].ToString();

                    return dr["ClienteContado"].ToString();
                }
            }

            return null;
        }
        // --- AGREGA ESTO DEBAJO DE BUSCAULTIMONUMERO ---

        public static void BuscarSuplidor(ref string id, ref string nombre)
        {
            frmVENSUP consulta = new frmVENSUP();
            consulta.ShowDialog();
            if (consulta.EData)
            {
                id = consulta.var1;
                nombre = consulta.var2;
            }
        }

        public static void BuscarCategoria(ref string id, ref string nombre)
        {
            frmVENCAT consulta = new frmVENCAT();
            consulta.ShowDialog();
            if (consulta.EData)
            {
                id = consulta.var1;
                nombre = consulta.var2;
            }
        }

        public static void BuscarSubCategoria(ref string id, ref string nombre, string categoriaPadre = "")
        {
            // El "string categoriaPadre = """ hace que el tercer argumento sea opcional
            frmVENSUBCAT consulta = new frmVENSUBCAT(categoriaPadre);
            consulta.ShowDialog();
            if (consulta.EData)
            {
                id = consulta.var1;
                nombre = consulta.var2;
            }
        }
        public static string BuscaUltimoNumero(string _nmID)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open(); // abre la base de datos
                string _Query = "SELECT CONTADOR + 1 AS ULTIMO FROM SECUENCIAS WHERE ID = @A1";

                SqlCommand cmd = new SqlCommand(_Query, cnx);  // envia el script al servidor
                cmd.Parameters.AddWithValue("@A1", _nmID);
                SqlDataReader rdr = cmd.ExecuteReader();  // ejecuta el query en el servidor

                if (rdr.Read()) // pregunta si el contenedor trajo informacion
                {
                    return rdr["ULTIMO"].ToString();
                }
            }

            return null;
        }
    }
}