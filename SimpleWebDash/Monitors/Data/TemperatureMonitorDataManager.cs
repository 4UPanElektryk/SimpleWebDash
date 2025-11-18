using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWebDash.Monitors.Data
{
	public class TemperatureMonitorDataManager : SaveObjManager<TemperatureMonitorData>
	{
		//key = ip, value = node name
		public static Dictionary<string, string> Nodes = new Dictionary<string, string>();

		internal const int MAX_ALLOWED_DATA_IN_RESPONSE = 1000;
		public static TemperatureMonitorData[] GetAllFrom(DateTime date, string IP)
		{
			//Console.WriteLine(data.Count);
			List<TemperatureMonitorData> allforip = Saved.FindAll((x) => x.IP == IP);
			//Console.WriteLine(allforip.Count);
			List<TemperatureMonitorData> allinagiventimespan = allforip.FindAll((x) => x.Time > date);
			allinagiventimespan.AddRange(Temp);
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
		public static async Task<TemperatureMonitorData[]> GetAllFromAsync(DateTime date, string IP)
		{
			return await Task.Run(() => GetAllFrom(date, IP));
		}
	}
}
