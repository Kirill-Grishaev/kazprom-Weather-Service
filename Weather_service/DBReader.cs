using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;

namespace Weather_service
{
    class DBReader
    {
        private Thread t;
        private DataTable table;
        //private DataTable buffer;
        Logger log = new Logger();
        public DBReader(DataTable source)
        {
            table = source;
            t = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "DBReader" };
            t.Start();
        }

        private void Handler()
        {
            while (true)
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

                    string sqlExp = "SELECT id, coordinates FROM vfd_node_registry";

                    DataContext db = new DataContext(connectionString);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand(sqlExp, connection);
                        SqlDataReader reader = command.ExecuteReader();

                        lock (table)
                        {
                            table.Clear();
                            table.Columns.Clear();
                            table.Load(reader);
                        }
                        reader.Close();

                        connection.Close();
                    }
                    Thread.Sleep(60000);
                }
                catch (Exception ex)
                {
                    lock (log)
                    {
                        log.Message(ex);
                    }
                }

            }
        }

    }
}
