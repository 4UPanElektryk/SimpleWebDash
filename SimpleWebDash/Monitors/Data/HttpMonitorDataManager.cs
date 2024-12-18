using System;
using System.Collections.Generic;

namespace SimpleWebDash.Monitors.Data
{
	public class HttpMonitorDataManager : SaveObjManager<HttpMonitorData>
	{
		public static void Add(HttpMonitorData idata)
		{
			data.Add(idata);
			Save();
		}
		public static HttpMonitorData[] GetAllFrom(DateTime date, string ID)
		{
			//Console.WriteLine(data.Count);
			List<HttpMonitorData> allforip = data.FindAll((x) => x.ID == ID);
			//Console.WriteLine(allforip.Count);
			List<HttpMonitorData> allinagiventimespan = allforip.FindAll((x) => x.Time > date);
			//Console.WriteLine(allinagiventimespan.Count);
			return allinagiventimespan.ToArray();
		}
	}
}
