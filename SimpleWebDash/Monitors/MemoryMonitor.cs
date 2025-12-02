using Newtonsoft.Json;
using SimpleWebDash.Monitors.Data;
using System.Net.Http;

namespace SimpleWebDash.Monitors
{
	public class MemoryMonitor : Monitor
	{
		private string _ip;
		public MemoryMonitor(string IpAddress, string NodeName) : base() { _ip = IpAddress; MemoryMonitorDataManager.Nodes.Add(IpAddress, NodeName); }
		public override void OnEvent(object sender, ClockTickEventArgs e)
		{
			HttpClient client = new HttpClient();
			MemoryInfo data = new MemoryInfo() { FreeMemory = 0, TotalMemory = 0 };
			try
			{
				data = JsonConvert.DeserializeObject<MemoryInfo>(client.GetStringAsync($"http://{_ip}:3000/api/memory").Result);
			}
			catch
			{
				MemoryMonitorDataManager.Add(new MemoryMonitorData()
				{
					IP = _ip,
					free_kb = data.FreeMemory,
					total_kb = data.TotalMemory,
					Time = e.TickTime
				});
				return;
			}

			MemoryMonitorDataManager.Add(new MemoryMonitorData()
			{
				IP = _ip,
				free_kb = data.FreeMemory,
				total_kb = data.TotalMemory,
				Time = e.TickTime
			});
		}
	}
	public struct MemoryInfo
	{
		[JsonProperty("total_kb")]
		public ulong TotalMemory;
		[JsonProperty("free_kb")]
		public ulong FreeMemory;
	}
}
