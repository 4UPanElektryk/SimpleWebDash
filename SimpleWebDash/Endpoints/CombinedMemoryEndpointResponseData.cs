using System.Collections.Generic;

namespace SimpleWebDash.Endpoints
{
	public struct CombinedMemoryEndpointResponseData
	{
		public Dictionary<string, MemoryEndpointResponseData> MemoryData { get; set; }
	}
}
