// Node Health & Telemetry Server
package main

import (
	"bufio"
	"encoding/json"		// JSON for responses
	"flag"				// for the info flag
	"fmt"				// Printing
	"net/http"			// http server
	"os"				// File readinf
	"strconv"			// str to int
	"strings"			// String Modification
)

const versionstr = "1.0.0"

type MemInfo struct {
	TotalKB uint64 `json:"total_kb"`
	FreeKB uint64 `json:"free_kb"`
}

func GetTemps() []int {
	AllTemperatures :=[]int{}
	files, err := os.ReadDir("/sys/class/thermal/");
	if err != nil {
		panic(err)
	}
	for i := 0; i < len(files); i++ {
		filename := files[i].Name()
		if(strings.HasPrefix(filename, "thermal_zone")){
			data, err := os.ReadFile("/sys/class/thermal/" + filename + "/temp")
			if err != nil {
				panic(err)
			}
			temperature, err := strconv.Atoi(strings.TrimSuffix(string(data), "\n"))
			if err != nil {
				panic(err)
			}
			temperature = temperature / 1000
			AllTemperatures = append(AllTemperatures, temperature);
		}
		
	}
	return AllTemperatures
}

func GetMemory() MemInfo {
	var info MemInfo

	file, _ := os.Open("/proc/meminfo")
	defer file.Close()

	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		line := scanner.Text()

		fields := strings.Fields(line)
		if len(fields) < 2 {
			continue
		}

		switch fields[0] {
		case "MemTotal:":
			info.TotalKB, _ = strconv.ParseUint(fields[1], 10, 64)
		case "MemFree:":
			info.FreeKB, _ = strconv.ParseUint(fields[1], 10, 64)
		}
	}
	return info
}

func ApiTemperature(w http.ResponseWriter, req *http.Request) {
	w.Header().Add("Content-Type", "application/json")
	arr := GetTemps();
	jsonBytes, err := json.Marshal(arr)
	if err != nil {
		panic(err)
	}
	fmt.Fprintf(w, string(jsonBytes))
}

func ApiMemory(w http.ResponseWriter, req *http.Request) {
	w.Header().Add("Content-Type", "application/json")
	var data MemInfo
	data = GetMemory();
	jsonBytes, err := json.Marshal(data)
	if err != nil {
		panic(err)
	}
	fmt.Fprintf(w, string(jsonBytes))
}

func main() {
	version := flag.Bool("v", false, "shows version of the software")
	flag.Parse()
	if *version {
		fmt.Println("Node Health & Telemetry Server version " + versionstr)
		return;
	}
	http.HandleFunc("/api/temperatures", ApiTemperature)
	http.HandleFunc("/api/memory", ApiMemory)
	http.ListenAndServe(":" + os.Getenv("PORT"), nil)
}

