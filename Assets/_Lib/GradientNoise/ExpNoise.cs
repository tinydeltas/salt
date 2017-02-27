using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = Util.Math;

namespace NoiseLib
{

	public class ExpNoise :  IHeightMappable<Vector2>

	{
		private float[] gradients2D = new float[256]; 
		private float mu = 1.16f; // from parberry, 2015

		private void init(){
			for (int i = 0; i < 256; i++) {
				gradients2D [i] = Mathf.Pow (mu, (float)i); 
			}
		}

		public float noise (Vector2 p)
		{
			if (gradients2D == null) {
				init (); 
			}; 

			// get constants 
			int[] h = Constants.hash;
			int[][] g = Constants.gradients2D; 
			int m = Constants.hashMask;
			int gm = Constants.hashMask2D;
			float n = Constants.normFactor;

			// get int base 
			int x0 = M.Floor(p.x); 
			int y0 = M.Floor (p.y); 

			// get displacements
			float dx0 = p.x - x0; 
			float dy0 = p.y - y0; 

			float dx1 = dx0 - 1; 
			float dy1 = dy0 - 1;

			// do the masking 
			x0 &= m; 
			y0 &= m;

			// get next ones over
			int x1 = x0 + 1; 
			int y1 = x1 + 1; 

			// get first-level displacement on x 
			int _0 = h[x0]; 
			int _1 = h [x1]; 

			dx0 = M.Fade (dx0);
			dy0 = M.Fade (dy0); 

			// this is where it diverges from value noise: pick a gradient 
			int[] _00 = g[h[_0 + y0] & gm]; 
			int[] _01 = g [h [_0 + y1] & gm]; 
			float x00 = M.Dot (_00, dx0, dy0); 
			float x01 = M.Dot (_01, dx0, dy1); 
	

			int[] _10 = g [h [_1 + y0] & gm]; 
			int[] _11 = g [h [_1 + y1] & gm]; 
			float x10 = M.Dot (_10, dx1, dy0);  //u = p * h (p_10) 
			float x11 = M.Dot (_11, dx1, dy1);  // v = p

			float p1 = M.Lerp (x00, x10, dx0); 
			float p2 = M.Lerp (x01, x11, dx0);
		
			float t0 = M.Lerp (p1, p2, dy0); 
			float prod = t0 * n; 

			return prod;
		}
	}

}

