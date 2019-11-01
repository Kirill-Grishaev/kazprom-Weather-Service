using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_service
{
    public class Start
    {
        DataTable nodes = new DataTable();
        DataTable buffer = new DataTable();

        DBReader db_reader;
        DBWriter db_writer;
        GetWeather get_weather;

        Logger log = new Logger();

        public void GetStart()
        {
            try
            {

                db_reader = new DBReader(nodes);
                db_writer = new DBWriter(buffer);
                get_weather = new GetWeather(nodes, buffer);

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
