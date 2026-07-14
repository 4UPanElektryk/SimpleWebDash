namespace SimpleWebDash.Endpoints
{
	public struct TemperatureResponse
	{
		public long[] Times;
		public int[] Temps;
		public int Avg;
		public int Max;
		public int Min;
	}
}
