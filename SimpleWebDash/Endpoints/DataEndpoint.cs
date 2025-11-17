using NetBase.Communication;
using System;

namespace SimpleWebDash
{
	public class DataEndpoint
	{
		public string EndpointUrl;
		public DataEndpoint(string url) { EndpointUrl = url; }
		public virtual HttpResponse ReturnData(HttpRequest request)
		{
			return new HttpResponse(StatusCode.Not_Found);
		}
		protected TimeSpan ParseRequestTimeSpan(HttpRequest request)
		{
			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));
			return new TimeSpan(days, hours, minutes, 0);
		}
	}
}
