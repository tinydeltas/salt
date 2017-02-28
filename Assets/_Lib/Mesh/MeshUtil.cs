using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshLib
{
	class MeshUtil
	{

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
					quads [t + 1] =  v + resolution + 1; 
					quads [t + 4] = v + resolution + 1;
					quads [t + 2] =v + 1; 
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
