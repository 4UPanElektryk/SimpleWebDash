using System;
using System.Collections;
using System.Collections.Generic;
using SimpleWebDash.Endpoints;

namespace SimpleWebDash.Monitors.Data
{
	public class IpMonitorDataManager : SaveObjManager<IpMonitorData>
	{
		public static IpMonitorData[] GetAllFrom(DateTime date, string IP)
		{
			//Console.WriteLine(data.Count);
			List<IpMonitorData> allforip = Saved.FindAll((x) => x.IP == IP);
			//Console.WriteLine(allforip.Count);
			List<IpMonitorData> allinagiventimespan = allforip.FindAll((x) => x.Time > date);
			//Console.WriteLine(allinagiventimespan.Count);
			return allinagiventimespan.ToArray();
		}
		public static IpEndpointResponseData GetResponseData(DateTime date, string IP)
		{
			IpMonitorData[] data = GetAllFrom(date, IP);
			IpEndpointResponseData response = new IpEndpointResponseData
			{
				Total = data.Length,
				Timeouts = 0,
				Avg = 0
			};
			foreach (IpMonitorData d in data)
			{
				if (!d.Success)
				{
					response.Timeouts++;
				}
				response.Avg += d.ResponseTime;
			}
			if (response.Total > 0)
			{
				response.Avg /= response.Total;
			}
			return response;
		}
	}
}
