using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace SimpleWebDash
{
	public class SaveObjManager<T>
	{
		//TODO: Convert this all to sql
		protected static List<T> data;
		private static string _path;
		public static void Initialize(string path)
		{
			_path = path;
			Load();
		}

		/// <summary>
		/// saves a list of object of a given type
		/// </summary>
		public static void Save()
		{
			File.WriteAllText(_path, JsonConvert.SerializeObject(data, Formatting.Indented));
		}

		/// <summary>
		/// loads a list of object of a given type
		/// </summary>
		public static void Load()
		{
			if (File.Exists(_path))
			{
				data = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(_path));
			}
			else
			{
				data = new List<T>();
			}
		}
	}
}
