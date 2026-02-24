using System.Collections.Generic;

namespace SimpleWebDash.Endpoints
{
	public struct CombinedTempertatureResponse
	{
		public Dictionary<string, string> Nodes { get; set; }
		public Dictionary<string, TemperatureResponse> Temperatures { get; set; }
	}
}
