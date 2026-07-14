const urlParams = new URLSearchParams(window.location.search);
const myParam = urlParams.get('id');

function FetchConfig(setupFinished: Function) {
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

function SetupAfterConfigurationRetrieved2() {
	console.log(myParam)
}
FetchConfig(SetupAfterConfigurationRetrieved2)