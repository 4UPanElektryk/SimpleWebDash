using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleWebDash.Monitors.Configuration
{
    public struct MonitorConfig
    {
		[JsonConverter(typeof(StringEnumConverter))]
		public string ID { get; set; }
		public string FriendlyName { get; set; }
		public MonitorType Type { get; set; }
		public string[] Data { get; set; }
	}
}
