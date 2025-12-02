using System.Collections.Generic;

namespace SimpleWebDash.Endpoints
{
	public struct CombinedTempertatureEndpointResponseData
	{
		public Dictionary<string, string> Nodes { get; set; }
		public Dictionary<string, TemperatureEndpointResponseData> Temperatures { get; set; }
	}
}
