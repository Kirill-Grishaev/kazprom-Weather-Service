using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_service
{
    class Logger
    {
        private static Logger logger;
        public Logger() { }

        public static Logger GetInstance()
        {
            if (logger == null)
                logger = new Logger();
            return logger;
        }

        public void Message(string msg)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\log";
            string todayDate = DateTime.Now.ToString("yyyyMMdd");

            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }

            using (StreamWriter myStream = new StreamWriter(path + "\\" + todayDate + ".log", true))
            {
                myStream.WriteLine(msg);
            }
        }

        public void Message(Exception ex)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\log";
            string todayDate = DateTime.Now.ToString("yyyyMMdd");

            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }

            using (StreamWriter myStream = new StreamWriter(path + "\\" + todayDate + ".log", true))
            {
                myStream.WriteLine($"[{DateTime.Now}]");
                myStream.WriteLine(ex.Message);
                myStream.WriteLine(ex.StackTrace);
            }

        }


    }
}

