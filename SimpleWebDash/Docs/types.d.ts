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
}
interface CombinedTemperatureEndpointResponseData {
	Nodes: Dictionary<string>,
	Temperatures: Dictionary<TemperatureEndpointResponseData>
}
