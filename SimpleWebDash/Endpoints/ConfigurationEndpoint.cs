using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBase.Communication;

namespace SimpleWebDash.Endpoints
{
	internal class ConfigurationEndpoint : DataEndpoint
	{
		public ConfigurationEndpoint(string url) : base(url) { }
		public override HttpResponse ReturnData(HttpRequest request)
		{
			string message = "OK";
			DataResponseType responseType = DataResponseType.Success;
			ServerDataResponse<ConfigurationEndpointResponseData> response1 = new ServerDataResponse<ConfigurationEndpointResponseData>()
			{
				Type = responseType,
				Message = message,
				Data = new ConfigurationEndpointResponseData()
				{
					Configuration = SimpleWebDash.ConfigurationManager.GetCurrentConfiguration(),
				}
			};
			HttpResponse response = new HttpResponse(StatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(response1), null, Encoding.UTF8, ContentType.application_json);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
