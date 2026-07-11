interface Dictionary<T> {
	[Key: string]: T;
}

interface ServerDataResponse<T> {
	Type: DataResponseType,
	Message: string,
	Data: T
}

interface IpResponse {
	Min: number,
	Max: number,
	Avg: number,
	Timeouts: number,
	Total: number
}

interface TemperatureResponse {
	Times: Array<number>,
	Temps: Array<number>,
	Min: number,
	Max: number,
    Avg: number
}

interface CombinedTemperatureResponse {
	Nodes: Dictionary<string>,
	Temperatures: Dictionary<TemperatureResponse>
}

interface MemoryResponse {
	Times: Array<number>;
	total_kb: Array<number>;
	used_kb: Array<number>;
	Avg: number;
	Max: number;
	Min: number;
}

interface CombinedMemoryResponse {
	MemoryData: Dictionary<MemoryResponse>
}

interface MonitorConfig {
	ID: string,
	FriendlyName: string,
	Type: MonitorType,
}

interface ConfigurationResponse {
    Configuration: Array<MonitorConfig>
}