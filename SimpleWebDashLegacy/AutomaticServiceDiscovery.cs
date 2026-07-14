using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebDash
{
	public static class AutomaticServiceDiscovery
	{
		public enum ScanType
		{
			Basic = 0, // Only search for gas services and internet connection
			Default = 1, // Also Scan port 80, 433 and 8080
			Full = 2, // Also Test DHCP and try to use hostnames
		}
		public static void AttemptServiceDiscovery(string startIp, string endIp, ScanType type)
		{
			Console.WriteLine("Attempting Automatic Service Discovery");
		}
	}
}
