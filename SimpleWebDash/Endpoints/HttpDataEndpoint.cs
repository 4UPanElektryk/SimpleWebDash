using MySqlConnector;
using NetBase.Communication;
using Newtonsoft.Json;
using SimpleWebDash.Monitors.Data;
using System;

namespace SimpleWebDash.Endpoints
{
	public class HttpDataEndpoint : DataEndpoint
	{
		public HttpDataEndpoint(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			int slowNetResponseTime = 150;

			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));
			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;
			IpEndpointResponseData responseData = HttpMonitorDataManager.GetResponseData(start, request.URLParamenters["id"]);
			
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
			ServerDataResponse<IpEndpointResponseData> response1 = new ServerDataResponse<IpEndpointResponseData>()
			{
				Type = responseType,
				Message = message,
				Data = responseData
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, null, JsonConvert.SerializeObject(response1), ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
