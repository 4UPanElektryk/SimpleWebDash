using System.Collections.Generic;
using System.Net;
using CoolConsole;
using CoolConsole.MenuItems;
using NetBase;
using NetBase.Communication;
using NetBase.FileProvider;
using NetBase.StaticRouting;
using SimpleWebDash.Endpoints;
using SimpleWebDash.Monitors;
using SimpleWebDash.Monitors.Data;

namespace SimpleWebDash
{
	internal class Program
	{
		private static List<DataEndpoint> endpoints;
		static void Main(string[] args)
		{
			ReturnCode code = Menu.Show(new List<MenuItem>
			{
				new TextboxMenuItem("IP Monitor Data Path", "ips.c.json"),
				new TextboxMenuItem("HTTP Monitor Data Path", "https.c.json"),
				new TextboxMenuItem("TEMPS Monitor Data Path", "temps.c.json"),
				new CheckboxMenuItem("ReadOnlyNode", true),
				new MenuItem("Continue")
			});
			string IPPath = code.Textboxes[0];
			string HTTPPath = code.Textboxes[1];
			string TEMPSPath = code.Textboxes[2];
			bool IsReadOnlyNode = code.Checkboxes[0];
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
			Server.Start(IPAddress.Loopback, 8080);
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
