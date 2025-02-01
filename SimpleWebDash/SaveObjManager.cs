using MySqlConnector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SimpleWebDash
{
	public class SaveObjManager<T>
	{
		protected static string connstring = "";
		protected static MySqlConnection NConnection { get { return new MySqlConnection(connstring); } }
		public static void Initialize(string server, string username, string password)
		{
			connstring = $"Server={server};Database=gathereddata;User={username};Password={password};";
		}
	}
}
