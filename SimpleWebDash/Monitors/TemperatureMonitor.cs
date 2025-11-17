using Newtonsoft.Json;
using SimpleWebDash.Monitors.Data;
using System.Linq;
using System.Net.Http;

namespace SimpleWebDash.Monitors
{
	public class TemperatureMonitor : Monitor
	{
		private string _ip;
		public TemperatureMonitor(string IpAddress, string NodeName) : base() { _ip = IpAddress; TemperatureMonitorDataManager.Nodes.Add(IpAddress, NodeName); }
		public override void OnEvent(object sender, ClockTickEventArgs e)
		{
			HttpClient client = new HttpClient();
			int[] temps = { 0 };
			try
			{
				temps = JsonConvert.DeserializeObject<int[]>(client.GetStringAsync($"http://{_ip}:3000/temps").Result);
			}
			catch
			{
				TemperatureMonitorDataManager.Add(new TemperatureMonitorData()
				{
					IP = _ip,
					Temperature = 0,
					Time = e.TickTime
				});
				return;
			}

			TemperatureMonitorDataManager.Add(new TemperatureMonitorData()
			{
				IP = _ip,
				Temperature = temps.Max(),
				Time = e.TickTime
			});
		}
	}
}
