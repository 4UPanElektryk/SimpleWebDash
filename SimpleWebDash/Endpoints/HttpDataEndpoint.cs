using MySqlConnector;
using NetBase.Communication;
using Newtonsoft.Json;
using SimpleWebDash.Monitors.Data;
using System;

namespace SimpleWebDash.Endpoints
{
	public class HttpDataEndpoint : DataEndpoint
	{
		public HttpDataEndpoint(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			int slowNetResponseTime = 150;

			string tspan = request.URLParamenters["t"]; // "0000d00h00m";
			int days = int.Parse(tspan.Split('d')[0]);
			int hours = int.Parse(tspan.Split('d')[1].Split('h')[0]);
			int minutes = int.Parse(tspan.Split('d')[1].Split('h')[1].TrimEnd('m'));
			TimeSpan span = new TimeSpan(days, hours, minutes, 0);
			DateTime start = DateTime.UtcNow - span;
			
			MySqlConnection conn = HttpMonitorDataManager.GetConnection();
			conn.Open();
			// IpEndpointResponseData
			// Avg, Max, Min, Timeouts, Total
			// SELECT * FROM httpmonitordata WHERE Time > @Time AND Address = @Address
			MySqlCommand cmd = new MySqlCommand("SELECT AVG(ResponseTime) as Avg, MAX(ResponseTime) as Max, MIN(ResponseTime) as Min, COUNT(ID) as Total FROM httpmonitordata WHERE Time > @Time AND Address = @Address", conn);

			cmd.Parameters.AddWithValue("@Time", start);
			cmd.Parameters.AddWithValue("@Address", request.URLParamenters["id"]);
			MySqlDataReader reader = cmd.ExecuteReader();
			reader.Read();
			long avg = reader.GetInt64("Avg");
			long max = reader.GetInt64("Max");
			long min = reader.GetInt64("Min");
			int total = reader.GetInt32("Total");
			reader.Close();

			cmd = new MySqlCommand("SELECT COUNT(ID) as Timeouts FROM httpmonitordata WHERE Time > @Time AND Address = @Address AND Success = 0", conn);
			cmd.Parameters.AddWithValue("@Time", start);
			cmd.Parameters.AddWithValue("@Address", request.URLParamenters["id"]);
			reader = cmd.ExecuteReader();
			reader.Read();
			int timeouts = reader.GetInt32("Timeouts");
			reader.Close();
			conn.Close();
			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			if (avg > slowNetResponseTime)
			{
				message = "Slow Response";
				responseType = DataResponseType.Warning;
			}
			else
			{
				if (timeouts > (total / 1000))
				{
					message = "Lost Packets";
					responseType = DataResponseType.Warning;
					if (timeouts > (total / 2))
					{
						message = "Currenty Expiriencing Colosal Packet Loss!\nThe System May Be Down";
						responseType = DataResponseType.Error;
					}
				}
			}
			ServerDataResponse<IpEndpointResponseData> response1 = new ServerDataResponse<IpEndpointResponseData>()
			{
				Type = responseType,
				Message = message,
				Data = new IpEndpointResponseData()
				{
					Avg = avg,
					Max = max,
					Min = min,
					Timeouts = timeouts,
					Total = total
				}
			};

			HttpResponse response = new HttpResponse(StatusCode.OK, null, JsonConvert.SerializeObject(response1), ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
