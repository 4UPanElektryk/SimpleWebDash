var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var DataResponseType;
(function (DataResponseType) {
    DataResponseType[DataResponseType["Success"] = 0] = "Success";
    DataResponseType[DataResponseType["Warning"] = 1] = "Warning";
    DataResponseType[DataResponseType["Error"] = 2] = "Error";
    DataResponseType[DataResponseType["Loading"] = 3] = "Loading";
})(DataResponseType || (DataResponseType = {}));
var LoadingDict = [];
const timespan = document.getElementById("period");
timespan.addEventListener("change", () => {
    location.reload();
});
google.charts.load('current', { packages: ['corechart'] });
google.charts.setOnLoadCallback(SetupAfterGoogleChartsLoad);
var Temps;
let SubscribedHTTP = [];
let SubscribedIP = [];
function SensorSimpleSet(obj, type, msg) {
    let icon = document.getElementById(`${obj}-icon`);
    let text = document.getElementById(`${obj}-msg`);
    let objj = document.getElementById(obj);
    icon === null || icon === void 0 ? void 0 : icon.classList.remove("fa-check", "fa-exclamation-triangle", "fa-times", "fa-refresh", "fa-spin");
    objj === null || objj === void 0 ? void 0 : objj.classList.remove("s-ok", "s-warning", "s-error");
    if (type == DataResponseType.Success) {
        icon === null || icon === void 0 ? void 0 : icon.classList.add("fa-check");
        objj === null || objj === void 0 ? void 0 : objj.classList.add("s-ok");
    }
    else if (type == DataResponseType.Warning) {
        icon === null || icon === void 0 ? void 0 : icon.classList.add("fa-exclamation-triangle");
        objj === null || objj === void 0 ? void 0 : objj.classList.add("s-warning");
    }
    else if (type == DataResponseType.Error) {
        icon === null || icon === void 0 ? void 0 : icon.classList.add("fa-times");
        objj === null || objj === void 0 ? void 0 : objj.classList.add("s-error");
    }
    else {
        icon === null || icon === void 0 ? void 0 : icon.classList.add("fa-refresh", "fa-spin");
    }
    text.innerText = msg;
}
function CheckIpEndpointAndSet(ip, obj) {
    fetch(`/api/ipstatus?ip=${ip}&t=${timespan.value}`).then((x) => x.json()).then((x) => {
        let res = x;
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
function CheckHttpEndpointAndSet(id, obj) {
    fetch(`/api/httpstatus?id=${id}&t=${timespan.value}`).then((x) => x.json()).then((x) => {
        let res = x;
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
function CheckTemperatureEndpointAndSet(id) {
    if (LoadingDict.includes(id)) {
        delete LoadingDict[LoadingDict.indexOf(id)];
    }
    GetTempsOfServer("192.168.10.10").then((Hydrogen) => {
        if (Hydrogen.Type == DataResponseType.Error) {
            if (!LoadingDict.includes(id)) {
                LoadingDict.push(id);
            }
        }
        else {
            GetTempsOfServer("192.168.10.11").then((Helium) => {
                if (Helium.Type == DataResponseType.Error) {
                    if (!LoadingDict.includes(id)) {
                        LoadingDict.push(id);
                    }
                }
                else {
                    GetTempsOfServer("192.168.10.12").then((Lithium) => {
                        if (Lithium.Type == DataResponseType.Error) {
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
                            let values = [Hydrogen, Helium, Lithium];
                            let dominant = 0;
                            for (let i = 0; i < values.length; i++) {
                                if (values[i].Data.Times.length == Math.max(values[0].Data.Times.length, values[1].Data.Times.length, values[2].Data.Times.length)) {
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
            });
        }
    });
}
function GetTempsOfServer(host) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const response = yield fetch(`/api/tempstats?ip=${host}&t=${timespan.value}`, { signal: AbortSignal.timeout(1500) });
            const times = yield response.json();
            return times;
        }
        catch (_a) {
            return new Promise((resolve, reject) => {
                resolve({
                    Type: DataResponseType.Error,
                    Message: "No Response From Server",
                    Data: null
                });
            });
        }
    });
}
function SubscribeIpEndpoint(ip, obj) {
    LoadingDict.push(obj);
    SubscribedIP.push({ obj: obj, data: ip });
}
function SubscribeHttpEndpoint(id, obj) {
    LoadingDict.push(obj);
    SubscribedHTTP.push({ obj: obj, data: id });
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
    };
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
setInterval(() => {
    SubscribedIP.forEach(element => {
        CheckIpEndpointAndSet(element.data, element.obj);
    });
    SubscribedHTTP.forEach(element => {
        CheckHttpEndpointAndSet(element.data, element.obj);
    });
    if (Temps != null) {
        CheckTemperatureEndpointAndSet("Temperature");
    }
}, 10000);
