using SimpleWebDash.Endpoints;
using System;
using System.Collections.Generic;

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
			allinagiventimespan.AddRange(Temp);
			return allinagiventimespan.ToArray();
		}
		public static IpEndpointResponseData GetResponseData(DateTime date, string IP)
		{
			IpMonitorData[] data = GetAllFrom(date, IP);
			IpEndpointResponseData response = new IpEndpointResponseData
			{
				Total = data.Length,
				Timeouts = 0,
				Avg = 0,
				Min = long.MaxValue,
				Max = long.MinValue
			};
			foreach (IpMonitorData d in data)
			{
				if (!d.Success)
				{
					response.Timeouts++;
					continue;
				}
				response.Avg += d.ResponseTime;
				if (d.ResponseTime < response.Min)
				{
					response.Min = d.ResponseTime;
				}
				if (d.ResponseTime > response.Max)
				{
					response.Max = d.ResponseTime;
				}
			}
			if (response.Total > 0)
			{
				int dev = response.Total - response.Timeouts;
				if (dev == 0)
					response.Avg = 0;
				else
					response.Avg /= (response.Total - response.Timeouts);
			}
			return response;
		}
	}
}
