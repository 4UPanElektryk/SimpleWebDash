using SWDCore.Monitors.DataManagers;
using SWDCore.Structures.MonitorData;
using System.Net.NetworkInformation;
using System.Text;

namespace SWDCore.Monitors;

public class IpMonitor : Monitor
{
	private string _ip;
	public IpMonitor(string IpAddress) : base() { _ip = IpAddress; }
	public override void OnEvent(object sender, ClockTickEventArgs e)
	{
		int timeout = 120;
		Ping pingSender = new();
		PingOptions options = new() { DontFragment = true };

		// Create a buffer of 32 bytes of data to be transmitted.
		string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
		byte[] buffer = Encoding.ASCII.GetBytes(data);
		PingReply reply = pingSender.Send(_ip, timeout);//, buffer, options);
		if (reply.Status != IPStatus.Success)
		{
			timeout = 250;
			reply = pingSender.Send(_ip, timeout);//, buffer, options);
		}
		IpMonitorDataManager.Add(new IpMonitorData()
		{
			IP = _ip,
			ResponseTime = reply.RoundtripTime,
			Success = IPStatus.Success == reply.Status,
			Time = e.TickTime
		});
	}
}
