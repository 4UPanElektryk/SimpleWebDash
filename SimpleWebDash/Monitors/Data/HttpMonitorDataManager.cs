using System;
using System.Collections.Generic;
using MySqlConnector;
using SimpleWebDash.Endpoints;

namespace SimpleWebDash.Monitors.Data
{
	public class HttpMonitorDataManager : SaveObjManager<HttpMonitorData>
	{
		public static void Add(HttpMonitorData idata)
		{
			conn.Open();
			MySqlCommand cmd = new MySqlCommand("INSERT INTO httpmonitordata (Address, Time, Success, ResponseTime) VALUES (@Address, @Time, @Success, @ResponseTime)", conn);
			cmd.Parameters.AddWithValue("@Address", idata.ID);
			cmd.Parameters.AddWithValue("@Time", idata.Time);
			cmd.Parameters.AddWithValue("@Success", idata.Success);
			cmd.Parameters.AddWithValue("@ResponseTime", idata.ResponseTime);
			cmd.ExecuteNonQuery();
			conn.Close();
		}
		public static IpEndpointResponseData GetResponseData(DateTime date, string IP)
		{
			conn.Open();
			// IpEndpointResponseData
			// Avg, Max, Min, Timeouts, Total
			// SELECT * FROM httpmonitordata WHERE Time > @Time AND Address = @Address
			MySqlCommand cmd = new MySqlCommand("SELECT AVG(ResponseTime) as Avg, MAX(ResponseTime) as Max, MIN(ResponseTime) as Min, COUNT(ID) as Total FROM httpmonitordata WHERE Time > @Time AND Address = @Address", conn);

			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@Address", IP);
			MySqlDataReader reader = cmd.ExecuteReader();
			reader.Read();
			long avg = reader.GetInt64("Avg");
			long max = reader.GetInt64("Max");
			long min = reader.GetInt64("Min");
			int total = reader.GetInt32("Total");
			reader.Close();

			cmd = new MySqlCommand("SELECT COUNT(ID) as Timeouts FROM httpmonitordata WHERE Time > @Time AND Address = @Address AND Success = 0", conn);
			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@Address", IP);
			reader = cmd.ExecuteReader();
			reader.Read();
			int timeouts = reader.GetInt32("Timeouts");
			reader.Close();
			conn.Close();

			return new IpEndpointResponseData
			{
				Avg = avg,
				Max = max,
				Min = min,
				Timeouts = timeouts,
				Total = total
			};
		}
		public static HttpMonitorData[] GetAllFrom(DateTime date, string ID)
		{
			conn.Open();
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM httpmonitordata WHERE Time > @Time AND Address = @Address", conn);
			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@Address", ID);
			MySqlDataReader reader = cmd.ExecuteReader();
			
			List<HttpMonitorData> data = new List<HttpMonitorData>();
			while (reader.Read())
			{
				HttpMonitorData temp = new HttpMonitorData();
				temp.ID = reader.GetString("Address");
				temp.Time = reader.GetDateTime("Time");
				temp.Success = reader.GetBoolean("Success");
				temp.ResponseTime = reader.GetInt64("ResponseTime");
				data.Add(temp);
			}
			conn.Close();
			return data.ToArray();
		}
		public static MySqlConnection GetConnection()
		{
			return conn;
		}
	}
}
