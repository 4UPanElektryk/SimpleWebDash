interface Dictionary<T> {
	[Key: string]: T;
}

interface ServerDataResponse<T> {
	Type: DataResponseType,
	Message: string,
	Data: T
}

interface IpEndpointResponseData {
	Min: number,
	Max: number,
	Avg: number,
	Timeouts: number,
	Total: number
}

interface TemperatureEndpointResponseData {
	Times: Array<number>,
	Temps: Array<number>,
	Min: number,
	Max: number,
    Avg: number
}

interface CombinedTemperatureEndpointResponseData {
	Nodes: Dictionary<string>,
	Temperatures: Dictionary<TemperatureEndpointResponseData>
}

interface MemoryEndpointResponseData {
	Times: Array<number>;
	total_kb: Array<number>;
	used_kb: Array<number>;
	Avg: number;
	Max: number;
	Min: number;
}

interface CombinedMemoryEndpointResponseData {
	MemoryData: Dictionary<MemoryEndpointResponseData>
}

interface MonitorConfig {
	ID: string,
	FriendlyName: string,
	Type: MonitorType,
}

interface ConfigurationEndpointResponseData {
    Configuration: Array<MonitorConfig>
}