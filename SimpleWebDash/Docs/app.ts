//#region Type Definitions
enum DataResponseType {
	Success,
	Warning,
	Error,
	Loading,
}
enum MonitorType {
	IP = 0,
	HTTP = 1,
	GAS = 2,
}
class SimpleSensor {
	name: string;
	object: HTMLElement;
	isLoading: boolean = true;
    UpdateData: (sender: SimpleSensor) => void;
	SetDisplay(type: DataResponseType, msg: string): void {
		let icon = this.object.querySelector(`#${this.name}-icon`) as HTMLElement;
		let text = this.object.querySelector(`#${this.name}-msg`) as HTMLElement;
		icon?.classList.remove("fa-check", "fa-exclamation-triangle", "fa-times", "fa-refresh", "fa-spin");
		this.object?.classList.remove("s-ok", "s-warning", "s-error");
		this.isLoading = false;
		if (type == DataResponseType.Success) {
			icon?.classList.add("fa-check");
			this.object?.classList.add("s-ok");
		} else if (type == DataResponseType.Warning) {
			icon?.classList.add("fa-exclamation-triangle");
			this.object?.classList.add("s-warning");
		}
		else if (type == DataResponseType.Error) {
			icon?.classList.add("fa-times");
			this.object?.classList.add("s-error");
		}
		else {
			icon?.classList.add("fa-refresh", "fa-spin")
			this.isLoading = true;
		}
		text!.innerText = msg;
	}
	PerformUpdate() {
        this.UpdateData(this);
	}
	constructor(name: string, displayText: string, parrent: HTMLElement) {
		this.name = name;
        if (parrent.classList.contains("sensor-set")) {
			this.object = document.getElementById(name);
			return;
        }
		this.object = document.createElement("div");
		this.object.classList.add("sensor-single");
		this.object.id = name;
		this.object.innerHTML = `
				<span class="big"><i class="fa" aria-hidden="true" id="${name}-icon"></i>${displayText}</span>
				<span class="small" id="${name}-msg"></span>
			`;
        parrent.appendChild(this.object);
	}
}
//#endregion
//#region Global Functions and Types
const timespan = document.getElementById("period") as HTMLSelectElement;
timespan.addEventListener("change", () => {
	location.reload();
});

let Sensors: Array<SimpleSensor> = [];
let current: HTMLElement;

function GetSensorGroup(): HTMLElement {
	if (Sensors.length % 3 == 0) {
		current = document.createElement("div");
		current.classList.add("sensor-group");
		document.getElementById("sensors-container").appendChild(current);
	}
	return current;
}

//Google Charts Setup
google.charts.load('current', { packages: ['corechart'] });
google.charts.setOnLoadCallback(SetupAfterGoogleChartsLoad);
var Temps: google.visualization.DataTable;
//#endregion

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
function CheckTemperatureEndpointAndSet(id: string) {
	const jsonData = GetTempsOfAllServers();
	jsonData.then((jsonData) => {
		/*if (jsonData.Type == DataResponseType.Error) {
			if (!LoadingDict.includes(id)) {
				LoadingDict.push(id);
			}
			return;
		}
		else if (LoadingDict.includes(id)) {
			delete LoadingDict[LoadingDict.indexOf(id)];
		}*/
		const tempData = jsonData.Data.Temperatures;
		const nodeNames = jsonData.Data.Nodes;
		const allTimestamps = new Set<number>();

		// Collect all unique timestamps
		for (const node in tempData) {
			tempData[node].Times.forEach(ts => allTimestamps.add(ts));
		}

		const sortedTimestamps = Array.from(allTimestamps).sort((a, b) => a - b);
		
		// Map timestamps to temperature data per node
		interface TimeRow {
			time: Date;
			[node: string]: Date | number | null;
		}
		const timestampMap: Record<number, TimeRow> = {};
		sortedTimestamps.forEach(ts => {
			timestampMap[ts] = { time: new Date(ts * 1000) };
		});
		// Fill in temperature readings
		for (const ip in tempData) {
			const label = nodeNames[ip] ?? ip;
			const times = tempData[ip].Times;
			const temps = tempData[ip].Temps;

			times.forEach((ts, idx) => {
				timestampMap[ts][label] = temps[idx];
			});
		}

		// Initialize Google DataTable
		const data = new google.visualization.DataTable();
		data.addColumn('datetime', 'Time');

		const labels = Object.values(nodeNames);
		labels.forEach(label => data.addColumn('number', label));

		const rows = sortedTimestamps.map(ts => {
			const row = [new Date(ts * 1000)] as (Date | number | null)[];
			labels.forEach(label => row.push(timestampMap[ts][label] ?? null));
			return row;
		});

		data.addRows(rows);
		Temps = data;
		drawChart();
		SensorSimpleSet(id, DataResponseType.Success, "");
	}).catch((x) => {
		/*if (!LoadingDict.includes(id)) {
			LoadingDict.push(id);
		}*/
	});
	/*if (LoadingDict.includes(id)) {
		delete LoadingDict[LoadingDict.indexOf(id)];
	}
	let All = GetTempsOfAllServers();
	All.then((values) => {
		if (values.Type == DataResponseType.Error) {
			if (!LoadingDict.includes(id)) {
				LoadingDict.push(id);
			}
		}
		else {
			Temps = new google.visualization.DataTable();
			Temps.addColumn('datetime', 'Time');
			for (let i = 0; i < Object.keys(values.Data.Nodes).length; i++) {
				Temps.addColumn('number', values.Data.Nodes[Object.keys(values.Data.Nodes)[i]] + " °C");
			}
			let dominant = 0; 
			for (let i = 0; i < Object.keys(values.Data.Temperatures).length; i++){
				if(values.Data.Temperatures[Object.keys(values.Data.Temperatures)[i]].Times.length == Math.max(...Object.values(values.Data.Temperatures).map(x => x.Times.length))){
					dominant = i;
				}
			}
			for (let i = 0; i < values.Data.Temperatures[Object.keys(values.Data.Temperatures)[dominant]].Times.length; i++) {
				let row: any[] = [new Date(values.Data.Temperatures[Object.keys(values.Data.Temperatures)[dominant]].Times[i] * 1000)];
				for (let j = 0; j < Object.keys(values.Data.Nodes).length; j++) {
					row.push(values.Data.Temperatures[Object.keys(values.Data.Temperatures)[j]].Temps[i]);
				}
				Temps.addRows([row]);
			}
			drawChart();
			SensorSimpleSet(id, DataResponseType.Success, "");
		}*/
	/*let Hydrogen = GetTempsOfServer("192.168.10.10");
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
	});*/
}
async function GetTempsOfAllServers(): Promise<ServerDataResponse<CombinedTemperatureResponse>> {
	try {
		const response = await fetch(`/api/fulltempstats?t=${timespan.value}`, { signal: AbortSignal.timeout(5000) });
		const times = await response.json() as ServerDataResponse<CombinedTemperatureResponse>;
		return times;
	}
	catch {
		return new Promise<ServerDataResponse<CombinedTemperatureResponse>>((resolve, reject) => {
			resolve({
				Type: DataResponseType.Error,
				Message: "No Response From Server",
				Data: null
			} as ServerDataResponse<CombinedTemperatureResponse>)
		});
	}
}
function SubscribeTemperatureEndpoint(obj: string) {
	CheckTemperatureEndpointAndSet(obj);
	setInterval(() => {
		CheckTemperatureEndpointAndSet(obj);
	}, 30000);
}
function SetupAfterGoogleChartsLoad() {
	Temps = new google.visualization.DataTable();
	SubscribeTemperatureEndpoint("Temperature");
}
function drawChart() {
	let t = +timespan.value.substring(0, 4) * 86400000 + +timespan.value.substring(5, 7) * 3600000 + +timespan.value.substring(8, 10) * 60000;
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
			title: 'Temperature °C',
			textStyle: { color: '#FFF' },
			minValue: 0,
			maxValue: 100,
		},
		interpolateNulls: true,
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
function CreateHttpChecker(id: string) {
	return function (sender: SimpleSensor) {
		fetch(`/api/httpstatus?id=${id}&t=${timespan.value}`).then((x) => x.json()).then((x) => {
			let res = x as ServerDataResponse<IpResponse>;
			if (res.Type == DataResponseType.Success) {
				sender.SetDisplay(res.Type, `Min: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\n`);
			}
			else if (res.Type == DataResponseType.Warning) {
				sender.SetDisplay(res.Type, `${res.Message}\nMin: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\nFail: ${res.Data.Timeouts} / ${res.Data.Total}`);
			}
			else {
				sender.SetDisplay(res.Type, res.Message);
			}
		}).catch((x) => {
            sender.isLoading = true;
		});
	}
}
function CreateIpChecker(ip: string) {
	return function (sender: SimpleSensor) {
		fetch(`/api/ipstatus?id=${ip}&t=${timespan.value}`).then((x) => x.json()).then((x) => {
			let res = x as ServerDataResponse<IpResponse>;
			if (res.Type == DataResponseType.Success) {
				sender.SetDisplay(res.Type, `Min: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\n`);
			}
			else if (res.Type == DataResponseType.Warning) {
				sender.SetDisplay(res.Type, `${res.Message}\nMin: ${res.Data.Min} ms\nMax: ${res.Data.Max} ms\nAvg: ${res.Data.Avg} ms\nFail: ${res.Data.Timeouts} / ${res.Data.Total}`);
			}
			else {
				sender.SetDisplay(res.Type, res.Message);
			}
		}).catch((x) => {
			sender.isLoading = true;
		});
	}
}

function FetchConfiguration(setupFinished: Function) {
	fetch(`/api/configuration`).then((x) => x.json()).then((x) => {
		let res = x as ServerDataResponse<ConfigurationResponse>;
		res.Data.Configuration.forEach((element) => {
			console.log(`Setting up sensor: ${element.FriendlyName} of type ${MonitorType[element.Type]} with ID: ${element.ID}`);
			if (element.Type == MonitorType.HTTP) {
				let httpSensor = new SimpleSensor(element.ID, element.FriendlyName, GetSensorGroup());
				httpSensor.UpdateData = CreateHttpChecker(element.ID);
				Sensors.push(httpSensor);
				console.log(`HTTP Sensor for ${element.FriendlyName} set up.`);
			}
			else if (element.Type == MonitorType.IP) {
				let ipSensor = new SimpleSensor(element.ID, element.FriendlyName, GetSensorGroup());
				ipSensor.UpdateData = CreateIpChecker(element.ID);
				Sensors.push(ipSensor);
                console.log(`IP Sensor for ${element.FriendlyName} set up.`);
			}
		});
        console.log("Configuration fetched and sensors set up.");
        setupFinished();
	}).catch((x) => {
		console.error("Failed to fetch configuration!\nMajor shit happend!\nHeads will Roll!");
		console.error(x);
		debugger;
		//window.location.reload();
	});
}

function SetupAfterConfigurationRetrieved() {
	Sensors.forEach((sensor) => {
		sensor.PerformUpdate();
	});
	setInterval(() => {
		Sensors.forEach((sensor) => {
			sensor.PerformUpdate();
		});
	}, 10000);
}
FetchConfiguration(SetupAfterConfigurationRetrieved)
let cycle = 0;
setInterval(() => {
	let str = "";
	if (cycle == 0) {
		str = "Loading.";
	}
	else if (cycle == 1) {
		str = "Loading..";
	}
	else if (cycle == 2) {
        str = "Loading...";
	}
	Sensors.forEach(element => {
		if (element.isLoading) {
			element.SetDisplay(DataResponseType.Loading, str);
		}
	});
	cycle++;
	cycle = cycle % 3;
}, 500);