namespace SWDCore.Structures.Endpoint;

public struct CombinedTempertatureResponse
{
	public Dictionary<string, string> Nodes { get; set; }
	public Dictionary<string, TemperatureResponse> Temperatures { get; set; }
}
