using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshLib
{
	class MeshUtil
	{
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

	
		public static int[] GetTriangles (int resolution)
		{
//			Debug.Log ("Resolution: " + resolution.ToString()); 

			int numQuads = resolution * resolution;
			int[] quads = new int[numQuads * 6]; 

			int t = 0; // counter for triangles
			for (int i = 0, v = 0; i < resolution; i++, v++) {
				for (int j = 0; j < resolution; j++, v++) {
					// assign two triangles
					quads [t] = v; 
					quads [t + 1] = v + resolution + 1; 
					quads [t + 4] = v + resolution + 1;
					quads [t + 2] = v + 1; 
					quads [t + 3] = v + 1;
					quads [t + 5] = v + resolution + 2;
					t += 6; 
				}
			}

//			Debug.Log ("Num quads expected: " + numQuads.ToString());
//			Debug.Log ("Num quads assigned: " + (t / 6f).ToString());
	

			return quads;
		}
	}
}
