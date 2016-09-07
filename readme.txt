Procedural Terrain Generation in Unity
Student ID: 719577

Implementation Steps:
	1. Generate random seed according to system time. Create an SIZE * SIZE float array. SIZE = 2n + 1.
	2. Randomize 4 corner values of the terrain
	3. Fill the squares in diamond-square steps. 2 noise values are added.
		1. Slopeness: random noise * 2^(-n) * Unity's Perlin Noise
		2. Roughness: random noise * n / SIZE * roughness factor
	   By setting correct combination of slopeness and roughness, natural-looking terrain can be
	   achieved
	4. 'Naturalize' the slope to smoothen out the ground
	5. Set the water height appropriately
	6. Pass on array to mesh generator. It will divide up the array into appropriate number of meshes,
	   then generates the verticies, normals and triangles. Completed array of meshes are returned.
	7. Array of meshes are applied to the children.

NOTE:
	Phong shader and directional light used is from the lab tutorial sessions.
	The light also rises and sets like the sun with colour change.
	Mesh normals are vertex normals.

All parameters are customizable through Unity Inspector.