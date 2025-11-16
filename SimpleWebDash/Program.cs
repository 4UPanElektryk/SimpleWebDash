using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using NetBase;
using NetBase.Communication;
using NetBase.FileProvider;
using NetBase.StaticRouting;
using SimpleWebDash.Endpoints;
using SimpleWebDash.Monitors;
using SimpleWebDash.Monitors.Data;
using SimpleWebDash.Monitors.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using NetBase.Runtime;

namespace SimpleWebDash
{
	internal class Program
	{
		private static List<DataEndpoint> endpoints;
		static void Main(string[] args)
		{
			Server server = new Server();
			Router router = new Router();
			// replace this mess with a config file if a config file is passed through the command line as an argument
			// could be a json file or an ini file
			string IPPath = null;
			string HTTPPath = null;
			string TEMPSPath = null;
			string IPAddres = null;
			int Port = 8080;
			bool IsReadOnlyNode = true;
			MonitorConfig[] monitorscfg =  new MonitorConfig[]{ 
				new MonitorConfig()
				{
					Type = MonitorType.IP,

				}
			};
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

			server.HandeRequest = DataRecieved;
			Monitor[] monitors = new Monitor[monitorscfg.Length];
			for (int i = 0; i < monitorscfg.Length; i++)
			{
				switch (monitorscfg[i].Type) { 
					case MonitorType.IP:
						monitors[i] = new IpMonitor(monitorscfg[i].Data[0]);
						break;
					case MonitorType.GAS:
						monitors[i] = new TemperatureMonitor(monitorscfg[i].Data[0], monitorscfg[i].Data[1]);
						//TODO: implement G
						break;
					case MonitorType.HTTP:
						monitors[i] = new HttpMonitor(monitorscfg[i].Data[0], monitorscfg[i].Data[1]);
						break;
					default:
						Console.Error.WriteLine("Unknown Monitor Type: " + monitorscfg[i].Type);
						break;
				}
			}
			endpoints = new List<DataEndpoint>{
				new IpDataEndpoint("api/ipstatus"),
				new TemperatureDataEndpoint("api/tempstats"),
				new CombinedTempertatureDataEndpoint("api/fulltempstats"),
				new HttpDataEndpoint("api/httpstatus"),
			};
			IFileLoader loader = new LocalFileLoader("Docs\\");
			Log log = new Log(null);
			log._prefix = "Program";
			router.Add(loader, "app.js");
			router.Add(loader, "index.html", "");
			router.Add(loader, "index.html");
			router.Add(loader, "style.css");
			log.Write("Loading Data");
			ManagerInit(IPPath, HTTPPath, TEMPSPath);
			log.Write("Data Loaded");
			if (!IsReadOnlyNode) { Clock.Start(); }
			server.Start($"http://{IPAddres}:{Port}/");
			while (true) { }
		}
		private async static void ManagerInit(string ipmgrPath, string httpmgrPath, string tempmgrPath)
		{
			await Task.WhenAll(
				IpMonitorDataManager.Initialize(ipmgrPath),
				HttpMonitorDataManager.Initialize(httpmgrPath),
				TemperatureMonitorDataManager.Initialize(tempmgrPath)
			);
		}
		public static HttpResponse DataRecieved(HttpRequest request)
		{
			if (request.Method == HttpMethod.GET) { 
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
