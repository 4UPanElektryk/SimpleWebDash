using System.Net.NetworkInformation;

namespace SWDCore;

public static class AutomaticServiceDiscovery
{
	public enum ScanType
	{
		Basic = 0, // Only search for gas services and internet connection
		Default = 1, // Also Scan port 80, 433 and 8080
		Full = 2, // Also Test DNS and try to use hostnames
	}
	public static void AttemptServiceDiscovery(string Ip, ScanType type)
	{
		Console.WriteLine("Attempting Automatic Service Discovery");
		NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface iface in interfaces)
		{
			if (iface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
			{
				Console.WriteLine($"Interface Name: {iface.Name}");
				IPInterfaceProperties ipProps = iface.GetIPProperties();
				IPv4InterfaceProperties ipv4Props = ipProps.GetIPv4Properties();
				IPv6InterfaceProperties ipv6Props = ipProps.GetIPv6Properties();
				//ipv4Props.
				Console.WriteLine($"");
			}
		}
	}
}
