using SimpleWebDash.Monitors.Configuration;

namespace SimpleWebDash.Endpoints
{
	public struct SafeMonitorConfig
	{
		public string ID { get; set; }
		public string FriendlyName { get; set; }
		public MonitorType Type { get; set; }
	}
}
