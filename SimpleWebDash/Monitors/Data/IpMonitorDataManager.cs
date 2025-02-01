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

		public static IpMonitorData[] GetAllFrom(DateTime date, string IP)
		{
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
