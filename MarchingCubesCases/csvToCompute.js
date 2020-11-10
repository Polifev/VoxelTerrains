const fs = require('fs');

let csv = fs.readFileSync('./MarchingCubesCases.csv', "utf-8");
let lines = csv.split('\n');
let i = 0;

let outputLines = "static const float3 configurations[256][15] = {\n";

for (let i = 0; i < 256; i++) {
	let line = lines[i];

	let parts = line.split(';');
	if (parts.length != 2)
		parts = ['', ''];

	let vectors = parts[0].split(',');
	let triangles = parts[1].split(',');

	vectors = vectors.map(vec => "float3" + vec.replace(/:/, ',').replace(/:/, ','));

	let outputLine = "\t{";
	for (let j = 0; j < 15; j++) {
		let vector = vectors[triangles[j]]
		if (vector != undefined) {
			outputLine += vector + ',';
		} else {
			outputLine += "float3(-1.0,-1.0,-1.0),";
		}
	}
	outputLine = outputLine.substring(0, outputLine.length - 1);
	outputLine += "},\n";
	outputLines += outputLine;
}

outputLines = outputLines.substring(0, outputLines.length - 2);
outputLines += "\n};";

fs.writeFileSync("marchingCubesTable.compute", outputLines);