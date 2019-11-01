using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Json;

namespace Weather_service
{
    class GetWeather
    {

        private DataTable nodes;
        private DataTable nodes_int = new DataTable();
        private DataTable buffer;
        Logger log = new Logger();
        private Thread t;

        public GetWeather(DataTable nodes, DataTable buffer)
        {

            this.nodes = nodes;
            this.buffer = buffer;

            t = new Thread(new ThreadStart(Handler)) { IsBackground = true, Name = "GetWeather" };
            t.Start();

        }


        private void Handler()
        {
            while (true)
            {
                try
                {
                    nodes_int.Clear();
                    nodes_int.Columns.Clear();

                    lock (nodes)
                    {
                        nodes_int = nodes.Copy();
                    }

                    foreach (DataRow row in nodes_int.Rows)
                    {
                        var id = row["id"];

                        var coord = row["coordinates"].ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');


                        if (coord.Length == 2)
                        {
                            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={coord[0]}&lon={coord[1]}&units=metric&appid=0d9caa92918801b76a16c3a1fe65b43e";

                            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                            string response;

                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                response = reader.ReadToEnd();

                                byte[] byteArray = Encoding.Unicode.GetBytes(response);
                                MemoryStream stream = new MemoryStream(byteArray);
                                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObject));
                                RootObject profileTypes = (RootObject)serializer.ReadObject(stream);


                                var temp = profileTypes.main.temp;
                                var humidity = profileTypes.main.humidity;
                                var pressure = profileTypes.main.pressure;
                                var speed = profileTypes.wind.speed;
                                var deg = profileTypes.wind.deg;

                                DataRow r = buffer.NewRow();
                                r["nameOfTable"] = String.Format("vfd_node{0:D5}_ws", Convert.ToInt64(row["id"]));
                                r["created_at"] = DateTime.Now;
                                r["temperature"] = temp;
                                r["humidity"] = humidity;
                                r["pressure"] = pressure;
                                r["wind_speed"] = speed;
                                r["wind_degree"] = deg;
                                buffer.Rows.Add(r);

                            }
                        }

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

                //Thread.Sleep(3600000);
            }
        }

        [DataContract]
        public class Coord
        {
            [DataMember]
            public double lon { get; set; }
            [DataMember]
            public double lat { get; set; }
        }
        [DataContract]
        public class Weather
        {
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public string main { get; set; }
            [DataMember]
            public string description { get; set; }
            [DataMember]
            public string icon { get; set; }
        }
        [DataContract]
        public class Main
        {
            [DataMember]
            public double temp { get; set; }
            [DataMember]
            public int pressure { get; set; }
            [DataMember]
            public int humidity { get; set; }
            [DataMember]
            public double temp_min { get; set; }
            [DataMember]
            public double temp_max { get; set; }
        }
        [DataContract]
        public class Wind
        {
            [DataMember]
            public double speed { get; set; }
            [DataMember]
            public int deg { get; set; }
        }
        [DataContract]
        public class Clouds
        {
            [DataMember]
            public int all { get; set; }
        }
        [DataContract]
        public class Sys
        {
            [DataMember]
            public int type { get; set; }
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public double message { get; set; }
            [DataMember]
            public string country { get; set; }
            [DataMember]
            public int sunrise { get; set; }
            [DataMember]
            public int sunset { get; set; }
        }
        [DataContract]
        public class RootObject
        {
            [DataMember]
            public Coord coord { get; set; }
            [DataMember]
            public List<Weather> weather { get; set; }
            [DataMember]
            public string @base { get; set; }
            [DataMember]
            public Main main { get; set; }
            [DataMember]
            public Wind wind { get; set; }
            [DataMember]
            public Clouds clouds { get; set; }
            [DataMember]
            public int dt { get; set; }
            [DataMember]
            public Sys sys { get; set; }
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public int cod { get; set; }
        }


    }

}
