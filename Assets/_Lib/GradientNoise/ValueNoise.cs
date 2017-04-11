using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = Util.Math;

namespace NoiseLib
{
	public class ValueNoise : IHeightMappable<Vector2>
	{
		// get constants 
		int[] h = Constants.hash;
		int m = Constants.hashMask;

		// 2D noise
		public float noise (Vector2 p)
		{
			// get int version
			int x0 = Mathf.FloorToInt (p.x); 
			int y0 = Mathf.FloorToInt (p.y); 

			// get starting displacements
			float dx = p.x - x0; 
			float dy = p.y - y0; 

			x0 &= m; 
			y0 &= m; 

			// get next ones over
			int x1 = x0 + 1; 
			int y1 = y0 + 1; 

			// first level hashing on x
			int _0 = h [x0];  
			int _1 = h [x1]; 

			// second level hashing on y 
			int _00 = h [_0 + y0]; 
			int _01 = h [_0 + y1]; 
			int _10 = h [_1 + y0]; 
			int _11 = h [_1 + y1];

			// smooth the displacements
			dx = M.Fade (dx); 
			dy = M.Fade (dy); 

			// interpolate between the relative points on a 2-d plane
			float t1 = Mathf.Lerp (_00, _10, dx); 
			float t2 = Mathf.Lerp (_01, _11, dx); 
			float z = Mathf.Lerp (t1, t2, dy); 

			// normalize the restult 
			float prod = z * (1f / m);

			return prod - 0.5f; 
		}

		// 3D noise
		public float _noise (Vector3 p)
		{
			// get constants
			int[] h = Constants.hash;
			int m = Constants.hashMask;

			int x0 = M.Floor (p.x);
			int y0 = M.Floor (p.y); 
			int z0 = M.Floor (p.z); 
			// only take the most significant bits of this int 
			// maybe not necessary if the side of the square isn't too large? 

			// mask tem 
			x0 &= m; 
			y0 &= m; 
			z0 &= m;

			// get the next adjacent point 
			int x1 = x0 + 1; 
			int y1 = y0 + 1; 
			int z1 = z0 + 1;

			// first level hashing on x 
			int _0 = h [x0]; 
			int _1 = h [x1];

			// second level hashing with the appropriate closest coordinate
			int _00 = h [_0 + y0]; 
			int _10 = h [_1 + y0]; 
			int _01 = h [_0 + y1]; 
			int _11 = h [_1 + y1];

			// third level three-dimensional coordinate hashing
			int x000 = h [_00 + z0]; 
			int x100 = h [_10 + z0]; 
			int x010 = h [_01 + z0]; 
			int x110 = h [_11 + z0]; 
			int x001 = h [_00 + z1]; 
			int x101 = h [_10 + z1]; 
			int x011 = h [_01 + z1]; 
			int x111 = h [_11 + z1];

			// get the smoothed floor displacements
			float dx = M.Fade (p.x - x0); 
			float dy = M.Fade (p.y - y0); 
			float dz = M.Fade (p.z - z0);

			// Do the interpolation 
			float p1 = M.Lerp (x000, x100, dx); 
			float p2 = M.Lerp (x010, x110, dx); 
			float t0 = M.Lerp (p1, p2, dy); 

			float p3 = M.Lerp (x001, x101, dx); 
			float p4 = M.Lerp (x011, x111, dx); 
			float t1 = M.Lerp (p3, p4, dy);

			float z = M.Lerp (t0, t1, dz);
				
			// normalize the result 
			float prod = z / m; 

			return prod;
		}
	}
}
