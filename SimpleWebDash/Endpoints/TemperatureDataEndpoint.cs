using NetBase.Communication;
using Newtonsoft.Json;
using SimpleWebDash.Monitors.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWebDash.Endpoints
{
	public class TemperatureDataEndpoint : DataEndpoint
	{
		public TemperatureDataEndpoint(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));
			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;
			TemperatureMonitorData[] datas = TemperatureMonitorDataManager.GetAllFrom(start, request.URLParamenters["ip"]);
			List<long> values = new List<long>();
			List<int> temps = new List<int>();
			foreach (TemperatureMonitorData data in datas)
			{
				values.Add(((DateTimeOffset)data.Time).ToUnixTimeSeconds());
				temps.Add(data.Temperature);
			}
			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			ServerDataResponse<TemperatureEndpointResponseData> response1 = new ServerDataResponse<TemperatureEndpointResponseData>()
			{
				Type = responseType,
				Message = message,
				Data = new TemperatureEndpointResponseData()
				{
					Times = values.ToArray(),
					Temps = temps.ToArray()
				}
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, null, JsonConvert.SerializeObject(response1), ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
