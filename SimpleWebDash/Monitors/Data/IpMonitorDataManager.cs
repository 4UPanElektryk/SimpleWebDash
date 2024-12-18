using System;
using System.Collections.Generic;

namespace SimpleWebDash.Monitors.Data
{
	public class IpMonitorDataManager : SaveObjManager<IpMonitorData>
	{
		public static void Add(IpMonitorData idata)
		{
			data.Add(idata);
			Save();
		}
		public static IpMonitorData[] GetAllFrom(DateTime date, string IP)
		{
            //Console.WriteLine(data.Count);
			List<IpMonitorData> allforip = data.FindAll((x) => x.IP == IP);
            //Console.WriteLine(allforip.Count);
			List<IpMonitorData> allinagiventimespan = allforip.FindAll((x) => x.Time > date);
            //Console.WriteLine(allinagiventimespan.Count);
			return allinagiventimespan.ToArray();
		}
	}
}
