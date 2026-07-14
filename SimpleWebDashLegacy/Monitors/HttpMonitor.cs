using SimpleWebDash.Monitors.Data;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleWebDash.Monitors
{
	public class HttpMonitor : Monitor
	{
		private string _requestString;
		private string _id;
		public HttpMonitor(string Id, string RequestString) : base() { _id = Id; _requestString = RequestString; }
		public override void OnEvent(object sender, ClockTickEventArgs e)
		{
			int timeout = 2000;

			var handler = new HttpClientHandler();
			handler.ClientCertificateOptions = ClientCertificateOption.Manual;
			handler.ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				};

			HttpClient client = new HttpClient(handler);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _requestString);
			client.Timeout = new TimeSpan(0, 0, 0, 0, timeout);
			Stopwatch stopwatch = Stopwatch.StartNew();
			bool success = false;
			try
			{
				Task<HttpResponseMessage> httpResponse = client.SendAsync(request);
				httpResponse.Wait();
				success = httpResponse.Result.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{e.TickTime} {this.GetType().Name}: {_id}\nException occured: {ex.Message}\nStack Trace: {ex.StackTrace}\n");
			}
			stopwatch.Stop();
			HttpMonitorDataManager.Add(new HttpMonitorData()
			{
				ID = _id,
				ResponseTime = stopwatch.ElapsedMilliseconds,
				Success = success,
				Time = e.TickTime
			});
		}
	}
}
