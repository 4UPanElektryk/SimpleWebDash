using NetBase.Runtime;
using SWDCore.Endpoints;
using SWDCore.Structures;

namespace SWDCore;

internal class Program
{
	public static Log log;
	private static List<DataEndpoint> endpoints;
	private static void Main(string[] args)
	{
		Console.WriteLine("Hello, World!");
	}
	private static void RunServer()
	{
		endpoints = new List<DataEndpoint>
		{
			new IpE("api/ipstatus"),
			new IpDeteailsE("api/ipdetails"),
			new TemperatureE("api/tempstats"),
			new CombinedTempertatureE("api/fulltempstats"),
			new CombinedMemoryE("api/fullmemstats"),
			new HttpE("api/httpstatus"),
			new ConfigurationE("api/configuration"),
		};
		Config.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));

	}
}