using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWebDash.Monitors.Data
{
	public class MemoryMonitorDataManager : SaveObjManager<MemoryMonitorData>
	{
		//key = ip, value = node name
		public static Dictionary<string, string> Nodes = new Dictionary<string, string>();

		internal const int MAX_ALLOWED_DATA_IN_RESPONSE = 1000;
		public static MemoryMonitorData[] GetAllFrom(DateTime date, string IP)
		{
			//Console.WriteLine(data.Count);
			List<MemoryMonitorData> allforip = Saved.FindAll((x) => x.IP == IP);
			allforip.AddRange(Temp.FindAll((x) => x.IP == IP));
			//Console.WriteLine(allforip.Count);
			List<MemoryMonitorData> allinagiventimespan = allforip.FindAll((x) => x.Time > date);
			//Console.WriteLine(allinagiventimespan.Count);
			List<MemoryMonitorData> Final = new List<MemoryMonitorData>();
			int evrynth = allinagiventimespan.Count / MAX_ALLOWED_DATA_IN_RESPONSE;
			if (evrynth < 1) { evrynth = 1; }
			for (int i = 0; i < allinagiventimespan.Count; i += evrynth)
			{
				Final.Add(allinagiventimespan[i]);
			}
			//Console.WriteLine(Final.Count);
			return Final.ToArray();
		}
		public static async Task<MemoryMonitorData[]> GetAllFromAsync(DateTime date, string IP)
		{
			return await Task.Run(() => GetAllFrom(date, IP));
		}
	}
}
