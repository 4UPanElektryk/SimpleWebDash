using System;
using System.Collections;
using System.Collections.Generic;
using MySqlConnector;
using SimpleWebDash.Endpoints;

namespace SimpleWebDash.Monitors.Data
{
	public class IpMonitorDataManager : SaveObjManager<IpMonitorData>
	{
		public static void Add(IpMonitorData idata)
		{
			MySqlConnection conn = NConnection;
			conn.Open();
			MySqlCommand cmd = new MySqlCommand("INSERT INTO ipmonitordata (IP, Time, Success, ResponseTime) VALUES (@IP, @Time, @Success, @ResponseTime)", conn);
			cmd.Parameters.AddWithValue("@IP", idata.IP);
			cmd.Parameters.AddWithValue("@Time", idata.Time);
			cmd.Parameters.AddWithValue("@Success", idata.Success);
			cmd.Parameters.AddWithValue("@ResponseTime", idata.ResponseTime);
			cmd.ExecuteNonQuery();
			conn.Close();
		}
		//get ipmonitordata from a specific date and IP and return it as IpEndpointResponseData
		public static IpEndpointResponseData GetResponseData(DateTime date, string IP)
		{
			MySqlConnection conn = NConnection;
			Console.WriteLine($"{DateTime.UtcNow:mm:ss.FFF}| Begin IP Check For {IP}");
			conn.Open();
			// IpEndpointResponseData
			// Avg, Max, Min, Timeouts, Total
			// SELECT * FROM httpmonitordata WHERE Time > @Time AND Address = @Address
			MySqlCommand cmd = new MySqlCommand("SELECT AVG(ResponseTime) as Avg, MAX(ResponseTime) as Max, MIN(ResponseTime) as Min, COUNT(ID) as Total FROM ipmonitordata WHERE Time > @Time AND IP = @Address", conn);
			Console.WriteLine($"{DateTime.UtcNow:mm:ss.FFF}| Comand IP Check For {IP}");
			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@Address", IP);
			MySqlDataReader reader = cmd.ExecuteReader();
			Console.WriteLine($"{DateTime.UtcNow:mm:ss.FFF}| Read Started IP Check For {IP}");
			reader.Read();
			long avg = reader.GetInt32("Avg");
			long max = reader.GetInt32("Max");
			long min = reader.GetInt32("Min");
			int total = reader.GetInt32("Total");
			reader.Close();

			Console.WriteLine($"{DateTime.UtcNow:mm:ss.FFF}| Timeouts IP Check For {IP}");
			cmd = new MySqlCommand("SELECT COUNT(ID) as Timeouts FROM httpmonitordata WHERE Time > @Time AND Address = @Address AND Success = 0", conn);
			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@Address", IP);
			reader = cmd.ExecuteReader();
			Console.WriteLine($"{DateTime.UtcNow:mm:ss.FFF}| Timeouts Read Started IP Check For {IP}");
			reader.Read();
			int timeouts = reader.GetInt32("Timeouts");
			reader.Close();
			conn.Close();
			Console.WriteLine($"{DateTime.UtcNow:mm:ss.FFF}| Finished IP Check For {IP}");
			return new IpEndpointResponseData
			{
				Avg = avg,
				Max = max,
				Min = min,
				Timeouts = timeouts,
				Total = total
			};
		}

		public static IpMonitorData[] GetAllFrom(DateTime date, string IP)
		{
			MySqlConnection conn = NConnection;
			conn.Open();
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM ipmonitordata WHERE Time > @Time AND IP = @IP", conn);
			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@IP", IP);
			MySqlDataReader reader = cmd.ExecuteReader();

			List<IpMonitorData> data = new List<IpMonitorData>();
			while (reader.Read())
			{
				IpMonitorData temp = new IpMonitorData();
				temp.IP = reader.GetString("IP");
				temp.Time = reader.GetDateTime("Time");
				temp.Success = reader.GetBoolean("Success");
				temp.ResponseTime = reader.GetInt64("ResponseTime");
				data.Add(temp);
			}
			conn.Close();
			return data.ToArray();
		}
	}
}
