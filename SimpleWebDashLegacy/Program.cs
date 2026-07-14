using NetBase;
using NetBase.Communication;
using NetBase.FileProvider;
using NetBase.Runtime;
using NetBase.StaticRouting;
using Newtonsoft.Json.Linq;
using SimpleWebDash.Endpoints;
using SimpleWebDash.Monitors;
using SimpleWebDash.Monitors.Configuration;
using SimpleWebDash.Monitors.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWebDash
{
	internal class Program
	{
		private static List<DataEndpoint> endpoints;
		public static Log log;
		public static MonitorConfig[] monitorConfigs;
		static void Main(string[] args)
		{
			Server server = new Server();
			Router router = new Router();
			log = new Log(null);
			log._prefix = "Program";
			// replace this mess with a config file if a config file is passed through the command line as an argument
			// could be a json file or an ini file
			string IPPath = null;
			string HTTPPath = null;
			string TEMPSPath = null;
			string MemoryPath = null;
			string IPAddres = null;
			int Port = 8080;
			bool IsReadOnlyNode = true;
			MonitorConfig[] monitorscfg = new MonitorConfig[] { };
			if (args.Length != 1)
			{
				Console.Error.WriteLine("Missing Configuration!");
				Console.Error.WriteLine("Please provide a configuration file as an argument");
				Environment.Exit(1);
			}
			else
			{
				try
				{
					if (!File.Exists(args[0]))
					{
						Console.Error.WriteLine("File not found: " + args[0]);
						Environment.Exit(1);
					}
					dynamic config = JObject.Parse(File.ReadAllText(args[0]));
					IPPath = config["IPMonitorDataPath"].Value;
					HTTPPath = config["HTTPMonitorDataPath"].Value;
					TEMPSPath = config["TEMPSMonitorDataPath"].Value;
					MemoryPath = config["MememoryMonitorDataPath"].Value;
					IPAddres = config["IPAddress"].Value;
					Port = (int)config["Port"].Value;
					IsReadOnlyNode = config["ReadOnlyNode"].Value;
					monitorscfg = config["Monitors"].ToObject<MonitorConfig[]>();

				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("Error parsing config file");
					Console.Error.WriteLine("Please make sure the config file is in the correct format");
					Console.Error.WriteLine("The config file should be a json file with the following format:");
					Console.Error.WriteLine("{");
					Console.Error.WriteLine("  \"IPMonitorDataPath\": \"ips.jsonl\",");
					Console.Error.WriteLine("  \"HTTPMonitorDataPath\": \"https.jsonl\",");
					Console.Error.WriteLine("  \"TEMPSMonitorDataPath\": \"temps.jsonl\",");
					Console.Error.WriteLine("  \"MememoryMonitorDataPath\": \"mems.jsonl\",");
					Console.Error.WriteLine("  \"IPAddress\": \"127.0.0.1\",");
					Console.Error.WriteLine("  \"Port\": 8080,");
					Console.Error.WriteLine("  \"ReadOnlyNode\": true,");
					Console.Error.WriteLine("  \"Monitors\": ");
					Console.Error.WriteLine("  [");
					Console.Error.WriteLine("    { \"ID\": \"localhost\", \"FriendlyName\": \"Server Working\" \"Type\": \"IP\", \"Data\": [\"127.0.0.1\"]}");
					Console.Error.WriteLine("  ]");
					Console.Error.WriteLine("}");
#if DEBUG
					throw ex;
#endif
					Environment.Exit(1);
				}
			}
			List<MonitorConfig> configs = monitorscfg.ToList();
			monitorscfg = configs.ToArray();
			configs.Sort((a, b) => a.ID.CompareTo(b.ID));
			List<Monitor> monitors = new List<Monitor>();
			for (int i = 0; i < monitorscfg.Length; i++)
			{
				switch (monitorscfg[i].Type)
				{
					case MonitorType.IP:
						monitors.Add(new IpMonitor(monitorscfg[i].Data[0]));
						break;
					case MonitorType.GAS:
						monitors.Add(new TemperatureMonitor(monitorscfg[i].Data[0], monitorscfg[i].ID));
						monitors.Add(new MemoryMonitor(monitorscfg[i].Data[0], monitorscfg[i].ID));
						break;
					case MonitorType.HTTP:
						monitors.Add(new HttpMonitor(monitorscfg[i].ID, monitorscfg[i].Data[0]));
						break;
					default:
						Console.Error.WriteLine("Unknown Monitor Type: " + monitorscfg[i].Type);
						break;
				}
			}
			monitorscfg = configs.ToArray();
			monitorConfigs = monitorscfg;
			endpoints = new List<DataEndpoint>{
				new IpE("api/ipstatus"),
				new IpDeteailsE("api/ipdetails"),
				new TemperatureE("api/tempstats"),
				new CombinedTempertatureE("api/fulltempstats"),
				new CombinedMemoryE("api/fullmemstats"),
				new HttpE("api/httpstatus"),
				new ConfigurationE("api/configuration"),
			};
			IFileLoader loader = new LocalFileLoader("Docs\\");
			router.Add(loader, "app.js");
			router.Add(loader, "details.js");
			router.Add(loader, "index.html", "");
			router.Add(loader, "index.html");
			router.Add(loader, "DetailView.html");
			router.Add(loader, "style.css");
			log.Write("Loading Data");
			ManagerInit(IPPath, HTTPPath, TEMPSPath, MemoryPath);
			log.Write("Data Loaded");
			if (!IsReadOnlyNode) { Clock.Start(); }
			server.HandeRequest = router.OnRequest;
			router.HandeRequest = DataRecieved;
			server.Start($"http://{IPAddres}:{Port}/");
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
		public static HttpResponse DataRecieved(HttpRequest request)
		{
			if (request.Method == HttpMethod.GET)
			{
				return GetRequestRecieved(request);
			}
			return new HttpResponse(StatusCode.Method_Not_Allowed);
		}
		public static HttpResponse GetRequestRecieved(HttpRequest request)
		{
			foreach (var item in endpoints)
			{
				if (item.EndpointUrl == request.Url)
				{
					return item.ReturnData(request);
				}
			}
			return new HttpResponse(StatusCode.Not_Found);
		}
	}
}
