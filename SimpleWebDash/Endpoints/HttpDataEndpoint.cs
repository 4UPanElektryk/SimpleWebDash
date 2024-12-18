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
			HttpMonitorData[] datas = HttpMonitorDataManager.GetAllFrom(start, request.URLParamenters["id"]);
			bool slowResponses = false;
			int lostPackets = 0;
			long average = 0;
			long Min = 1000000;
			long Max = 0;
			int successCount = 0;
			foreach (HttpMonitorData data in datas)
			{
				if (!data.Success)
				{
					lostPackets++;
					continue;
				}
				if (data.ResponseTime > Max)
				{
					Max = data.ResponseTime;
				}
				if (data.ResponseTime < Min)
				{
					Min = data.ResponseTime;
				}

				average += data.ResponseTime;
				successCount++;
				if (data.ResponseTime > slowNetResponseTime)
				{
					slowResponses = true;
				}
			}
			if (successCount > 0)
			{
				average /= successCount;
			}
			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			if (slowResponses)
			{
				message = "Slow Response";
				responseType = DataResponseType.Warning;
			}
			else
			{
				if (lostPackets > span.TotalMinutes / 10)
				{
					message = "Lost Packets";
					responseType = DataResponseType.Warning;
					if (!datas[datas.Length - 1].Success)
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
				Data = new IpEndpointResponseData()
				{
					Avg = average,
					Max = Max,
					Min = Min,
					Timeouts = lostPackets,
					Total = datas.Length
				}
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, null, JsonConvert.SerializeObject(response1), ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
