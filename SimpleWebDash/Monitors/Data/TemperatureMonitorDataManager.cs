using System;
using System.Collections.Generic;

namespace SimpleWebDash.Monitors.Data
{
	public class TemperatureMonitorDataManager : SaveObjManager<TemperatureMonitorData>
	{
		internal const int MAX_ALLOWED_DATA_IN_RESPONSE = 1000;
		public static void Add(TemperatureMonitorData idata)
		{
			data.Add(idata);
			Save();
		}
		public static TemperatureMonitorData[] GetAllFrom(DateTime date, string IP)
		{
			//Console.WriteLine(data.Count);
			List<TemperatureMonitorData> allforip = data.FindAll((x) => x.IP == IP);
			//Console.WriteLine(allforip.Count);
			List<TemperatureMonitorData> allinagiventimespan = allforip.FindAll((x) => x.Time > date);
			//Console.WriteLine(allinagiventimespan.Count);
			List<TemperatureMonitorData> Final = new List<TemperatureMonitorData>();
			int evrynth = allinagiventimespan.Count / MAX_ALLOWED_DATA_IN_RESPONSE;
			if (evrynth < 1) { evrynth = 1; }
			for (int i = 0; i < allinagiventimespan.Count; i += evrynth)
			{
				Final.Add(allinagiventimespan[i]);
			}
			//Console.WriteLine(Final.Count);
			return Final.ToArray();
		}
	}
}
