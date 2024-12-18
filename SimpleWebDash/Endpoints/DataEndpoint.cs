using NetBase.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
