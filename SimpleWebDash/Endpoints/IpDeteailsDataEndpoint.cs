using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBase.Communication;

namespace SimpleWebDash.Endpoints
{
    class IpDeteailsDataEndpoint : DataEndpoint
    {
		public IpDeteailsDataEndpoint(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			string ip = request.URLParamenters["ip"];
			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));
			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;

			//TODO: Implement the backend logig to get data in a speciefied time range
			throw new NotImplementedException();
			IpDetails details = IpMonitorDataManager.GetIpDetails(ip);
		}
	}
}
