const math = require('mathjs');
const fs = require('fs');

const tetrahedrons = [
	[0, 1, 2, 4],
	[0, 3, 2, 4],
	[3, 7, 4, 2],
	[6, 7, 4, 2],
	[5, 6, 2, 4],
	[5, 1, 2, 4]
];

const indexes = [
	[0, 0, 0, 0], // 0
	[1, 0, 0, 0], // 1
	[0, 1, 0, 0], // 2
	[1, 1, 0, 0], // 3
	[0, 0, 1, 0], // 4
	[1, 0, 1, 0], // 5
	[0, 1, 1, 0], // 6
	[1, 1, 1, 0], // 7
	[0, 0, 0, 1], // 8
	[1, 0, 0, 1], // 9
	[0, 1, 0, 1], // 10
	[1, 1, 0, 1], // 11
	[0, 0, 1, 1], // 12
	[1, 0, 1, 1], // 13
	[0, 1, 1, 1], // 14
	[1, 1, 1, 1], // 15
];

const vertices = [
	[], // 0
	[[0.5, 0, 0], [0.5, 0, 0.5], [0, 0.5, 0]], // 1
	[[0.5, 0, 0], [0.5, 0.5, 0], [1, 0, 0.5]], // 2
	[[0, 0.5, 0], [0.5, 0, 0.5], [0.5, 0.5, 0], [1, 0, 0.5]], // 3
	[[0.5, 0, 0.5], [1, 0, 0.5], [0.5, 0.5, 0.5]], // 4
	[[0.5, 0, 0], [1, 0, 0.5], [0, 0.5, 0], [0.5, 0.5, 0.5]], // 5
	[[0.5, 0, 0], [0.5, 0, 0.5], [0.5, 0.5, 0], [0.5, 0.5, 0.5]], // 6
	[[0, 0.5, 0], [0.5, 0.5, 0.5], [0.5, 0.5, 0]], // 7
	[[0, 0.5, 0], [0.5, 0.5, 0.5], [0.5, 0.5, 0]], // 8
	[[0.5, 0, 0], [0.5, 0, 0.5], [0.5, 0.5, 0], [0.5, 0.5, 0.5]], // 9
	[[0.5, 0, 0], [1, 0, 0.5], [0, 0.5, 0], [0.5, 0.5, 0.5]], // 10
	[[0.5, 0, 0.5], [1, 0, 0.5], [0.5, 0.5, 0.5]], // 11
	[[0, 0.5, 0], [0.5, 0, 0.5], [0.5, 0.5, 0], [1, 0, 0.5]], // 12
	[[0.5, 0, 0], [0.5, 0.5, 0], [1, 0, 0.5]], // 13
	[[0.5, 0, 0], [0.5, 0, 0.5], [0, 0.5, 0]], // 14
	[] // 15
];

const triangles = [
	[], // 0
	[[0, 1, 2]], // 1
	[[0, 1, 2]], // 2
	[[0, 2, 1], [1, 2, 3]], // 3
	[[0, 1, 2]], // 4
	[[0, 1, 2], [1, 3, 2]], // 5
	[[0, 2, 1], [1, 2, 3]], // 6
	[[0, 2, 1]], // 7
	[[0, 1, 2]], // 8
	[[0, 1, 2], [1, 3, 2]], // 9
	[[0, 2, 1], [1, 2, 3]], // 10
	[[0, 2, 1]], // 11
	[[0, 1, 2], [1, 3, 2]], // 12
	[[0, 2, 1]], // 13
	[[0, 2, 1]], // 14
	[] // 15
];


function coords1To2(vector) {
	let vec = [...vector];
	let rotYNonante = math.matrix([
		[0, 0, 1],
		[0, 1, 0],
		[-1, 0, 0]
	]);

	let symX = math.matrix([
		[-1, 0, 0],
		[0, 1, 0],
		[0, 0, 1]
	]);

	vec = math.multiply(vec, rotYNonante)._data;
	vec = math.multiply(vec, symX)._data;

	return vec;
}

function coords1To3(vector) {
	let vec = [...vector];
	let rotYCentQuatreVingt = math.matrix([
		[-1, 0, 0],
		[0, 1, 0],
		[0, 0, -1]
	]);

	let rotZNonante = math.matrix([
		[0, -1, 0],
		[1, 0, 0],
		[0, 0, 1]
	]);

	vec = math.multiply(vec, rotYCentQuatreVingt)._data;
	vec = math.multiply(vec, rotZNonante)._data;

	vec[2] = vec[2] + 1;

	return vec;
}
function coords1To4(vector) {
	let vec = [...vector];
	let rotXCentQuatreVingt = math.matrix([
		[1, 0, 0],
		[0, -1, 0],
		[0, 0, -1]
	]);

	let symX = math.matrix([
		[-1, 0, 0],
		[0, 1, 0],
		[0, 0, 1]
	]);

	vec = math.multiply(vec, rotXCentQuatreVingt)._data;
	vec = math.multiply(vec, symX)._data;

	vec[0] = vec[0] + 1;
	vec[1] = vec[1] + 1;
	vec[2] = vec[2] + 1;

	return vec;
}
function coords1To5(vector) {
	let vec = [...vector];
	let rotXMoinsNonante = math.matrix([
		[1, 0, 0],
		[0, 0, 1],
		[0, -1, 0]
	]);

	let rotYNonante = math.matrix([
		[0, 0, 1],
		[0, 1, 0],
		[-1, 0, 0]
	]);

	vec = math.multiply(vec, rotXMoinsNonante)._data;
	vec = math.multiply(vec, rotYNonante)._data;

	vec[0] += 1;
	vec[1] += 1;

	return vec;
}

function coords1To6(vector) {
	let vec = [...vector];
	let rotZNonante = math.matrix([
		[0, -1, 0],
		[1, 0, 0],
		[0, 0, 1],
	]);

	let symX = math.matrix([
		[-1, 0, 0],
		[0, 1, 0],
		[0, 0, 1]
	]);

	vec = math.multiply(vec, rotZNonante)._data;
	vec = math.multiply(vec, symX)._data;

	vec[0] += 1;
	vec[1] += 1;

	return vec;
}

function invertTriangles(triangles) {
	let tri = [];
	for (let i = 0; i < triangles.length; i++) {
		let tr = []
		tr.push(triangles[i][0]);
		tr.push(triangles[i][2]);
		tr.push(triangles[i][1]);
		tri.push(tr);
	}
	return tri;
}

function vectorToString(vector) {
	return `(${vector[0]}:${vector[1]}:${vector[2]})`;
}

function triangleToString(triangle) {
	return `${triangle[0]},${triangle[1]},${triangle[2]}`;
}


let finalString = "";
for (let i = 0; i < indexes.length; i++) {
	let vectors = vertices[i];
	let tris = invertTriangles(triangles[i]);
	for (let j = 0; j < vectors.length - 1; j++) {
		finalString += vectorToString(vectors[j]) + ",";
	}
	if (vectors.length != 0)
		finalString += vectorToString(vectors[vectors.length - 1]) + ";";


	for (let j = 0; j < tris.length - 1; j++) {
		finalString += triangleToString(tris[j]) + ",";
	}
	if (vectors.length != 0)
		finalString += triangleToString(tris[tris.length - 1])
	finalString += "\n";
}

for (let i = 0; i < indexes.length; i++) {
	let vectors = vertices[i];
	let tris = triangles[i];

	for (let j = 0; j < vectors.length - 1; j++) {
		finalString += vectorToString(coords1To2(vectors[j])) + ",";
	}
	if (vectors.length != 0)
		finalString += vectorToString(coords1To2(vectors[vectors.length - 1])) + ";";

	for (let j = 0; j < tris.length - 1; j++) {
		finalString += triangleToString(tris[j]) + ",";
	}
	if (vectors.length != 0)
		finalString += triangleToString(tris[tris.length - 1])
	finalString += "\n";
}

for (let i = 0; i < indexes.length; i++) {
	let vectors = vertices[i];
	let tris = invertTriangles(triangles[i]);

	for (let j = 0; j < vectors.length - 1; j++) {
		finalString += vectorToString(coords1To3(vectors[j])) + ",";
	}
	if (vectors.length != 0)
		finalString += vectorToString(coords1To3(vectors[vectors.length - 1])) + ";";

	for (let j = 0; j < tris.length - 1; j++) {
		finalString += triangleToString(tris[j]) + ",";
	}
	if (vectors.length != 0)
		finalString += triangleToString(tris[tris.length - 1]);
	finalString += "\n";
}

for (let i = 0; i < indexes.length; i++) {
	let vectors = vertices[i];
	let tris = triangles[i];

	for (let j = 0; j < vectors.length - 1; j++) {
		finalString += vectorToString(coords1To4(vectors[j])) + ",";
	}
	if (vectors.length != 0)
		finalString += vectorToString(coords1To4(vectors[vectors.length - 1])) + ";";

	for (let j = 0; j < tris.length - 1; j++) {
		finalString += triangleToString(tris[j]) + ",";
	}
	if (vectors.length != 0)
		finalString += triangleToString(tris[tris.length - 1]);
	finalString += "\n";
}

for (let i = 0; i < indexes.length; i++) {
	let vectors = vertices[i];
	let tris = invertTriangles(triangles[i]);

	for (let j = 0; j < vectors.length - 1; j++) {
		finalString += vectorToString(coords1To5(vectors[j])) + ",";
	}
	if (vectors.length != 0)
		finalString += vectorToString(coords1To5(vectors[vectors.length - 1])) + ";";

	for (let j = 0; j < tris.length - 1; j++) {
		finalString += triangleToString(tris[j]) + ",";
	}
	if (vectors.length != 0)
		finalString += triangleToString(tris[tris.length - 1]);
	finalString += "\n";
}

for (let i = 0; i < indexes.length; i++) {
	let vectors = vertices[i];
	let tris = triangles[i];

	for (let j = 0; j < vectors.length - 1; j++) {
		finalString += vectorToString(coords1To6(vectors[j])) + ",";
	}
	if (vectors.length != 0)
		finalString += vectorToString(coords1To6(vectors[vectors.length - 1])) + ";";

	for (let j = 0; j < tris.length - 1; j++) {
		finalString += triangleToString(tris[j]) + ",";
	}
	if (vectors.length != 0)
		finalString += triangleToString(tris[tris.length - 1]);
	finalString += "\n";
}

fs.writeFileSync('./marchingTetrasCases.csv', finalString);
