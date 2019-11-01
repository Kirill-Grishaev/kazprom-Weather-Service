using System;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace WeatherService
{
    class DBWriter
    {
        DataTable buffer;
        Logger log = new Logger();
        private Thread t;

        public DBWriter(DataTable in_buffer)
        {
            buffer = in_buffer;

            buffer.Columns.Add("nameOfTable");
            buffer.Columns.Add("created_at");
            buffer.Columns.Add("temperature");
            buffer.Columns.Add("humidity");
            buffer.Columns.Add("pressure");
            buffer.Columns.Add("wind_speed");
            buffer.Columns.Add("wind_degree");


            t = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "DBWriter" };
            t.Start();
        }

        private void Handler()
        {

            while (true)
            {

                try
                {
                    DataTable buffer_int;

                    lock (buffer)
                    {
                        buffer_int = buffer.Copy();
                        buffer.Clear();
                    }



                    foreach (DataRow row in buffer_int.Rows)
                    {

                        string table_name = "[" + row["nameOfTable"].ToString() + "] ";
                        string str_columns = "[" + buffer_int.Columns[1].ColumnName + "],";
                        string str_values = "'" + Convert.ToDateTime(row["created_at"]).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";


                        for (int i = 2; i < row.ItemArray.Length; i++)
                        {

                            if (!row.IsNull(i))
                            {
                                str_values += "'" + row[i].ToString().Replace(",", ".") + "',";
                                str_columns += "[" + buffer_int.Columns[i].ColumnName.ToString() + "],";
                            }
                        }

                        str_columns = str_columns.TrimEnd(',');
                        str_values = str_values.TrimEnd(',');

                        string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            if (connection == null || String.IsNullOrEmpty(connection.ConnectionString))
                            {
                                throw new Exception("Fatal error: missing connecting string in web.config file");
                            }
                            else
                            {

                                if (connection.State == ConnectionState.Open)
                                    connection.Close();

                                connection.Open();

                                string sqlExp = string.Format($"INSERT INTO {table_name} ({str_columns}) VALUES ({str_values})");
                                Debug.WriteLine(sqlExp);
                                SqlCommand cmd = new SqlCommand(sqlExp, connection);
                                cmd.ExecuteNonQuery();

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    lock (log)
                    {
                        log.Message(ex);
                    }
                }

                Thread.Sleep(10);

            }
        }
    }
}


