using Newtonsoft.Json;
using System.Text;

namespace SWDCore;

public class SaveObjManager<T>
{
	public static List<T> Saved;
	public static List<T> Temp;
	protected static string Path;
	public static async Task Initialize(string path)
	{
		Saved = new List<T>();
		Temp = new List<T>();
		Path = path;
		Saved = await Load();
	}
	public static string Serialize(T obj)
	{
		return JsonConvert.SerializeObject(obj);
	}
	public static T Deserialize(string obj)
	{
		return JsonConvert.DeserializeObject<T>(obj);
	}
	public static async Task<List<T>> Load()
	{
		if (!File.Exists(Path))
		{
			File.Create(Path).Close();
			return new List<T>();
		}
		List<T> local = new();
		string[] strings = File.ReadAllLines(Path);
		local.Capacity = strings.Length;
		for (int i = 0; i < strings.Length; i++)
		{
			local.Add(Deserialize(strings[i]));
		}
		return local;
	}
	public static void Save()
	{
		StringBuilder stringBuilder = new();
		lock (Temp)
		{
			foreach (T obj in Temp)
			{
				stringBuilder.AppendLine(Serialize(obj));
			}
			Saved.Capacity += Temp.Count;
			Saved.AddRange(Temp);
			Temp.Clear();
		}
		File.AppendAllText(Path, stringBuilder.ToString());
		Program.log.Write("Saved to " + Path);
	}
	public static void Add(T obj)
	{
		Temp.Add(obj);
	}
}
