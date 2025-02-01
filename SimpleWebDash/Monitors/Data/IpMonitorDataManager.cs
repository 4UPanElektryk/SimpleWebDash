using System;
using System.Collections.Generic;
using MySqlConnector;

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
