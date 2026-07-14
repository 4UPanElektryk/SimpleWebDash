using System;

namespace SimpleWebDash.Monitors.Data
{
	public struct HttpMonitorData
	{
		public string ID;
		public DateTime Time;
		public bool Success;
		public long ResponseTime;
	}
}
