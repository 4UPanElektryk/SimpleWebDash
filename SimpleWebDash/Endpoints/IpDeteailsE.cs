using NetBase.Communication;
using Newtonsoft.Json;
using SimpleWebDash.Endpoints.DataStructures;
using SimpleWebDash.Monitors.Data;
using System;
using System.Linq;
using System.Text;

namespace SimpleWebDash.Endpoints
{
	class IpDeteailsE : DataEndpoint
	{
		public IpDeteailsE(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			string id = Program.monitorConfigs.ToList().Find((e) => e.ID == request.URLParamenters["id"]).Data[0];
			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));
			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;
			int steps = int.Parse(request.URLParamenters["step"]);

			IpResponse[] responses = new IpResponse[steps];

			for (int i = 0; i < steps; i++)
			{
				responses[i] = IpMonitorDataManager.GetResponseDataRange(start.AddTicks(span.Ticks / steps * i), start.AddTicks(span.Ticks / steps * (i + 1)), id);
			}

			ServerDataResponse<IpDetailsResponse> response1 = new ServerDataResponse<IpDetailsResponse>()
			{
				Type = DataResponseType.Success,
				Message = "OK",
				Data = new IpDetailsResponse() { Data = responses }
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, JsonConvert.SerializeObject(response1), null, Encoding.UTF8, ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
