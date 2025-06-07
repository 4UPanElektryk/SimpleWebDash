import { createServer } from 'node:http';
import { readdirSync, readFileSync } from 'fs';

function GetTemps(){
	let filestoread = readdirSync('/sys/class/thermal/');
	let temps = [];
	for (let i = 0; i < filestoread.length; i++){
		if (filestoread[i].startsWith("thermal_zone")){
			temps.push(+readFileSync(`/sys/class/thermal/${filestoread[i]}/temp`, 'utf8') / 1000);
		}
	}
	return temps;
}


const hostname = '0.0.0.0';
const port = 3000;

const server = createServer((req, res) => {
	if(req.method == "GET" && req.url == "/temps"){
		res.statusCode = 200;
		res.setHeader('Content-Type', 'application/json');
		res.setHeader('Access-Control-Allow-Origin', '*');
		res.end(JSON.stringify(GetTemps()));
	}
	else{
		res.statusCode = 404;
		res.setHeader('Content-Type', 'text/html');
		res.setHeader('Access-Control-Allow-Origin', '*');
		res.end("<!doctype html><head><title>404 not found</title></head><body><h1>404 File Not Found</h1></body></html>");
	}
});

server.listen(port, hostname, () => {
	console.log(`Server running at http://${hostname}:${port}/`);
});