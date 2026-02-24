namespace SimpleWebDash.Endpoints
{
	public struct MemoryResponse
	{
		public long[] Times;
		public ulong[] total_kb;
		public ulong[] used_kb;
		public ulong Avg;
		public ulong Max;
		public ulong Min;
	}
}
