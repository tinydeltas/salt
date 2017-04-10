using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = Util.Math;

namespace NoiseLib
{
	public class ExpNoise :  IHeightMappable<Vector2>
	{
		private float[] gradients = new float[256];

		public static float mu = 0.99f;
		// from parberry, 2015

		public ExpNoise ()
		{
			init ();
		}

		private void init ()
		{
			for (int i = 0; i < 256; i++) {
				gradients [i] = Mathf.Pow (mu, (float)i); 
			}
		}

		public float noise (Vector2 p)
		{
			// get shared & individual constants 
			int[] h = Constants.hash;
			int[][] g = Constants.gradients2D; 

			int m = Constants.hashMask;
			int gm = Constants.hashMask2D;

			float n = Constants.normFactor;

			float[] e = gradients;

			// get int base 
			int x0 = M.Floor (p.x); 
			int y0 = M.Floor (p.y); 

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

			Vector2 p_00 = new Vector2 (dx0, dy0); 
			Vector2 p_01 = new Vector2 (dx0, dy1); 
			Vector2 p_10 = new Vector2 (dx1, dy0); 
			Vector2 p_11 = new Vector2 (dx1, dy1);
	
			// u = h'(p00)p dot h (p00)
			// here, p is (dx0, dy0)
			// and h'(p00) is h'(h(p00))
			int[] _x00 = g [h [_00] & gm]; 
			int[] _x10 = g [h [_10] & gm];
			Vector2 l1 = e [_00] * p_00;
			Vector2 l2 = e [_10] * p_10;
			float x00 = M.Dot (_x00, l1);
			float x10 = M.Dot (_x10, l2);

			float p1 = M.Lerp (x00, x10, dx0); 

			// v = h'(p01)p  * h(p01) 
			int[] _x01 = g [h [_01] & gm]; 
			int[] _x11 = g [h [_11] & gm];
			l1 = e [_01] * p_01; 
			l2 = e [_11] * p_11;
			float x01 = M.Dot (_x01, l1); 
			float x11 = M.Dot (_x11, l2);

			// Step 6&8 
			// u = (h'(p10)p) dot h (p10)
			// v = h'(p11)p dot h (p11)
			float p2 = M.Lerp (x01, x11, dx0); 

			float t0 = M.Lerp (p1, p2, dy0); 
			float prod = t0 * n;

			return prod * 0.5f;
		}

		public float _noise (Vector3 p)
		{
			return 0f;
		}

		private float s_curve (float x)
		{
			return (x * x) * (3f - (2f * x));
		}
	}

}

