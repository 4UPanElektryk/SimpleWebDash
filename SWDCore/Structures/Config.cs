using Newtonsoft.Json;

namespace SWDCore.Structures;

public static class Config
{
	public static TConfig Current;
	public static void Load(string Path)
	{
		Current = new TConfig();
		try
		{
			Current = JsonConvert.DeserializeObject<TConfig>(Path);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Error parsing config file");
			Console.Error.WriteLine("Please make sure the config file is in the correct format");

			Console.Error.WriteLine($"Inner Exception: \nMessage: {ex.Message}\nSource: {ex.Source}\nTrace: {ex.StackTrace}");
			Environment.Exit(1);
		}
	}
	public static void Save(string Path)
	{
		File.WriteAllText(Path, JsonConvert.SerializeObject(Current));
	}
}
