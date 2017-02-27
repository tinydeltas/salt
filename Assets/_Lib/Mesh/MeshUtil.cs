using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshLib
{
	class MeshUtil
	{

		public static int[] GetTriangles (int resolution)
		{
			int numQuads = resolution * resolution;
			int[] quads = new int[numQuads * 6]; 

			int t = 0; 
			for (int v = 0; v < numQuads; v++) {
				quads [t] = v; 
				quads [t + 1] = quads [t + 4] = v + resolution + 1; 
				quads [t + 2] = quads [t + 3] = v + 1; 
				quads [t + 5] = v + resolution + 2;
				t += 6; 
			}

			return quads;
		}
	}

}
