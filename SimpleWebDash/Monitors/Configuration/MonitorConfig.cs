using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleWebDash.Monitors.Configuration
{
	public struct MonitorConfig
	{
		public string ID { get; set; }
		public string FriendlyName { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public MonitorType Type { get; set; }
		public string[] Data { get; set; }
	}
}
