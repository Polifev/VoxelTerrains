const { config } = require("process");

const specialCases = [
	0,
	0,
	0,
	10,
	20,
	10,
	20,
	0,
	11,
	12,
	20,
	10,
	20,
	30,
	41,
	42,
	0,
	0,
	11,
	12,
	20,
	0,
	11,
	12,
	20,
	10,
	20,
	30,
	40,
	51,
	52,
	0
];

const initialVertices = [
	[0, 0, 0, 0, 0, 0, 0, 0], //0
	[0, 0, 0, 1, 0, 0, 0, 0], //1
	[0, 0, 1, 1, 0, 0, 0, 0], //2
	[0, 0, 0, 1, 0, 0, 1, 0], //3.1
	[0, 0, 0, 1, 0, 0, 1, 0], //3.2
	[0, 0, 0, 1, 0, 1, 0, 0], //4.1
	[0, 0, 0, 1, 0, 1, 0, 0], //4.2
	[1, 0, 1, 0, 0, 0, 0, 0], //5
	[0, 0, 1, 1, 0, 1, 0, 0], //6.1.1
	[0, 0, 1, 1, 0, 1, 0, 0], //6.1.2
	[0, 0, 1, 1, 0, 1, 0, 0], //6.2
	[0, 0, 1, 0, 0, 1, 0, 1], //7.1
	[0, 0, 1, 0, 0, 1, 0, 1], //7.2
	[0, 0, 1, 0, 0, 1, 0, 1], //7.3
	[0, 0, 1, 0, 0, 1, 0, 1], //7.4.1
	[0, 0, 1, 0, 0, 1, 0, 1], //7.4.2
	[1, 1, 1, 1, 0, 0, 0, 0], //8
	[1, 1, 0, 1, 1, 0, 0, 0], //9
	[0, 1, 0, 1, 0, 1, 0, 1], //10.1.1
	[0, 1, 0, 1, 0, 1, 0, 1], //10.1.2
	[0, 1, 0, 1, 0, 1, 0, 1], //10.2
	[1, 1, 0, 1, 0, 1, 0, 0], //11
	[1, 1, 1, 0, 0, 0, 0, 1], //12.1.1
	[1, 1, 1, 0, 0, 0, 0, 1], //12.1.2
	[1, 1, 1, 0, 0, 0, 0, 1], //12.2
	[0, 1, 0, 1, 1, 0, 1, 0], //13.1
	[0, 1, 0, 1, 1, 0, 1, 0], //13.2
	[0, 1, 0, 1, 1, 0, 1, 0], //13.3
	[0, 1, 0, 1, 1, 0, 1, 0], //13.4
	[0, 1, 0, 1, 1, 0, 1, 0], //13.5.1
	[0, 1, 0, 1, 1, 0, 1, 0], //13.5.2
	[1, 1, 1, 0, 1, 0, 0, 0]  //14
];

const initialTriangles = [
	[], //case 0
	[[2, 3, 7]], //case 1
	[[1, 3, 7], [1, 7, 6]], //case 2
	[[2, 3, 7], [6, 10, 9]], //case 3.1
	[[6, 2, 3], [3, 9, 6], [3, 10, 9], [3, 7, 10]], //case 3.2
	[[2, 3, 7], [5, 9, 8]], //case 4.1
	[[9, 7, 2], [9, 8, 7], [8, 3, 7], [8, 5, 3], [5, 2, 3], [5, 9, 2]], //case 4.2
	[[2, 6, 3], [6, 4, 3], [6, 5, 4]], //case 5
	[[1, 3, 7], [7, 6, 1], [9, 8, 5]], //case 6.1.1
	[[9, 7, 6], [9, 8, 7], [8, 3, 7], [8, 5, 3], [5, 1, 3], [5, 9, 1], [9, 6, 1]], //case 6.1.2
	[[9, 7, 6], [9, 8, 7], [8, 3, 7], [8, 5, 3], [5, 1, 3]], //case 6.2
	[[1, 2, 6], [7, 11, 10], [8, 5, 9]], //case 7.1
	[[1, 2, 6], [10, 9, 5], [10, 5, 7], [8, 11, 7], [8, 7, 5]], //case 7.2
	[[9, 6, 12], [6, 2, 12], [12, 2, 1], [12, 1, 5], [12, 5, 8], [12, 8, 11], [12, 11, 7], [7, 10, 12], [10, 9, 12]], //case 7.3
	[[9, 10, 6], [2, 5, 1], [2, 11, 5], [2, 7, 11], [11, 8, 5]], //case 7.4.1
	[[10, 2, 6], [10, 7, 2], [10, 11, 7], [11, 10, 8], [8, 10, 9], [8, 9, 5], [9, 6, 5], [6, 1, 5], [6, 2, 1]], //case 7.4.2
	[[5, 7, 6], [5, 4, 7]], //case 8
	[[2, 1, 7], [7, 1, 11], [11, 1, 5], [11, 5, 8]], //case 9
	[[10, 2, 3], [10, 3, 11], [9, 8, 0], [9, 0, 1]], //case 10.1.1
	[[9, 10, 2], [9, 2, 1], [1, 2, 0], [0, 2, 3], [0, 3, 11], [0, 11, 8], [11, 10, 9], [11, 9, 8]], //case 10.1.2
	[[12, 2, 1], [12, 1, 9], [12, 9, 8], [12, 8, 0], [12, 0, 3], [12, 3, 11], [12, 11, 10], [12, 10, 2]], //case 10.2
	[[4, 7, 2], [4, 2, 9], [2, 1, 9], [4, 9, 8]], //case 11
	[[11, 10, 7], [4, 3, 2], [4, 2, 6], [4, 6, 5]], //case 12.1.1
	[[7, 2, 10], [2, 6, 10], [10, 6, 5], [10, 5, 11], [11, 5, 4], [11, 4, 3], [11, 3, 7], [7, 3, 2]], //case 12.1.2
	[[11, 10, 12], [10, 6, 12], [12, 6, 5], [12, 5, 3], [5, 4, 3], [12, 3, 2], [12, 2, 7], [12, 7, 11]], //case 12.2
	[[5, 0, 1], [7, 2, 3], [10, 9, 6], [8, 11, 4]], //case 13.1
	[[5, 0, 1], [7, 2, 3], [11, 6, 10], [11, 4, 6], [9, 6, 4], [9, 4, 8]], //case 13.2
	[[5, 0, 1], [10, 12, 6], [12, 9, 6], [12, 8, 9], [12, 4, 8], [12, 3, 4], [12, 2, 3], [12, 7, 2], [12, 11, 7], [12, 10, 11]], //case 13.3
	[[10, 12, 6], [12, 9, 6], [12, 8, 9], [12, 1, 8], [1, 5, 8], [12, 0, 1], [12, 4, 0], [12, 3, 4], [12, 2, 3], [12, 7, 2], [12, 11, 7], [12, 10, 11]], //case 13.4
	[[7, 2, 3], [9, 5, 8], [10, 11, 4], [10, 4, 0], [10, 0, 6], [6, 0, 1]], //case 13.5.1
	[[9, 5, 8], [10, 7, 2], [10, 2, 6], [6, 2, 1], [2, 0, 1], [2, 3, 0], [3, 4, 0], [3, 11, 4], [3, 7, 11], [7, 10, 11]], //case 13.5.2
	[[11, 3, 2], [11, 2, 5], [11, 5, 8], [5, 2, 6]] //case 14
];

function indexFromConfiguration(configuration) {
	let sum = 0;
	for (let i = 0; i < 8; i++) {
		sum += Math.pow(2, i) * configuration[i];
	}
	return sum;
}

function configurationFromIndex(index) {
	let configuration = [0, 0, 0, 0, 0, 0, 0, 0];
	for (let i = 0; i < 8; i++) {
		let power = Math.pow(2, 8 - i);
		if (index >= power) {
			configuration[8 - i] = 1;
			index -= power;
		}
	}
	return configuration;
}

function indexFullRotation(index) {
	let result = [];
	let configuration = configurationFromIndex(index);
	for (let x = 0; x < 4; x++) {
		for (let y = 0; y < 4; y++) {
			for (let z = 0; z < 4; z++) {
				result.push(indexFromConfiguration(configuration));
				configuration = rotateConfigurationZ(configuration);
			}
			configuration = rotateConfigurationY(configuration);
		}
		configuration = rotateConfigurationX(configuration);
	}
	return result;
}

function triangulationFullRotation(triangulation) {
	let result = [];
	for (let x = 0; x < 4; x++) {
		for (let y = 0; y < 4; y++) {
			for (let z = 0; z < 4; z++) {
				result.push(triangulation);
				triangulation = rotateTriangulationZ(triangulation);
			}
			triangulation = rotateTriangulationY(triangulation);
		}
		triangulation = rotateTriangulationX(triangulation);
	}
	return result;
}

function rotateConfigurationX(configuration) {
	let rotated = [0, 0, 0, 0, 0, 0, 0, 0];
	rotated[0] = configuration[3];
	rotated[1] = configuration[2];
	rotated[2] = configuration[6];
	rotated[3] = configuration[7];

	rotated[4] = configuration[0];
	rotated[5] = configuration[1];
	rotated[6] = configuration[5];
	rotated[7] = configuration[4];
	return rotated;
}
function rotateConfigurationY(configuration) {
	let rotated = [0, 0, 0, 0, 0, 0, 0, 0];
	rotated[0] = configuration[1];
	rotated[1] = configuration[2];
	rotated[2] = configuration[3];
	rotated[3] = configuration[0];

	rotated[4] = configuration[5];
	rotated[5] = configuration[6];
	rotated[6] = configuration[7];
	rotated[7] = configuration[4];
	return rotated;
}
function rotateConfigurationZ(configuration) {
	let rotated = [0, 0, 0, 0, 0, 0, 0, 0];
	rotated[0] = configuration[4];
	rotated[1] = configuration[0];
	rotated[2] = configuration[3];
	rotated[3] = configuration[7];

	rotated[4] = configuration[5];
	rotated[5] = configuration[1];
	rotated[6] = configuration[2];
	rotated[7] = configuration[6];
	return rotated;
}
function invertIndex(index) {
	let configuration = configurationFromIndex(index);
	let result = [1, 1, 1, 1, 1, 1, 1, 1];
	for (let i = 0; i < 8; i++) {
		if (configuration[i] == 1) {
			result[i] = 0;
		}
	}
	return indexFromConfiguration(result);
}
function invertTriangulation(triangulation) {
	let result = [...triangulation];
	result.map(triangle => {
		let temp = triangle[1];
		triangle[1] = triangle[2];
		triangle[2] = temp;
		return triangle;
	});
	return result;
}

function rotateTriangulationX(triangulation) {
	let table = [5, 9, 6, 1, 0, 8, 10, 2, 4, 11, 7, 3, 12];
	let result = [...triangulation];
	result.map(triangle => {
		triangle[0] = table[triangle[0]];
		triangle[1] = table[triangle[1]];
		triangle[2] = table[triangle[2]];
	});
	return result;
}

function rotateTriangulationY(triangulation) {
	let table = [3, 0, 1, 2, 7, 4, 5, 6, 11, 8, 9, 10, 12];
	let result = [...triangulation];
	result.map(triangle => {
		triangle[0] = table[triangle[0]];
		triangle[1] = table[triangle[1]];
		triangle[2] = table[triangle[2]];
		return triangle;
	});
	return result;
}

function rotateTriangulationZ(triangulation) {
	let table = [2, 6, 4, 7, 3, 1, 9, 11, 0, 5, 8, 4, 12];
	let result = [...triangulation];
	result.map(triangle => {
		triangle[0] = table[triangle[0]];
		triangle[1] = table[triangle[1]];
		triangle[2] = table[triangle[2]];
	});
	return result;
}

let finalMap = {};
for (let i = 0; i < initialVertices.length; i++) {
	let index = indexFromConfiguration(initialVertices[i]);
	let subIndex = specialCases[i];
	let fullIndex = `${index}_${subIndex}`;
	finalMap[fullIndex] = initialTriangles[i];
}

let firstIteration = [...Object.keys(finalMap)];
firstIteration.forEach(key => {
	let parts = key.split("_");
	let index = parts[0];
	let subIndex = parts[1];
	let triangulation = finalMap[key];

	let triangulations = triangulationFullRotation(triangulation);
	let indices = indexFullRotation(index);


	for (let i = 0; i < triangulations.length; i++) {
		let fullIndex = `${indices[i]}_${subIndex}`;
		finalMap[fullIndex] = triangulations[i];
	}
});

Object.keys(finalMap)
	.sort((a, b) => parseFloat(a.replace('_', '.')) - parseFloat(b.replace('_', '.')))
	.forEach(key => console.log(key));

let secondIteration = [...Object.keys(finalMap)];
secondIteration.forEach(key => {
	let parts = key.split("_");
	let index = invertIndex(parts[0]);
	let subIndex = parts[1];
	let triangulation = invertTriangulation(finalMap[key]);

	let fullIndex = `${index}_${subIndex}`;
	finalMap[fullIndex] = triangulation;
});

