using System.Collections.Generic;

namespace SimpleWebDash.Endpoints
{
	public struct CombinedMemoryResponse
	{
		public Dictionary<string, MemoryResponse> MemoryData { get; set; }
	}
}
