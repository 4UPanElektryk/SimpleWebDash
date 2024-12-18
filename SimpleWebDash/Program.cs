using System.Collections.Generic;
using System.Net;
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
			IpMonitorDataManager.Initialize("IpData.json");
			HttpMonitorDataManager.Initialize("HttpData.json");
			TemperatureMonitorDataManager.Initialize("TempsData.json");
			Clock.Start();
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
