using NetBase;
using NetBase.Communication;
using NetBase.FileProvider;
using NetBase.Runtime;
using NetBase.StaticRouting;
using SWDCore.Endpoints;
using SWDCore.Monitors;
using Monitor = SWDCore.Monitors.Monitor;
using SWDCore.Structures;
using SWDCore.Monitors.DataManagers;
using HttpMethod = NetBase.Communication.HttpMethod;

namespace SWDCore;

internal class Program
{
	public static Log log;
	private static List<DataEndpoint> endpoints;
	private static void Main(string[] args)
	{
		RunServer();
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
		Console.WriteLine("Loading configuration...");
		Console.WriteLine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));
		Config.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));
		Console.WriteLine("Initializing monitors...");
		List<Monitor> monitors = new List<Monitor>();
		List<MonitorConfig> configuration = Config.Current.Monitors.ToList();
		configuration.Sort((a, b) => a.ID.CompareTo(b.ID));
		Config.Current.Monitors = configuration.ToArray();
		foreach (MonitorConfig monitor in Config.Current.Monitors)
		{
			Console.WriteLine(monitor.Type);
			switch (monitor.Type)
			{
				case MonitorType.IP:
					Console.WriteLine("IP Monitor Created");
					monitors.Add(new IpMonitor(monitor.Data[0]));
					break;
				case MonitorType.GAS:
					monitors.Add(new TemperatureMonitor(monitor.Data[0], monitor.ID));
					monitors.Add(new MemoryMonitor(monitor.Data[0], monitor.ID));
					break;
				case MonitorType.HTTP:
					monitors.Add(new HttpMonitor(monitor.ID, monitor.Data[0]));
					break;
				default:
					Console.Error.WriteLine($"Unknown monitor type {monitor.Type}");
					break;
			}
		}
		
		IFileLoader loader = new LocalFileLoader("Docs/");
		Server server = new Server();
		Router router = new Router();
		log = new Log(Config.Current.LogDir);
		router.Add(loader, "app.js");
		router.Add(loader, "details.js");
		router.Add(loader, "index.html", "");
		router.Add(loader, "index.html");
		router.Add(loader, "DetailView.html");
		router.Add(loader, "style.css");
		log.Write("Loading Data");
		ManagerInit(Config.Current.IpDB, Config.Current.HttpDB, Config.Current.TemperatureDB, Config.Current.MemoryDB);
		log.Write("Data Loaded");
		if (!Config.Current.IsReadOnlyMode) { Clock.Start(Config.Current.TestIntervalS, Config.Current.SaveIntervalS); }
		server.HandeRequest = router.OnRequest;
		router.HandeRequest = DataRecieved;
		server.Start($"http://{Config.Current.IPAddress}:{Config.Current.Port}/");
		while (true) { }
		
	}
	private async static void ManagerInit(string ipmgrPath, string httpmgrPath, string tempmgrPath, string memoryPath)
	{
		await Task.WhenAll(
			IpMonitorDataManager.Initialize(ipmgrPath),
			HttpMonitorDataManager.Initialize(httpmgrPath),
			TemperatureMonitorDataManager.Initialize(tempmgrPath),
			MemoryMonitorDataManager.Initialize(memoryPath)
		);
	}

	private static HttpResponse DataRecieved(HttpRequest request)
	{
		if (request.Method == HttpMethod.GET)
		{
			foreach (var endpoint in endpoints)
			{
				if (endpoint.EndpointUrl == request.Url)
				{
					return endpoint.ReturnData(request);
				}
			}

			return new HttpResponse(StatusCode.Not_Found);
		}
		return new HttpResponse(StatusCode.Method_Not_Allowed);
	} 
}