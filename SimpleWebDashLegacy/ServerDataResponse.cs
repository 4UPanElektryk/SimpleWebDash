namespace SimpleWebDash
{
	public enum DataResponseType
	{
		Success,
		Warning,
		Error,
	}
	public struct ServerDataResponse<T>
	{
		//[JsonConverter(typeof(StringEnumConverter))]
		public DataResponseType Type { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }
	}
}
