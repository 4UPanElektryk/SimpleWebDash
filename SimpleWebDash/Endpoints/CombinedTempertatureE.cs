using NetBase.Communication;
using Newtonsoft.Json;
using SimpleWebDash.Monitors.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebDash.Endpoints
{
	public class CombinedTempertatureE : DataEndpoint
	{
		public CombinedTempertatureE(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));



			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;

			Dictionary<string, TemperatureResponse> allData = GetAllData(start).Result;

			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			ServerDataResponse<CombinedTempertatureResponse> response1 = new ServerDataResponse<CombinedTempertatureResponse>()
			{
				Type = responseType,
				Message = message,
				Data = new CombinedTempertatureResponse()
				{
					Nodes = TemperatureMonitorDataManager.Nodes,
					Temperatures = allData,
				}
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, JsonConvert.SerializeObject(response1), null, Encoding.UTF8, ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
		/// <summary>
		/// returns all the data for all the nodes
		/// string = ip, value = node temp data
		/// </summary>
		private async Task<Dictionary<string, TemperatureResponse>> GetAllData(DateTime start)
		{
			Dictionary<string, Task<TemperatureMonitorData[]>> allData = new Dictionary<string, Task<TemperatureMonitorData[]>>();
			foreach (var node in TemperatureMonitorDataManager.Nodes)
			{
				Task<TemperatureMonitorData[]> datas = TemperatureMonitorDataManager.GetAllFromAsync(start, node.Key);
				allData.Add(node.Key, datas);
			}
			Task.WaitAll(allData.Values.ToArray());
			Dictionary<string, TemperatureResponse> allDataResult = new Dictionary<string, TemperatureResponse>();
			foreach (var node in allData)
			{
				List<long> values = new List<long>();
				List<int> temps = new List<int>();
				foreach (TemperatureMonitorData data in node.Value.Result)
				{
					values.Add(((DateTimeOffset)data.Time).ToUnixTimeSeconds());
					temps.Add(data.Temperature);
				}
				allDataResult.Add(node.Key, new TemperatureResponse()
				{
					Times = values.ToArray(),
					Temps = temps.ToArray(),
					Max = temps.Count > 0 ? temps.Max() : 0,
					Min = temps.Count > 0 ? temps.Min() : 0,
					Avg = temps.Count > 0 ? (int)temps.Average() : 0,
				});
			}
			return allDataResult;
		}
	}
}
