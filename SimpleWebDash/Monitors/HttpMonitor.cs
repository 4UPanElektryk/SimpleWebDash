using SimpleWebDash.Monitors.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace SimpleWebDash.Monitors
{
	public class HttpMonitor : Monitor
	{
		private string _requestString;
		private string _id;
		public HttpMonitor(string RequestString, string Id) : base() { _id = Id; _requestString = RequestString; }
		public override void OnEvent(object sender, ClockTickEventArgs e)
		{
			int timeout = 1000;

			var handler = new HttpClientHandler();
			handler.ClientCertificateOptions = ClientCertificateOption.Manual;
			handler.ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				};

			HttpClient client = new HttpClient(handler);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _requestString);
			client.Timeout = new TimeSpan(0,0,0,0,timeout);
			Stopwatch stopwatch = Stopwatch.StartNew();
			Task<HttpResponseMessage> httpResponse = client.SendAsync(request);
			httpResponse.Wait();
			stopwatch.Stop();
			HttpMonitorDataManager.Add(new HttpMonitorData()
			{
				ID = _id,
				ResponseTime = stopwatch.ElapsedMilliseconds,
				Success = httpResponse.Result.StatusCode == System.Net.HttpStatusCode.OK,
				Time = e.TickTime
			});
		}
	}
}
