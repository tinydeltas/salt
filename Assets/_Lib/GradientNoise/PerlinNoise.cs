using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseLib
{
	
	public class PerlinNoise : MonoBehaviour, IHeightMappable<float>
	{


		public float noise (Vector3 p)
		{
			int[] h = Noise.hash;
			int[][] g = Noise.gradients;
			int m = Noise.hashMask;

			int x0 = Math.floor(p.x);
			int y0 = Math.floor(p.y); 
			int z0 = Math.floor(p.z); 

			float dx0 = p.x - x0; 
			float dy0 = p.y - y0; 
			float dz0 = p.z - z0; 

			return 0f;

		}

	}
}
