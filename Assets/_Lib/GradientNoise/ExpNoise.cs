using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = Util.Math;

namespace NoiseLib
{
	public class ExpNoise :  IHeightMappable<Vector2>
	{
		private float[] e = new float[256];

		public static float mu = 0.99f;
		// from parberry, 2015

		int[] h = Constants.hash;
		int[][] g = Constants.gradients2D;

		int m = Constants.hashMask;
		int gm = Constants.hashMask2D;

		float n = Constants.normFactor;

		float[] p_00 = new float[2]; 
		float[] p_01 = new float[2];
		float[] p_10 = new float[2];
		float[] p_11 = new float[2];

		public ExpNoise ()
		{
			init ();
		}

		private void init ()
		{
			for (int i = 0; i < 256; i++) {
				e [i] = Mathf.Pow (mu, (float)i); 
			}
		}

		public float noise (Vector2 p)
		{
			// get shared & individual constants 

			// get int base 
			int x0 = Mathf.FloorToInt (p.x); 
			int y0 = Mathf.FloorToInt (p.y); 

			// get displacements
			float dx0 = p.x - x0; 
			float dy0 = p.y - y0; 

			float dx1 = dx0 - 1f; 
			float dy1 = dy0 - 1f;

			x0 &= m; 
			y0 &= m;

			// Step 3 & 4: first get p00, p01, p10, and p11 
			int x1 = x0 + 1; 
			int y1 = y0 + 1; 

			int _0 = h [x0]; 
			int _1 = h [x1]; 

			int _00 = h [_0 + y0];  // h(p00)
			int _01 = h [_0 + y1];  // h(p01)
			int _10 = h [_1 + y0];  // h(p10)
			int _11 = h [_1 + y1];  // h(p11)

			dx0 = M.Fade (dx0); // must be in [0, 1] range
			dy0 = M.Fade (dy0);

			p_00 [0] = dx0; 
			p_00 [1] = dy0; 
			p_01 [0] = dx0; 
			p_01 [1] = dy1; 
			p_10 [0] = dx1; 
			p_10 [1] = dy0; 
			p_11 [0] = dx1; 
			p_11 [1] = dy1;
	
			// u = h'(p00)p dot h (p00)
			// here, p is (dx0, dy0)
			// and h'(p00) is h'(h(p00))

			float p1 = Mathf.Lerp (M.Dot (g [h [_00] & gm], e [_00] * p_00[0], e[_00] * p_00[1]), 
			M.Dot (g [h [_10] & gm], e [_10] * p_10[0], e[_10] * p_10[1] ), dx0);

			// v = h'(p01)p  * h(p01) 

			// Step 6&8 
			// u = (h'(p10)p) dot h (p10)
			// v = h'(p11)p dot h (p11)
			float p2 = Mathf.Lerp (M.Dot (g [h [_01] & gm], e [_01] * p_01[0], e[_01] * p_01[1]), 
				M.Dot (g [h [_11] & gm], e [_11] * p_11[0], e[_11] * p_11[1]), dx0); 

			return Mathf.Lerp (p1, p2, dy0) * n *0.5f;
		}


		public float _noise (Vector3 p)
		{
			return 0f;
		}
	}
}

