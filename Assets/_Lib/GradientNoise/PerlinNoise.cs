using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = Util.Math;

namespace NoiseLib
{
	public class PerlinNoise : IHeightMappable<Vector2>
	{
		private static int hashMask3D = 15;


		private static int[][] gradients3D = {
			new int[]{ 1, 1, 0 }, 
			new int[] { 1, 1, 0 }, 
			new int[] { -1, 1, 0 }, 
			new int[]{ -1, 1, 0 }, 
			new int[]{ 1, -1, 0 }, 
			new int[]{ -1, -1, 0 },
			new int[]{ 1, 0, 1 }, 
			new int[]{ -1, 0, 1 }, 
			new int[]{ 1, 0, -1 }, 
			new int[]{ -1, 0, -1 },
			new int[]{ 0, 1, 1 }, 
			new int[] { 0, -1, 1 }, 
			new int[]{ 0, -1, 1 }, 
			new int[]{ 0, 1, -1 },
			new int[]{ 0, -1, -1 }, 
			new int[]{ 0, -1, -1 }
		};



		public float noise (Vector2 p)
		{
			// get shared & individual constants 
			int[] h = Constants.hash;
			int[][] g = Constants.gradients2D; 

			int m = Constants.hashMask;
			int gm = Constants.hashMask2D;

			float n = Constants.normFactor;

			// get int base 
			int x0 = M.Floor (p.x); 
			int y0 = M.Floor (p.y); 

			// get displacements
			float dx0 = p.x - x0; 
			float dy0 = p.y - y0; 

			float dx1 = dx0 - 1f; 
			float dy1 = dy0 - 1f;

			// do the masking 
			x0 &= m; 
			y0 &= m;

			// get next ones over
			int x1 = x0 + 1; 
			int y1 = y0 + 1; 

			// get first-level displacement on x 
			int _0 = h [x0]; 
			int _1 = h [x1]; 

			int _00 = h [_0 + y0]; 
			int _01 = h [_0 + y1]; 
			int _10 = h [_1 + y0]; 
			int _11 = h [_1 + y1]; 

			dx0 = M.Fade (dx0);
			dy0 = M.Fade (dy0); 

			// this is where it diverges from value noise: pick a gradient 
			int[] _x00 = g [h [_00] & gm]; 		 
			int[] _x10 = g [h [_10] & gm];       
			float x00 = M.Dot (_x00, dx0, dy0);     // u = p dot h[p_00]
			float x10 = M.Dot (_x10, dx1, dy0); 	   // v = p dot h[_10]

			float p1 = M.Lerp (x00, x10, dx0); 

			int[] _x01 = g [h [_01] & gm]; 
			int[] _x11 = g [h [_11] & gm]; 
			float x01 = M.Dot (_x01, dx0, dy1);
			float x11 = M.Dot (_x11, dx1, dy1); 

			float p2 = M.Lerp (x01, x11, dx0);
	 
			float t0 = M.Lerp (p1, p2, dy0); 
			float prod = t0 * n; 

			return prod * 0.5f;
		}

		// four-dimensional option
		public float _noise (Vector3 p)
		{
			// get constants 
			int[] h = Constants.hash;
			int m = Constants.hashMask;

			int gm = hashMask3D;
			int[][] g = gradients3D;

			// get masked int version
			int x0 = M.Floor (p.x);
			int y0 = M.Floor (p.y); 
			int z0 = M.Floor (p.z); 

			// get the displacements
			float dx0 = p.x - x0; 
			float dy0 = p.y - y0; 
			float dz0 = p.z - z0; 

			// now do the masking 
			x0 &= m; 
			y0 &= m; 
			z0 &= m;

			// get next ones over
			int x1 = x0 + 1; 
			int y1 = y0 + 1; 
			int z1 = z0 + 1;
			;

			// why is this subtracted by 1? 
			float dx1 = dx0 - 1f;
			float dy1 = dy0 - 1f; 
			float dz1 = dz0 - 1f; 

			// first level hashing on x
			int _0 = h [x0]; 
			int _1 = h [x1]; 

			// second level hashing on y
			int _00 = h [_0 + y0]; 
			int _01 = h [_0 + y1]; 
			int _10 = h [_1 + y0]; 
			int _11 = h [_1 + y1];

			// third "level" hashing on z 
			int[] _000 = g [h [_00 + z0] & gm]; 
			int[] _001 = g [h [_00 + z1] & gm];
			int[] _010 = g [h [_01 + z0] & gm]; 
			int[] _011 = g [h [_01 + z1] & gm];
			int[] _100 = g [h [_10 + z0] & gm]; 
			int[] _101 = g [h [_10 + z1] & gm];
			int[] _110 = g [h [_11 + z0] & gm]; 
			int[] _111 = g [h [_11 + z1] & gm]; 

			// do the dot product with displacement vector in order to smooth 
			float x000 = M.Dot (_000, dx0, dy0, dz0); 
			float x001 = M.Dot (_001, dx0, dy0, dz1); 
			float x010 = M.Dot (_010, dx0, dy1, dz0); 
			float x011 = M.Dot (_011, dx0, dy1, dz1); 
			float x100 = M.Dot (_100, dx1, dy0, dz0); 
			float x101 = M.Dot (_101, dx1, dy0, dz1); 
			float x110 = M.Dot (_110, dx1, dy1, dz0);
			float x111 = M.Dot (_111, dx1, dy1, dz1); 

			// smooth the original displacements
			float dx = M.Fade (dx0); 
			float dy = M.Fade (dy0); 
			float dz = M.Fade (dz0); 

			// Do the interpolation
			float p1 = M.Lerp (x000, x100, dx); 
			float p2 = M.Lerp (x010, x110, dx); 
			float t0 = M.Lerp (p1, p2, dy); 

			float p3 = M.Lerp (x001, x101, dx); 
			float p4 = M.Lerp (x011, x111, dx); 
			float t1 = M.Lerp (p3, p4, dy);

			float res = M.Lerp (t0, t1, dz); 
			return res;
		}

	}
}
