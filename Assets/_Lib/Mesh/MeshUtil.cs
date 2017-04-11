using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshLib
{
	public static class MeshUtil
	{
		//==============================================
		// MESH FUNCTIONS 

		public static Mesh NewMesh(int resolution, Mesh m = null) {
			_debug("creating new Mesh");

			if (resolution <= 0) { 
				Debug.LogError ("Resolution for new mesh should not be <= 0 : "
				+ resolution.ToString ());
			}

			if (m == null)
				m = new Mesh (); 

			int numVerts = (resolution + 1) * (resolution + 1);

			Vector3[] verts = new Vector3[numVerts];
			Vector2[] uv = new Vector2[numVerts];
			Vector3[] norms = new Vector3[numVerts]; 
			Color[] colors = new Color[numVerts];

			Color c = Color.clear;
			Vector3 up = Vector3.up;
			int v = 0; // vertex enumerator 
			float u = 1f / resolution;
			for (int i = 0; i <= resolution; i++) {
				for (int j = 0; j <= resolution; j++) {
					verts [v] = new Vector3 (j * u - 0.5f, 0f, i * u - 0.5f); 
					norms [v] = up;
					colors [v] = c;
					uv [v] = new Vector2 (j * u, i * u);

					v++;
				}
			}

			m.vertices = verts; 
			m.colors = colors;
			m.uv = uv;
			m.normals = norms;
			m.triangles = getTriangles (resolution);

			return m;
		}
			
		public static int[] getTriangles (int resolution)
		{
			_debug ("Calculating triangles");

			int numQuads = resolution * resolution;
			int[] quads = new int[numQuads * 6]; 

			int t = 0; // counter for triangles
			for (int i = 0, v = 0; i < resolution; i++, v++) {
				for (int j = 0; j < resolution; j++, v++) {
					quads [t] = v; 
					quads [t + 1] = v + resolution + 1; 
					quads [t + 4] = v + resolution + 1;
					quads [t + 2] = v + 1; 
					quads [t + 3] = v + 1;
					quads [t + 5] = v + resolution + 2;
					t += 6; 
				}
			}
			return quads;
		}

		// on the go
		public static void meshCoalesce (Mesh m, int factor)
		{
			// make new triangles 
		}

		//==============================================
		// DOCUMENTATION AND DEBUGGING

		public static void _debug (string message)
		{
			Debug.Log ("[MeshLib log]\t\t" + message);
		}

		//==============================================
		// for diamond/square stuff

		// starts at the upper left and goes clockwise
		// return set <=8 of the neighbors of the vertex
		// (Basically converting from a 1d to 2d representation)
		public static int[] getNeighbors (int i, int res)
		{
			int[] n1 = getDiamondNeighbors (i, res);
			int[] n2 = getSquareNeighbors (i, res);

			int[] neighbors = new int[] {
				n2 [0], n1 [0], n2 [1], // first row 
				n1 [1], n1 [2], // second row 
				n2 [2], n1 [3], n2 [3] // third row 
			}; 

			return neighbors;
		}

		// get the indices of the four diamond point neighbors of a vertex
		// starting from the top middle
		public static int[] getDiamondNeighbors (int idx, int res)
		{
			int[] neighbors = new int[] { -1, -1, -1, -1 }; 
			Vector2 p = getPoint (res, idx); 
			if (p.y != 0) {
				neighbors [0] = getIdx (res, p.x, p.y - 1); 
			}
			if (p.x != res - 1) {
				neighbors [1] = getIdx (res, p.x + 1, p.y); 
			}
			if (p.y != res - 1) {
				neighbors [2] = getIdx (res, p.x, p.y + 1);
			}
			if (p.x != 0) {
				neighbors [3] = getIdx (res, p.x - 1, p.y);
			}
			return neighbors;
		}

		// starting from the top left
		public static int[] getSquareNeighbors (int idx, int res)
		{
			int[] neighbors = new int[] { -1, -1, -1, -1 }; 
			Vector2 p = getPoint (res, idx); 
			if (p.x != 0 && p.y != 0) {
				neighbors [0] = getIdx (res, p.x - 1, p.y - 1); 
			}
			if (p.x != (res - 1) && p.y != 0) {
				neighbors [1] = getIdx (res, p.x + 1, p.y - 1); 
			}
			if (p.x != 0 && p.y != (res - 1)) {
				neighbors [2] = getIdx (res, p.x - 1, p.y + 1);
			}
			if (p.x != (res - 1) && p.y != (res - 1)) {
				neighbors [3] = getIdx (res, p.x + 1, p.y + 1);
			}
			return neighbors;
		}

		public static int getIdx (int res, float x, float y)
		{
			return 0;
		}

		public static Vector2 getPoint (int res, int idx)
		{
			return new Vector2 (getX (res, idx), getY (res, idx));
		}

		public static int getX (int idx, int res)
		{
			return idx % res;
		}

		public static int getY (int idx, int res)
		{
			return (int)Mathf.Floor (idx / (float)res);
		}
	}
}
