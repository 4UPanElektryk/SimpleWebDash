using System;
using System.Collections.Generic;
using MySqlConnector;

namespace SimpleWebDash.Monitors.Data
{
	public class TemperatureMonitorDataManager : SaveObjManager<TemperatureMonitorData>
	{
		internal const int MAX_ALLOWED_DATA_IN_RESPONSE = 1000;
		public static void Add(TemperatureMonitorData idata)
		{
			conn.Open();
			MySqlCommand cmd = new MySqlCommand("INSERT INTO temperaturemonitordata (Temperature, Time, IP) VALUES (@Temperature, @Time, @IP)", conn);
			cmd.Parameters.AddWithValue("@Temperature", idata.Temperature);
			cmd.Parameters.AddWithValue("@Time", idata.Time);
			cmd.Parameters.AddWithValue("@IP", idata.IP);
			cmd.ExecuteNonQuery();
			conn.Close();
		}
		public static TemperatureMonitorData[] GetAllFrom(DateTime date, string IP)
		{
			conn.Open();
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM temperaturemonitordata WHERE Time > @Time AND IP = @IP", conn);
			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@IP", IP);
			MySqlDataReader reader = cmd.ExecuteReader();

			List<TemperatureMonitorData> data = new List<TemperatureMonitorData>();
			while (reader.Read()) {
				TemperatureMonitorData temp = new TemperatureMonitorData();
				temp.IP = reader.GetString("IP");
				temp.Time = reader.GetDateTime("Time");
				temp.Temperature = reader.GetInt32("Temperature");
				data.Add(temp);
			}
			conn.Close();

			//Console.WriteLine(allinagiventimespan.Count);
			List<TemperatureMonitorData> Final = new List<TemperatureMonitorData>();
			int evrynth = data.Count / MAX_ALLOWED_DATA_IN_RESPONSE;
			if (evrynth < 1) { evrynth = 1; }
			for (int i = 0; i < data.Count; i += evrynth)
			{
				Final.Add(data[i]);
			}
			//Console.WriteLine(Final.Count);
			return Final.ToArray();
		}
	}
}
