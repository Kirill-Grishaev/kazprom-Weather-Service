using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace WeatherService
{
    public partial class Service1 : ServiceBase
    {
        Start1 start = new Start1();

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            start.GetStart();
        }

        protected override void OnStop()
        {
        }
    }
}
