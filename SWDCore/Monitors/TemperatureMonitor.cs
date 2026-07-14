using Newtonsoft.Json;
using SWDCore.Monitors.DataManagers;
using SWDCore.Structures.MonitorData;

namespace SWDCore.Monitors;

public class TemperatureMonitor : Monitor
{
	private string _ip;
	public TemperatureMonitor(string IpAddress, string NodeName) : base() { _ip = IpAddress; TemperatureMonitorDataManager.Nodes.Add(IpAddress, NodeName); }
	public override void OnEvent(object sender, ClockTickEventArgs e)
	{
		HttpClient client = new();
		int[] temps = { 0 };
		try
		{
			temps = JsonConvert.DeserializeObject<int[]>(client.GetStringAsync($"http://{_ip}:3000/api/temperatures").Result);
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
