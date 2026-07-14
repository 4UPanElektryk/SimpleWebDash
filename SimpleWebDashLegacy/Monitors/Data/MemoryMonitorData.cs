using System;

namespace SimpleWebDash.Monitors.Data
{
	public struct MemoryMonitorData
	{
		public DateTime Time;
		public ulong total_kb;
		public ulong free_kb;
		public string IP;
	}
}
