const math = require('mathjs');
const fs = require('fs');


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
	[0, 1, 1],
];

const MIDDLE_EDGES = [
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
]

//   0  1  2  3  4  5  6  7
const initialVertices = [
	[0, 0, 0, 0, 0, 0, 0, 0],
	[0, 0, 0, 1, 0, 0, 0, 0],
	[0, 0, 1, 1, 0, 0, 0, 0],
	[0, 0, 0, 1, 0, 0, 1, 0],
	[1, 1, 1, 0, 0, 0, 0, 0],
	[1, 1, 1, 1, 0, 0, 0, 0],
	[1, 1, 1, 0, 0, 0, 0, 1],
	[0, 1, 0, 1, 1, 0, 1, 0],
	[1, 1, 0, 1, 1, 0, 0, 0],
	[1, 1, 1, 0, 1, 0, 0, 0],
	[0, 0, 0, 1, 0, 1, 0, 0],
	[0, 0, 1, 1, 0, 1, 0, 0],
	[0, 0, 1, 0, 0, 1, 0, 1],
	[0, 1, 0, 1, 0, 1, 0, 1],
	[1, 1, 0, 1, 0, 1, 0, 0],
];

const initialEdges = [
	[0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
	[0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0],
	[0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0],
	[0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0],
	[0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0],
	[0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0],
	[0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1],
	[1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
	[0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1],
	[0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1],
	[0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0],
	[0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0],
	[0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1],
	[1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1],
	[0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0]
];

const initialTriangles = [
	[],
	[[0, 1, 2]],
	[[0, 3, 2], [0, 1, 3]],
	[[0, 1, 3], [2, 5, 4]],
	[[0, 4, 1], [1, 4, 2], [2, 4, 3]],
	[[0, 3, 1], [1, 3, 2]],
	[[0, 4, 1], [1, 4, 2], [2, 4, 3], [5, 7, 6]],
	[[0, 1, 5], [2, 3, 7], [9, 6, 10], [4, 8, 11]],
	[[0, 2, 1], [1, 2, 4], [1, 4, 3], [4, 5, 3]],
	[[0, 5, 1], [0, 2, 5], [0, 3, 2], [4, 5, 2]],
	[[2, 5, 4], [0, 1, 3]],
	[[0, 1, 4], [0, 4, 3], [2, 6, 5]],
	[[0, 1, 3], [4, 8, 7], [2, 6, 5]],
	[[0, 2, 4], [4, 1, 5], [2, 7, 6], [7, 2, 3]],
	[[0, 5, 1], [1, 5, 2], [1, 2, 3], [2, 5, 4]]
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