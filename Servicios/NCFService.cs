using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLUNES
{
    public class NCFService
    {
        public static string GenerarNCF(int sucursalID, string tipoID)
        {
            using (var conn = DB.GetConnection())
            {
                conn.Open(); // 

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmdSerie = new SqlCommand(
                            @"SELECT SerieID, UltimoNumero 
                                FROM NCFSeries 
                                WHERE SucursalID=@s AND TipoID=@t",
                            conn, tran);

                        cmdSerie.Parameters.AddWithValue("@s", sucursalID);
                        cmdSerie.Parameters.AddWithValue("@t", tipoID);

                        SqlDataReader reader = cmdSerie.ExecuteReader();

                        if (!reader.Read())
                            throw new Exception("No existe serie configurada.");

                        int serieID = Convert.ToInt32(reader["SerieID"]);
                        int ultimoNumero = Convert.ToInt32(reader["UltimoNumero"]);
                        reader.Close();

                        int nuevoNumero = ultimoNumero + 1;

                        string numeroNCF = tipoID + nuevoNumero.ToString("00000000");

                        SqlCommand cmdUpdate = new SqlCommand(
                            @"UPDATE NCFSeries 
                                SET UltimoNumero=@n 
                                WHERE SerieID = @id",
                            conn, tran);

                        cmdUpdate.Parameters.AddWithValue("@n", nuevoNumero);
                        cmdUpdate.Parameters.AddWithValue("@id", serieID);
                        cmdUpdate.ExecuteNonQuery();

                        tran.Commit();

                        return numeroNCF;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
