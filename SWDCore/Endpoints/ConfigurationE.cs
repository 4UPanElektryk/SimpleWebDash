using NetBase.Communication;
using Newtonsoft.Json;
using SWDCore.Structures;
using SWDCore.Structures.Endpoint;
using System.Text;

namespace SWDCore.Endpoints;

internal class ConfigurationE : DataEndpoint
{
	public ConfigurationE(string url) : base(url) { }
	public override HttpResponse ReturnData(HttpRequest request)
	{
		string message = "OK";
		DataResponseType responseType = DataResponseType.Success;
		MonitorConfig[] currentConfig = Config.Current.Monitors;
		SafeMonitorConfig[] safeConfigs = new SafeMonitorConfig[currentConfig.Length];
		for (int i = 0; i < currentConfig.Length; i++)
		{
			safeConfigs[i] = new SafeMonitorConfig()
			{
				ID = currentConfig[i].ID,
				FriendlyName = currentConfig[i].FriendlyName,
				Type = currentConfig[i].Type
			};
		}
		ServerDataResponse<ConfigurationResponse> response1 = new()
		{
			Type = responseType,
			Message = message,
			Data = new ConfigurationResponse()
			{
				Configuration = safeConfigs
			}
		};
		HttpResponse response = new(StatusCode.OK, JsonConvert.SerializeObject(response1), null, Encoding.UTF8, ContentType.application_json);
		response.Headers.Add("Access-Control-Allow-Origin", "*");
		return response;
	}
}
