const math = require('mathjs');
const fs = require('fs');

//Correlation simple - advance
//				0 		0
//				1 		1
//				2 		2
//				3 		3
//				4 		5
//				5 		8
//				6 		12
//				7 		13
//				8 		9
//				9		14
//				10		4
//				11		6
//				12		7
//				13		10
//				14		11


// =========
// Base data
// =========
const CORNERS = [
	[0, 0, 0],
	[1, 0, 0],
	[1, 0, 1],
	[0, 0, 1],
	[0, 1, 0],
	[1, 1, 0],
	[1, 1, 1],
	[0, 1, 1]
];

const MIDDLE_EDGES = 
[
	[0.5, 0, 0],
	[1, 0, 0.5],
	[0.5, 0, 1],
	[0, 0, 0.5],

	[0, 0.5, 0],
	[1, 0.5, 0],
	[1, 0.5, 1],
	[0, 0.5, 1],

	[0.5, 1, 0],
	[1, 1, 0.5],
	[0.5, 1, 1],
	[0, 1, 0.5],
	[0.5, 0.5, 0.5]
]

//   0  1  2  3  4  5  6  7
const initialVertices = [
	[0, 0, 0, 0,    0, 0, 0, 0], //0
	[0, 0, 0, 1,    0, 0, 0, 0], //1
	[0, 0, 1, 1,    0, 0, 0, 0], //2
	[0, 0, 0, 1,    0, 0, 1, 0], //3.1
	[0, 0, 0, 1,    0, 0, 1, 0], //3.2
	[0, 0, 0, 1,    0, 1, 0, 0], //4.1
	[0, 0, 0, 1,    0, 1, 0, 0], //4.2
	[1, 0, 1, 0,    0, 0, 0, 0], //5
	[0, 0, 1, 1,    0, 1, 0, 0], //6.1.1
	[0, 0, 1, 1,    0, 1, 0, 0], //6.1.2
	[0, 0, 1, 1,    0, 1, 0, 0], //6.2
	[0, 0, 1, 0,    0, 1, 0, 1], //7.1
	[0, 0, 1, 0,    0, 1, 0, 1], //7.2
	[0, 0, 1, 0,    0, 1, 0, 1], //7.3
	[0, 0, 1, 0,    0, 1, 0, 1], //7.4.1
	[0, 0, 1, 0,    0, 1, 0, 1], //7.4.2
	[1, 1, 1, 1,    0, 0, 0, 0], //8
	[1, 1, 0, 1,    1, 0, 0, 0], //9
	[0, 1, 0, 1,    0, 1, 0, 1], //10.1.1
	[0, 1, 0, 1,    0, 1, 0, 1], //10.1.2
	[0, 1, 0, 1,    0, 1, 0, 1], //10.2
	[1, 1, 0, 1,    0, 1, 0, 0], //11
	[1, 1, 1, 0,    0, 0, 0, 1], //12.1.1
	[1, 1, 1, 0,    0, 0, 0, 1], //12.1.2
	[1, 1, 1, 0,    0, 0, 0, 1], //12.2
	[0, 1, 0, 1,    1, 0, 1, 0], //13.1
	[0, 1, 0, 1,    1, 0, 1, 0], //13.2
	[0, 1, 0, 1,    1, 0, 1, 0], //13.3
	[0, 1, 0, 1,    1, 0, 1, 0], //13.4
	[0, 1, 0, 1,    1, 0, 1, 0], //13.5.1
	[0, 1, 0, 1,    1, 0, 1, 0], //13.5.2
	[1, 1, 1, 0,    1, 0, 0, 0]  //14
];   

const initialEdges = [
	[0, 0, 0, 0,	 0, 0, 0, 0,	 0, 0, 0, 0,	 0], //0
	[0, 0, 1, 1,	 0, 0, 0, 1,	 0, 0, 0, 0,	 0], //1
	[0, 1, 0, 1,	 0, 0, 1, 1,	 0, 0, 0, 0,	 0], //2
	[0, 0, 1, 1,	 0, 0, 1, 1,	 0, 1, 1, 0,	 0], //3.1
	[0, 0, 1, 1,	 0, 0, 1, 1,	 0, 1, 1, 0,	 0], //3.2
	[0, 0, 1, 1,	 0, 1, 0, 1,	 1, 1, 0, 0,	 0], //4.1
	[0, 0, 1, 1,	 0, 1, 0, 1,	 1, 1, 0, 0,	 0], //4.2
	[0, 0, 1, 1,	 1, 1, 1, 0,	 0, 0, 0, 0,	 0], //5
	[0, 1, 0, 1,	 0, 1, 1, 1,	 1, 1, 0, 0,	 0], //6.1.1
	[0, 1, 0, 1,	 0, 1, 1, 1,	 1, 1, 0, 0,	 0], //6.1.2
	[0, 1, 0, 1,	 0, 1, 1, 1,	 1, 1, 0, 0,	 0], //6.2
	[0, 1, 1, 0,	 0, 1, 1, 1,	 1, 1, 1, 1,	 0], //7.1
	[0, 1, 1, 0,	 0, 1, 1, 1,	 1, 1, 1, 1,	 0], //7.2
	[0, 1, 1, 0,	 0, 1, 1, 1,	 1, 1, 1, 1,	 1], //7.3
	[0, 1, 1, 0,	 0, 1, 1, 1,	 1, 1, 1, 1,	 0], //7.4.1
	[0, 1, 1, 0,	 0, 1, 1, 1,	 1, 1, 1, 1,	 0], //7.4.2
	[0, 0, 0, 0,	 1, 1, 1, 1,	 0, 0, 0, 0,	 0], //8
	[0, 1, 1, 0,	 0, 1, 0, 1,	 1, 0, 0, 1,	 0], //9
	[1, 1, 1, 1,	 0, 0, 0, 0,	 1, 1, 1, 1,	 0], //10.1.1
	[1, 1, 1, 1,	 0, 0, 0, 0,	 1, 1, 1, 1,	 0], //10.1.2
	[1, 1, 1, 1,	 0, 0, 0, 0,	 1, 1, 1, 1,	 1], //10.2
	[0, 1, 1, 0,	 1, 0, 0, 1,	 1, 1, 0, 0,	 0], //11
	[0, 0, 1, 1,	 1, 1, 1, 1,	 0, 0, 1, 1,	 0], //12.1.1
	[0, 0, 1, 1,	 1, 1, 1, 1,	 0, 0, 1, 1,	 0], //12.1.2
	[0, 0, 1, 1,	 1, 1, 1, 1,	 0, 0, 1, 1,	 1], //12.2
	[1, 1, 1, 1,	 1, 1, 1, 1,	 1, 1, 1, 1,	 0], //13.1
	[1, 1, 1, 1,	 1, 1, 1, 1,	 1, 1, 1, 1,	 0], //13.2
	[1, 1, 1, 1,	 1, 1, 1, 1,	 1, 1, 1, 1,	 1], //13.3
	[1, 1, 1, 1,	 1, 1, 1, 1,	 1, 1, 1, 1,	 1], //13.4
	[1, 1, 1, 1,	 1, 1, 1, 1,	 1, 1, 1, 1,	 0], //13.5.1
	[1, 1, 1, 1,	 1, 1, 1, 1,	 1, 1, 1, 1,	 0], //13.5.2
	[0, 0, 1, 1,	 0, 1, 1, 0,	 1, 0, 0, 1,	 0]  //14
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

// =========
// Algorithm
// =========

// Initialisation de la map avec les valeurs de départ.
let finalMap = {};
for (let i = 0; i < 15; i++) {
	let config = {
		vertices: verticesToVectors(initialVertices[i]),
		edges: edgesToVectors(initialEdges[i]),
		triangles: initialTriangles[i]
	};
	let key = vectorsToByte(config.vertices);
	finalMap[key] = config;
}

// Pour chaque clé de la map, inverser les vertices et réintroduire ensuite
let iterationTwoKeys = [...Object.keys(finalMap)];

for (let i = 0; i < iterationTwoKeys.length; i++) {
	let key = iterationTwoKeys[i];
	let initialConfig = finalMap[key];

	let invertedConfig = invertConfig(initialConfig);
	let invertedKey = vectorsToByte(invertedConfig.vertices);

	finalMap[invertedKey] = invertedConfig;
}

// Pour chaque clé de la map, effectuer toutes les rotations possibles et réintroduire chaque résultat
let iterationThreeKeys = [...Object.keys(finalMap)];

for (let i = 0; i < iterationThreeKeys.length; i++) {
	let key = iterationThreeKeys[i];
	let config = finalMap[key];

	for (let x = 0; x < 4; x++) {
		for (let y = 0; y < 4; y++) {
			for (let z = 0; z < 4; z++) {
				finalMap[vectorsToByte(config.vertices)] = config;
				config = rotateConfigZ(config);
			}
			config = rotateConfigY(config);
		}
		config = rotateConfigX(config);
	}
}


// Vérifier que chaque clé possède sa valeur
let missingCount = 0;
for (let i = 0; i < 255; i++) {
	if (finalMap[i] === undefined) {
		missingCount++;
		console.log('Missing key: ' + i);
	}
}
console.log('total missing: ' + missingCount);

// Export JSON
let json = JSON.stringify(finalMap);
fs.writeFileSync('./marchingCubesCases.json', json);

// Export CSV
let csv = encodeCsv(finalMap);
fs.writeFileSync('./marchingCubesCases.csv', csv);

// =========
// Functions
// =========

function invertConfig(config) {
	let vertices = [...config.vertices];
	let edges = [...config.edges];
	let triangles = [...config.triangles];

	let newVertices = [];

	// Ajouter chaque coin qui n'était pas dans la config initiale
	let myStrings = vertices.map(vecToString);
	let allStrings = CORNERS.map(vecToString);
	for (let i = 0; i < allStrings.length; i++) {
		if (!myStrings.includes(allStrings[i])) {
			newVertices.push(CORNERS[i]);
		}
	}

	// inverser les faces
	let newTriangles = [];
	triangles.forEach(triangle => {
		newTriangles.push([triangle[1], triangle[0], triangle[2]]);
	});

	return {
		vertices: newVertices,
		edges,
		triangles: newTriangles
	};
}

function rotateConfigX(config) {
	let vertices = [...config.vertices];
	let edges = [...config.edges];
	let triangles = [...config.triangles];

	// centrer l'ensemble des points
	vertices = vertices.map(vertice => vertice.map(coordinate => coordinate - 0.5));
	// effectuer la symétrie
	vertices = vertices.map(vertice => math.multiply(vertice, createMatrixRotationX())._data);
	// décentrer à nouveau
	vertices = vertices.map(vertice => vertice.map(coordinate => coordinate + 0.5));

	// centrer l'ensemble des points
	edges = edges.map(vertice => vertice.map(coordinate => coordinate - 0.5));
	// effectuer la symétrie
	edges = edges.map(vertice => math.multiply(vertice, createMatrixRotationX())._data);
	// décentrer à nouveau
	edges = edges.map(vertice => vertice.map(coordinate => coordinate + 0.5));

	return {
		vertices,
		edges,
		triangles
	};
}

function rotateConfigY(config) {
	let vertices = [...config.vertices];
	let edges = [...config.edges];
	let triangles = [...config.triangles];

	// centrer l'ensemble des points
	vertices = vertices.map(vertice => vertice.map(coordinate => coordinate - 0.5));
	// effectuer la symétrie
	vertices = vertices.map(vertice => math.multiply(vertice, createMatrixRotationY())._data);
	// décentrer à nouveau
	vertices = vertices.map(vertice => vertice.map(coordinate => coordinate + 0.5));

	// centrer l'ensemble des points
	edges = edges.map(vertice => vertice.map(coordinate => coordinate - 0.5));
	// effectuer la symétrie
	edges = edges.map(vertice => math.multiply(vertice, createMatrixRotationY())._data);
	// décentrer à nouveau
	edges = edges.map(vertice => vertice.map(coordinate => coordinate + 0.5));

	return {
		vertices,
		edges,
		triangles
	};
}

function rotateConfigZ(config) {
	let vertices = [...config.vertices];
	let edges = [...config.edges];
	let triangles = [...config.triangles];

	// centrer l'ensemble des points
	vertices = vertices.map(vertice => vertice.map(coordinate => coordinate - 0.5));
	// effectuer la symétrie
	vertices = vertices.map(vertice => math.multiply(vertice, CreateMatrixRotationZ())._data);
	// décentrer à nouveau
	vertices = vertices.map(vertice => vertice.map(coordinate => coordinate + 0.5));

	// centrer l'ensemble des points
	edges = edges.map(vertice => vertice.map(coordinate => coordinate - 0.5));
	// effectuer la symétrie
	edges = edges.map(vertice => math.multiply(vertice, CreateMatrixRotationZ())._data);
	// décentrer à nouveau
	edges = edges.map(vertice => vertice.map(coordinate => coordinate + 0.5));

	return {
		vertices,
		edges,
		triangles
	};
}

function createMatrixRotationX() {
	return math.matrix([
		[1, 0, 0],
		[0, 0, -1],
		[0, 1, 0]
	]);
}

function createMatrixRotationY() {
	return math.matrix([
		[0, 0, 1],
		[0, 1, 0],
		[-1, 0, 0]
	]);
}

function CreateMatrixRotationZ() {
	return math.matrix([
		[0, -1, 0],
		[1, 0, 0],
		[0, 0, 1]
	]);
}

function vectorsToByte(vectors) {
	const availableVectors = CORNERS;

	let sum = 0;
	let strings = vectors.map(vecToString);
	for (let i = 0; i < 8; i++) {
		let string = vecToString(availableVectors[i]);
		if (strings.includes(string)) {
			sum += math.pow(2, i);
		}
	}
	return sum;
}

function verticesToVectors(vertices) {
	const availableVectors = CORNERS;

	let vectors = [];
	for (let i = 0; i < 8; i++) {
		if (vertices[i] === 1) {
			vectors.push(availableVectors[i]);
		}
	}
	return vectors;
}

function edgesToVectors(edges) {
	const availableVectors = MIDDLE_EDGES;

	let vectors = [];
	for (let i = 0; i < 12; i++) {
		if (edges[i] === 1) {
			vectors.push(availableVectors[i]);
		}
	}
	return vectors;
}

function vecToString(vec) {
	return `(${vec[0]}:${vec[1]}:${vec[2]})`;
}

function encodeCsv(map) {
	let result = '';

	Object.keys(map).forEach(key => {
		let line = '';
		let config = map[key];
		config.edges.forEach(vector => {
			line += vecToString(vector) + ',';
		});
		line = line.substring(0, line.length - 1);
		line += ';';

		config.triangles.forEach(triangle => triangle.forEach(vertexIndex => {
			line += vertexIndex + ',';
		}));
		line = line.substring(0, line.length - 1);

		result += line + '\n';
	});
	return result;
}