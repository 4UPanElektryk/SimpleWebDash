using SWDCore.Monitors.DataManagers;
using System.Timers;
using Timer = System.Timers.Timer;
namespace SWDCore;

public static class Clock
{
	public static event EventHandler<ClockTickEventArgs> Tick;
	private static Timer timer;
	private static Timer AutoSaveTimer;
	public static void Start(int requestInterval, int saveInterval)
	{
		timer = new Timer();
		timer.Elapsed += new ElapsedEventHandler(RequestDataEvent);
		timer.Interval = requestInterval * 1000;
		timer.Enabled = true;
		AutoSaveTimer = new Timer();
		AutoSaveTimer.Elapsed += new ElapsedEventHandler(AutoSaveEvent);
		AutoSaveTimer.Interval = saveInterval * 1000;
		AutoSaveTimer.Enabled = true;
	}
	private static void AutoSaveEvent(object sender, ElapsedEventArgs e)
	{
		IpMonitorDataManager.Save();
		HttpMonitorDataManager.Save();
		TemperatureMonitorDataManager.Save();
		MemoryMonitorDataManager.Save();
	}
	public static void Stop()
	{
		timer.Stop();
	}
	private static void RequestDataEvent(object sender, ElapsedEventArgs e)
	{
		if (Tick != null)
		{
			ClockTickEventArgs args = new();
			args.TickTime = DateTime.UtcNow;
			Tick.Invoke(null, args);
		}
	}
}
public class ClockTickEventArgs : EventArgs
{
	public DateTime TickTime;
}
