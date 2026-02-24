using NetBase.Communication;
using Newtonsoft.Json;
using SimpleWebDash.Monitors.Configuration;
using System.Text;

namespace SimpleWebDash.Endpoints
{
	internal class ConfigurationE : DataEndpoint
	{
		public ConfigurationE(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			MonitorConfig[] currentConfig = Program.monitorConfigs;
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
			ServerDataResponse<ConfigurationResponse> response1 = new ServerDataResponse<ConfigurationResponse>()
			{
				Type = responseType,
				Message = message,
				Data = new ConfigurationResponse()
				{
					Configuration = safeConfigs
				}
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, JsonConvert.SerializeObject(response1), null, Encoding.UTF8, ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
