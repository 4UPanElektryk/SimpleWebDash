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
	public class CombinedMemoryDataEndpoint : DataEndpoint
	{
		public CombinedMemoryDataEndpoint(string url) : base(url) { }

		public override HttpResponse ReturnData(HttpRequest request)
		{
			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));



			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;

			Dictionary<string, MemoryEndpointResponseData> allData = GetAllData(start).Result;

			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			ServerDataResponse<CombinedMemoryEndpointResponseData> response1 = new ServerDataResponse<CombinedMemoryEndpointResponseData>()
			{
				Type = responseType,
				Message = message,
				Data = new CombinedMemoryEndpointResponseData()
				{
					MemoryData = allData,
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
		private async Task<Dictionary<string, MemoryEndpointResponseData>> GetAllData(DateTime start)
		{
			Dictionary<string, Task<MemoryMonitorData[]>> allData = new Dictionary<string, Task<MemoryMonitorData[]>>();
			foreach (var node in TemperatureMonitorDataManager.Nodes)
			{
				Task<MemoryMonitorData[]> datas = MemoryMonitorDataManager.GetAllFromAsync(start, node.Key);
				allData.Add(node.Value, datas);
			}
			Task.WaitAll(allData.Values.ToArray());
			Dictionary<string, MemoryEndpointResponseData> allDataResult = new Dictionary<string, MemoryEndpointResponseData>();
			foreach (var node in allData)
			{
				List<long> timestamps = new List<long>();
				List<ulong> totalkbs = new List<ulong>();
				List<ulong> usedkbs = new List<ulong>();
				foreach (MemoryMonitorData data in node.Value.Result)
				{
					timestamps.Add(((DateTimeOffset)data.Time).ToUnixTimeSeconds());
					totalkbs.Add(data.total_kb);
					usedkbs.Add(data.total_kb - data.free_kb);
				}
				allDataResult.Add(node.Key, new MemoryEndpointResponseData()
				{
					Times = timestamps.ToArray(),
					total_kb = totalkbs.ToArray(),
					used_kb = usedkbs.ToArray(),
					Max = usedkbs.Count > 0 ? usedkbs.Max() : 0,
					Min = usedkbs.Count > 0 ? usedkbs.Min() : 0,
					Avg = usedkbs.Count > 0 ? (ulong)(usedkbs.Sum(x => (double)x) / usedkbs.Count) : 0,
				});
			}
			return allDataResult;
		}
	}
}
