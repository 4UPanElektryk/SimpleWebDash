enum DataResponseType {
	Success,
	Warning,
	Error,
	Loading,
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

var LoadingDict: Array<string> = [];

const timespan = document.getElementById("period") as HTMLSelectElement;
timespan.addEventListener("change", () => {
	location.reload();
});
google.charts.load('current', { packages: ['corechart'] });
google.charts.setOnLoadCallback(SetupAfterGoogleChartsLoad);
var Temps: google.visualization.DataTable;

function SensorSimpleSet(obj: string, type: DataResponseType, msg: string,) {
	let icon = document.getElementById(`${obj}-icon`);
	let text = document.getElementById(`${obj}-msg`);
	let objj = document.getElementById(obj);
	icon?.classList.remove("fa-check", "fa-exclamation-triangle", "fa-times", "fa-refresh", "fa-spin");
	objj?.classList.remove("s-ok", "s-warning", "s-error");
	if (type == DataResponseType.Success) {
		icon?.classList.add("fa-check");
		objj?.classList.add("s-ok");
	} else if (type == DataResponseType.Warning) {
		icon?.classList.add("fa-exclamation-triangle");
		objj?.classList.add("s-warning");
	}
	else if (type == DataResponseType.Error) {
		icon?.classList.add("fa-times");
		objj?.classList.add("s-error");
	}
	else {
		icon?.classList.add("fa-refresh", "fa-spin")
	}
	text!.innerText = msg;
}
function CheckIpEndpointAndSet(ip: string, obj: string) {
	fetch(`/api/ipstatus?ip=${ip}&t=${timespan.value}`).then((x) => x.json()).then((x) => {
		let res = x as ServerDataResponse<IpEndpointResponseData>;
		if (LoadingDict.includes(obj)) {
			delete LoadingDict[LoadingDict.indexOf(obj)];
		}
		if (res.Type == DataResponseType.Success) {
			SensorSimpleSet(obj, res.Type, `Min: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\n`);
		}
		else if (res.Type == DataResponseType.Warning) {
			SensorSimpleSet(obj, res.Type, `${res.Message}\nMin: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\nFail: ${res.Data.Timeouts} / ${res.Data.Total}`);
		}
		else {
			SensorSimpleSet(obj, res.Type, res.Message);
		}
	}).catch((x) => {
		if (!LoadingDict.includes(obj)) {
			LoadingDict.push(obj);
		}
	});
}
function CheckHttpEndpointAndSet(id: string, obj: string) {
	fetch(`/api/httpstatus?id=${id}&t=${timespan.value}`).then((x) => x.json()).then((x) => {
		let res = x as ServerDataResponse<IpEndpointResponseData>;
		if (LoadingDict.includes(obj)) {
			delete LoadingDict[LoadingDict.indexOf(obj)];
		}
		if (res.Type == DataResponseType.Success) {
			SensorSimpleSet(obj, res.Type, `Min: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\n`);
		}
		else if (res.Type == DataResponseType.Warning) {
			SensorSimpleSet(obj, res.Type, `${res.Message}\nMin: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\nFail: ${res.Data.Timeouts} / ${res.Data.Total}`);
		}
		else {
			SensorSimpleSet(obj, res.Type, res.Message);
		}
	}).catch((x) => {
		if (!LoadingDict.includes(obj)) {
			LoadingDict.push(obj);
		}
	});
}
function CheckTemperatureEndpointAndSet(id: string) {
	if (LoadingDict.includes(id)) {
		delete LoadingDict[LoadingDict.indexOf(id)];
	}
	let Hydrogen = GetTempsOfServer("192.168.10.10");
	let Helium = GetTempsOfServer("192.168.10.11");
	let Lithium = GetTempsOfServer("192.168.10.12");
	Promise.all([Hydrogen, Helium, Lithium]).then((values) => {
		if (values[0].Type == DataResponseType.Error && values[1].Type == DataResponseType.Error && values[2].Type == DataResponseType.Error) {
			if (!LoadingDict.includes(id)) {
				LoadingDict.push(id);
			}
		}
		else {
			Temps = new google.visualization.DataTable();
			Temps.addColumn('datetime', 'Time');
			Temps.addColumn('number', 'Hydrogen °C');
			Temps.addColumn('number', 'Helium °C');
			Temps.addColumn('number', 'Lithium °C');
			//console.log(values);
			let dominant = 0; 
			for (let i = 0; i < values.length; i++){
				if(values[i].Data.Times.length == Math.max(values[0].Data.Times.length, values[1].Data.Times.length, values[2].Data.Times.length)){
					dominant = i;
				}
			}
			for (let i = 0; i < values[dominant].Data.Times.length; i++) {
				Temps.addRows([
					[
						new Date(values[dominant].Data.Times[i] * 1000),
						values[0].Data.Temps[i],
						values[1].Data.Temps[i],
						values[2].Data.Temps[i]
					]
				]);
			}
			drawChart();
			SensorSimpleSet(id, DataResponseType.Success, "");
			//console.log(`Temps: ${values[0].Data.Temps.values[0]} ${values[1].Data.Temps.values[0]} ${values[2].Data.Temps.values[0]}`)
		}
	});
}
async function GetTempsOfServer(host: string): Promise<ServerDataResponse<TemperatureEndpointResponseData>> {
	try {
		const response = await fetch(`/api/tempstats?ip=${host}&t=${timespan.value}`, { signal: AbortSignal.timeout(500) });
		const times = await response.json() as ServerDataResponse<TemperatureEndpointResponseData>;
		return times;
	}
	catch {
		return new Promise<ServerDataResponse<TemperatureEndpointResponseData>>((resolve, reject) => {
			resolve({
				Type: DataResponseType.Error,
				Message: "No Response From Server",
				Data: null
			} as ServerDataResponse<TemperatureEndpointResponseData>)
		});
	}
}

function SubscribeIpEndpoint(ip: string, obj: string) {
	LoadingDict.push(obj);
	CheckIpEndpointAndSet(ip, obj);
	setInterval(() => {
		CheckIpEndpointAndSet(ip, obj);
	}, 10000);
}
function SubscribeTemperatureEndpoint(obj: string) {
	LoadingDict.push(obj);
	CheckTemperatureEndpointAndSet(obj);
	setInterval(() => {
		CheckTemperatureEndpointAndSet(obj);
	}, 10000);
}
function SubscribeHttpEndpoint(id: string, obj: string){
	LoadingDict.push(obj);
	CheckHttpEndpointAndSet(id, obj);
	setInterval(() => {
		CheckHttpEndpointAndSet(id, obj);
	}, 10000);
}

SubscribeIpEndpoint("hole.lan", "PiHole");
SubscribeIpEndpoint("192.168.10.251", "Printer");
SubscribeIpEndpoint("192.168.10.252", "TrueNas");
SubscribeIpEndpoint("192.168.10.149", "VPN");
SubscribeHttpEndpoint("NVR", "NVR");
SubscribeHttpEndpoint("NextCloud", "NextCloud");

function SetupAfterGoogleChartsLoad() {
	Temps = new google.visualization.DataTable();
	Temps.addColumn('datetime', 'Time');
	Temps.addColumn('number', 'Hydrogen °C');
	Temps.addColumn('number', 'Helium °C');
	Temps.addColumn('number', 'Lithium °C');
	SubscribeTemperatureEndpoint("Temperature");
}
function drawChart() {
	let t = +timespan.value.substring(0,4) * 86400000 + +timespan.value.substring(5,7) * 3600000 + +timespan.value.substring(8,10) * 60000;
	const options = {
		title: 'Server Temperature',
		backgroundColor: 'transparent',
		legendTextStyle: { color: '#FFF' },
		titleTextStyle: { color: '#FFF' },
		hAxis: {
			title: 'Time',
			/*viewWindow: {
				min: new Date(Date.now() - t),
				max: new Date(Date.now())
				},*/
			gridlines: {
				count: -1,
				units: {
					days: { format: ['MMM dd'] },
					hours: { format: ['HH:mm', 'ha'] },
				}
			},
		},
		vAxis: {
			textStyle: { color: '#FFF' },
			minValue: 0,
			maxValue: 100,
		},
		legend: 'none',
		chartArea: {
			right: 10,
			top: 20,
			width: "90%",
			height: "90%"
		}
	} as google.visualization.LineChartOptions;
	// Draw
	const chart = new google.visualization.LineChart(document.getElementById('Temperature-chart'));
	chart.draw(Temps, options);
}
let cycle = 0;
setInterval(() => {
	if (cycle == 0) {
		LoadingDict.forEach(element => {
			SensorSimpleSet(element, DataResponseType.Loading, "Loading.");
		});
	}
	else if (cycle == 1) {
		LoadingDict.forEach(element => {
			SensorSimpleSet(element, DataResponseType.Loading, "Loading..");
		});
	}
	else if (cycle == 2) {
		LoadingDict.forEach(element => {
			SensorSimpleSet(element, DataResponseType.Loading, "Loading...");
		});
	}
	cycle++;
	cycle = cycle % 3;
}, 500);