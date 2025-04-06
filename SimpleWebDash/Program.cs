using System.Collections.Generic;
using System.Net;
using CoolConsole;
using CoolConsole.MenuItems;
using NetBase;
using NetBase.Communication;
using NetBase.FileProvider;
using NetBase.StaticRouting;
using Newtonsoft.Json.Linq;
using SimpleWebDash.Endpoints;
using SimpleWebDash.Monitors;
using SimpleWebDash.Monitors.Data;
using System.IO;
using System;

namespace SimpleWebDash
{
	internal class Program
	{
		private static List<DataEndpoint> endpoints;
		static void Main(string[] args)
		{
			// replace this mess with a config file if a config file is passed through the command line as an argument
			// could be a json file or an ini file
			string IPPath = null;
			string HTTPPath = null;
			string TEMPSPath = null;
			string IPAddres = null;
			int Port = 8080;
			bool IsReadOnlyNode = true;
			if (args.Length != 1)
			{
				ReturnCode code = Menu.Show(new List<MenuItem>
				{
					new TextboxMenuItem("IP Monitor Data Path", "ips.c.json"),
					new TextboxMenuItem("HTTP Monitor Data Path", "https.c.json"),
					new TextboxMenuItem("TEMPS Monitor Data Path", "temps.c.json"),
					new CheckboxMenuItem("ReadOnlyNode", true),
					new TextboxMenuItem("IP Address", "127.0.0.1"),
					new NumboxMenuItem("Port", 8080),
					new MenuItem("Continue")
				});
				IPPath = code.Textboxes[0];
				HTTPPath = code.Textboxes[1];
				TEMPSPath = code.Textboxes[2];
				IPAddres = code.Textboxes[3];
				Port = code.Numboxes[0];
				IsReadOnlyNode = code.Checkboxes[0];
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
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("Error parsing config file");
					Console.Error.WriteLine("Please make sure the config file is in the correct format");
					Console.Error.WriteLine("The config file should be a json file with the following format:");
					Console.Error.WriteLine("{");
					Console.Error.WriteLine("  \"IPMonitorDataPath\": \"ips.c.json\",");
					Console.Error.WriteLine("  \"HTTPMonitorDataPath\": \"https.c.json\",");
					Console.Error.WriteLine("  \"TEMPSMonitorDataPath\": \"temps.c.json\",");
					Console.Error.WriteLine("  \"IPAddress\": \"127.0.0.1\",");
					Console.Error.WriteLine("  \"Port\": 8080,");
					Console.Error.WriteLine("  \"ReadOnlyNode\": true");
					Console.Error.WriteLine("}");
#if DEBUG
					throw ex;
#endif
					Environment.Exit(1);
				}
			}

			Server.router = DataRecieved;
			Monitor[] monitors = {
				new IpMonitor("192.168.10.149"),
				new IpMonitor("192.168.10.251"),
				new IpMonitor("192.168.10.252"),
				new IpMonitor("hole.lan"),
				new TemperatureMonitor("192.168.10.10"),
				new TemperatureMonitor("192.168.10.11"),
				new TemperatureMonitor("192.168.10.12"),
				new HttpMonitor("https://192.168.10.252:8283/login?redirect_url=/apps/dashboard/", "NextCloud"),
				new HttpMonitor("https://192.168.10.249/live", "NVR")
			};
			endpoints = new List<DataEndpoint>{
				new IpDataEndpoint("api/ipstatus"),
				new TemperatureDataEndpoint("api/tempstats"),
				new HttpDataEndpoint("api/httpstatus")
			};
			IFileLoader loader = new LocalFileLoader("Docs\\");
			Router.Add(loader, "app.js");
			Router.Add(loader, "index.html", "");
			Router.Add(loader, "index.html");
			Router.Add(loader, "style.css");
			IpMonitorDataManager.Initialize(IPPath);
			HttpMonitorDataManager.Initialize(HTTPPath);
			TemperatureMonitorDataManager.Initialize(TEMPSPath);
			if (!IsReadOnlyNode) { Clock.Start(); }
			Server.Start(IPAddress.Parse(IPAddres), Port);
			while (true) { }
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
