using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using Util.Math;

namespace TextureLib
{
	class CellularTemplate : TextureBuilder
	{

		public bool debug = true;

		private static int numDist = 3;
		private static uint[] _probs = new uint[] {
			393325350,
			1022645910,
			1861739990,
			2700834071,
			3372109335, 
			3819626178,
			4075350088,
			4203212043,
		};

		private static float[] probs;
		float[] distArr = new float[numDist];

		// probability lookup

		private static int probLookup (float value)
		{
			int i = 0; 
			while (i < 8 && probs [i] < value)
				i++;
			return i;
		}

		public CellularTemplate ()
		{
			probs = new float[_probs.Length];
			for (int i = 0; i < _probs.Length; i++)
				probs [i] = (float)_probs [i] / uint.MaxValue;
		}

		public static Texture2D fillTexture(Texture2D tex, int resolution, int size, TextureBuilder tb) {
			for (int y = 0; y < size; y++) {
				int yAbs = y * resolution; 
				for (int z = 0; z < size; z++) {
					int zAbs = z * resolution;

					for (int i = 0; i < resolution; i++) {

						for (int j = 0; j < resolution; j++) {

							Vector3 pt = new Vector3 (
								y + ((float)i / resolution), 
								z + ((float)j / resolution), 
								0f);

							tex.SetPixel ( 
								yAbs + i,  
								zAbs + j, 
								tb.gen (pt, y, z));
						}
					}
				}
			}
			tex.Apply ();
			return tex;
		}

		public Color gen (Vector3 p, int cubeX, int cubeY)
		{
			// based on the paper 
			int x, y, n;
			float pX = p.x;
			float pY = p.y;
			float dX, dY;

			reset (distArr);

			for (int a = -1; a < 2; a++) {
				for (int b = -1; b < 2; b++) {
					x = cubeX + a; 
					y = cubeY + b;

					// hash the cube
					// create the random number generator by seeding it with the hash 
					Random.InitState ((541 * x + 79 * y) % int.MaxValue);

					// determine the number of feature points 
					n = probLookup (Random.value);

					// for each feature point, place it in the cube 
					for (int i = 0; i < n; i++) {
						dX = x + Random.value; 
						dY = y + Random.value;

						// by inserting into sorted list 

						insert (manhattan (pX, pY, dX, dY));
					}
				}
			}
				
			// color distance function 
			float color = cellular_4 (distArr); 
//
//			_debug("color: " + color.ToString ()); 
			return new Color (color, color, color, 1);
		}

		private void reset (float[] distArr)
		{ 
			for (int i = 0; i < distArr.Length; i++) {
				distArr [i] = float.MaxValue;
			}
		}

		private static uint lcgRandom (uint lastValue)
		{
			return (1103515245u * lastValue + 12345u) % uint.MaxValue;
		}

		// insertion function
		private void insert (float dist)
		{
			float temp;
			for (int i = distArr.Length - 1; i >= 0; i--) {
				if (dist > distArr [i])
					break; 
				temp = distArr [i]; 
				distArr [i] = dist; 
				if (i + 1 < distArr.Length)
					distArr [i + 1] = temp;
			}
				
//			return distArr;
		}
		
		// distance functions

		// yay optimization
		//		public delegate float distFunc (float pX, float pY);
		//
		//		private distFunc[] DistFuncs = {
		//			euclidian,
		//			manhattan,
		//		};

		//		private static float euclidian (float pX, float pY)
		//		{
		//			return Mathf.Sqrt (manhattan (pX, pY));
		//		}

		private static float manhattan (float pX, float pY, float dX, float dY)
		{
			float dx = pX - dX; 
			float dy = pY - dY;

			return dx * dx + dy * dy;
		}

		//		private float randDist (float pX, float pY)
		//		{
		////			int idx = Random.Range (0, DistFuncs.Length);
//			return DistFuncs [1] (pX, pY);
//		}
//
		// combination functions
		public delegate float combFunc (float[] dists);

		private combFunc[] CombFuncs = {
//			cellular_1, 
			cellular_2, 
			cellular_3, 
			cellular_4
		};
//
//		// c1 = -1
//		private static float cellular_1 (float[] dists)
//		{
//			return -1 * dists [0]; 
//		}

		// c2 = 1
		public static float cellular_2 (float[] dists)
		{
			return dists [0];
		}

		// c3 = 1
		public static float cellular_3 (float[] dists)
		{
			return dists [2]; 
		}

		// c1 = -1 and c2 = 1
		public static float cellular_4 (float[] dists)
		{
			return -1 * dists [0] + dists [1];
		}

		private float randDiff (float[] distArr)
		{
			int idx = Random.Range (0, CombFuncs.Length);
			return CombFuncs [idx] (distArr); 

		}

		private void _debug (string s)
		{
			if (debug) {
				Debug.Log (s);
			}
		}

	}

}
