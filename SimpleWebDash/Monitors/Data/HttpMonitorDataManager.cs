using System;
using System.Collections.Generic;
using SimpleWebDash.Endpoints;

namespace SimpleWebDash.Monitors.Data
{
	public class HttpMonitorDataManager : SaveObjManager<HttpMonitorData>
	{
		public static HttpMonitorData[] GetAllFrom(DateTime date, string ID)
		{
			//Console.WriteLine(data.Count);
			List<HttpMonitorData> allforip = Saved.FindAll((x) => x.ID == ID);
			//Console.WriteLine(allforip.Count);
			List<HttpMonitorData> allinagiventimespan = allforip.FindAll((x) => x.Time > date);
			//Console.WriteLine(allinagiventimespan.Count);
			return allinagiventimespan.ToArray();
		}
		public static IpEndpointResponseData GetResponseData(DateTime date, string ID)
		{
			HttpMonitorData[] data = GetAllFrom(date, ID);
			IpEndpointResponseData responseData = new IpEndpointResponseData
			{
				Total = data.Length,
				Timeouts = 0,
				Avg = 0
			};
			foreach (HttpMonitorData d in data)
			{
				if (!d.Success)
				{
					responseData.Timeouts++;
				}
				responseData.Avg += d.ResponseTime;
			}
			responseData.Avg /= responseData.Total;
			return responseData;
		}
	}
}
