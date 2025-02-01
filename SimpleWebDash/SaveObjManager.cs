using MySqlConnector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SimpleWebDash
{
	public class SaveObjManager<T>
	{
		protected static MySqlConnection conn;
		public static void Initialize(string server, string username, string password)
		{
			conn = new MySqlConnection($"Server={server};Database=gathereddata;User={username};Password={password};");
		}
	}
}
