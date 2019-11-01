using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Weather_service
{
    class Program
    {


        static void Main(string[] args)
        {
            Start start = new Start();
            start.GetStart();
            
            Console.ReadKey();

        }
    }
}
