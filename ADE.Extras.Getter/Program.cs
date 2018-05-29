using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADE.Extras.Getter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(@"D:\padron_reducido_ruc.txt"))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    string BDCon = $"Server=192.168.90.8;Database=SUNAT;User Id=SA;Password=C0rporaci0n;Connection Timeout=150;pooling=false;";
                    int i = 1;
                    using (SqlConnection con = new SqlConnection(BDCon))
                    {
                        con.Open();
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] c = line.Split('|');

                            using (SqlCommand cmd = new SqlCommand("[dbo].[InsertaContribuyentes]", con))
                            {
                                cmd.Parameters.Clear();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@RUC", c[0]);
                                cmd.Parameters.AddWithValue("@NOMBRE", c[1]);
                                cmd.Parameters.AddWithValue("@ESTADO", c[2]);
                                cmd.Parameters.AddWithValue("@CONDICION", c[3]);
                                cmd.Parameters.AddWithValue("@UBIGEO", c[4]);
                                cmd.Parameters.AddWithValue("@TIPO_VIA", c[5]);
                                cmd.Parameters.AddWithValue("@NOMBRE_VIA", c[6]);
                                cmd.Parameters.AddWithValue("@COD_ZONA", c[7]);
                                cmd.Parameters.AddWithValue("@TIP_ZONA", c[8]);
                                cmd.Parameters.AddWithValue("@NUMERO", c[9]);
                                cmd.Parameters.AddWithValue("@INTERIOR", c[10]);
                                cmd.Parameters.AddWithValue("@LOTE", c[11]);
                                cmd.Parameters.AddWithValue("@DEPARTAMENTO", c[12]);
                                cmd.Parameters.AddWithValue("@MANZANA", c[13]);

                                cmd.ExecuteNonQuery();
                                
                            }
                            Console.WriteLine("linea " + i + " insertada");
                            i++;
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }
    }
}
