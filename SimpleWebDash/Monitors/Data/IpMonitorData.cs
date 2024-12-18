using System;

namespace SimpleWebDash.Monitors.Data
{
	public struct IpMonitorData
	{
		public string IP;
		public DateTime Time;
		public bool Success;
		public long ResponseTime;
	}
}
