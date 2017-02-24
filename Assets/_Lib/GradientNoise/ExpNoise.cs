using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseLib
{

	public class ExpNoise :  IHeightMappable<float>
	{
		public float noise (float x, float y)
		{
			int[] h = Noise.hash;
			int[][] g = Noise.gradients;
			int m = Noise.hashMask;

			int x0 = Math.floor (p.x);
			int y0 = Math.floor (p.y); 
			int z0 = Math.floor (p.z);

			return 0f;
		}
	}

}

