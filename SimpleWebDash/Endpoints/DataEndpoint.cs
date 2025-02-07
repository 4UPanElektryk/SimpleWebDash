using NetBase.Communication;

namespace SimpleWebDash
{
	public class DataEndpoint
	{
		public string EndpointUrl;
		public DataEndpoint(string url) { EndpointUrl = url; }
		public virtual HttpResponse ReturnData(HttpRequest request)
		{
			return new HttpResponse(StatusCode.Not_Found);
		}
	}
}
