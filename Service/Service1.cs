using Paparazzi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace service
{
	public partial class Service1 : ServiceBase
	{
		Investigator _investigator;

		public Service1()
		{
			InitializeComponent();
		}

		public void OnDebug()
		{
			OnStart(null);
		}

		protected override void OnStart(string[] args)
		{
			_investigator = new Investigator();
			// investigate every minute
			var timer = new Timer(60000);
			//var timer = new Timer(5000);
			timer.Elapsed += timer_Elapsed;
			timer.Start();
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			_investigator.Investigate();
		}

		protected override void OnStop()
		{
		}

		protected override void OnPause()
		{
			base.OnPause();
		}
	}
}
