using NetBase.Communication;
using Newtonsoft.Json;
using SimpleWebDash.Monitors.Data;
using System;
using System.Linq;
using System.Text;

namespace SimpleWebDash.Endpoints
{
	public class IpE : DataEndpoint
	{
		public IpE(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			int slowNetResponseTime = 50;

			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));
			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;
			IpResponse responseData = IpMonitorDataManager.GetResponseDataRange(start, DateTime.UtcNow, Program.monitorConfigs.ToList().Find((e) => e.ID == request.URLParamenters["id"]).Data[0]); // parsing the stupid shitt because im lazy
			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			if (responseData.Avg > slowNetResponseTime)
			{
				message = "Slow Response";
				responseType = DataResponseType.Warning;
			}
			else
			{
				if (responseData.Timeouts > (responseData.Total / 1000))
				{
					message = "Lost Packets";
					responseType = DataResponseType.Warning;
					if (responseData.Timeouts > (responseData.Total / 2))
					{
						message = "Currenty Expiriencing Colosal Packet Loss!\nThe System May Be Down";
						responseType = DataResponseType.Error;
					}
				}
			}
			ServerDataResponse<IpResponse> response1 = new ServerDataResponse<IpResponse>()
			{
				Type = responseType,
				Message = message,
				Data = responseData
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, JsonConvert.SerializeObject(response1), null, Encoding.UTF8, ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
