﻿using SimpleWebDash.Monitors.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SimpleWebDash
{
	public class Clock
	{
		public static event EventHandler<ClockTickEventArgs> Tick;
		private static Timer timer;
		private static Timer AutoSaveTimer;
		public static void Start()
		{
			timer = new Timer();
			timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			timer.Interval = 10000; // ~ 10 seconds
			timer.Enabled = true;
			AutoSaveTimer = new Timer();
			AutoSaveTimer.Elapsed += new ElapsedEventHandler(AutoSaveEvent);
			AutoSaveTimer.Interval = 60000; // ~ 1 minute
			AutoSaveTimer.Enabled = true;
		}
		private static void AutoSaveEvent(object sender, ElapsedEventArgs e)
		{
			IpMonitorDataManager.Save();
			HttpMonitorDataManager.Save();
			TemperatureMonitorDataManager.Save();
		}
		public static void Stop() { 
			timer.Stop();
		}
		private static void OnTimedEvent(object sender, ElapsedEventArgs e) {
			if (Tick != null)
			{
				ClockTickEventArgs args = new ClockTickEventArgs();
				args.TickTime = DateTime.UtcNow;
				Tick.Invoke(null, args);
			}
		}
	}
	public class ClockTickEventArgs : EventArgs
	{
		public DateTime TickTime;
	}
}
