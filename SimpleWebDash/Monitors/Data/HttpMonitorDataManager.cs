using System;
using System.Collections.Generic;
using MySqlConnector;

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
		public static HttpMonitorData[] GetAllFrom(DateTime date, string ID)
		{
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM httpmonitordata WHERE Time > @Time AND Address = @Address", conn);
			cmd.Parameters.AddWithValue("@Time", date);
			cmd.Parameters.AddWithValue("@Address", ID);
			conn.Open();
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
	}
}
