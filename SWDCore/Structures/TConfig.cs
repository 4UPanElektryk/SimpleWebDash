namespace SWDCore.Structures;

public struct TConfig
{
	public string IpDB;
	public string HttpDB;
	public string TemperatureDB;
	public string MemoryDB;
	public string LogDir;
	public string IPAddress;
	public int Port;
	public bool IsReadOnlyMode;
	public int TestIntervalS;
	public int SaveIntervalS;
	public int SlowNetworkResponseMs;
	public MonitorConfig[] Monitors;
}
