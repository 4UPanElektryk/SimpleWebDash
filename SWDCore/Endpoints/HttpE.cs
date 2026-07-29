using NetBase.Communication;
using Newtonsoft.Json;
using SWDCore.Monitors.DataManagers;
using SWDCore.Structures.Endpoint;
using System.Text;

namespace SWDCore.Endpoints;

public class HttpE : DataEndpoint
{
	public HttpE(string url) : base(url) { }
	public override HttpResponse ReturnData(HttpRequest request)
	{
		int slowNetResponseTime = 300;

		TimeSpan span = ParseRequestTimeSpan(request);
		DateTime start = DateTime.UtcNow - span;
		IpResponse responseData = HttpMonitorDataManager.GetResponseData(start, request.URLParamenters["id"]);

		string message = "OK";
		DataResponseType responseType = DataResponseType.Success;
		if (responseData.Avg > slowNetResponseTime)
		{
			message = "Slow Response";
			responseType = DataResponseType.Warning;
		}
		else
		{
			if (responseData.Timeouts > (responseData.Total / 1000))
			{
				message = "Lost Packets";
				responseType = DataResponseType.Warning;
				if (responseData.Timeouts > (responseData.Total / 2))
				{
					message = "Currenty Expiriencing Colosal Packet Loss!\nThe System May Be Down";
					responseType = DataResponseType.Error;
				}
			}
		}
		ServerDataResponse<IpResponse> response1 = new()
		{
			Type = responseType,
			Message = message,
			Data = responseData
		};
		HttpResponse response = new(StatusCode.OK, JsonConvert.SerializeObject(response1), null, Encoding.UTF8, ContentType.application_json);
		response.Headers.Add("Access-Control-Allow-Origin", "*");
		return response;
	}
}
