using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseLib
{
	public static class VoronoiNoise 
	{
		public static bool debug = true;

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

		public static int[][] indices = {
			new int[]{ 1, 0 },
			new int[]{ -1, 0 },
			new int[]{ 0, 1 },
			new int[]{ 0, -1 },
			new int[]{ 1, 1 },
			new int[]{ -1, 1 },
			new int[]{ 1, -1 },
			new int[]{ -1, -1 },
			new int[]{ 0, 0 },  
		};

		static float[] distArr = new float[numDist];

		// should only be called once
		public static void Init ()
		{
			probs = new float[_probs.Length];
			for (int i = 0; i < _probs.Length; i++)
				probs [i] = (float)_probs [i] / uint.MaxValue;
		}

		public static float noise (Vector2 p) {
			// based on the paper 
			int x, y, n, h;
			
			int cubeX = Mathf.FloorToInt(p.x); 
			int cubeY = Mathf.FloorToInt(p.y);

			float pX = p.x;
			float pY = p.y;

			reset (distArr);

			for (int j = 0; j < indices.Length; j++) {
				x = cubeX + indices[j][0]; 
				y = cubeY + indices[j][1];

				h = (541 * x + 79 * y);

				// hash the cube
				// create the random number generator by seeding it with the hash 
				Random.InitState (h);

				// determine the number of feature points 
				n = probLookup (Random.value);

				// for each feature point, place it in the cube 
				for (int i = 0; i < n; i++) {
					// by inserting into sorted list 
					insert (manhattan (pX, pY, 
						x + Random.value, 
						y + Random.value));
				}
			}

			// color distance function  
			return cellular_4 (distArr);
		}

		// probability lookup
		private static int probLookup (float value)
		{
			int i = 0; 
			while (i < 8 && probs [i] < value)
				i++;
			return i;
		}


		private static void reset (float[] distArr)
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
		private static float[] insert (float dist)
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

			return distArr;
		}

		// distance functions

		// yay optimization
		public delegate float distFunc (float pX, float pY,
			float dX, float dY);

		private static distFunc[] DistFuncs = {
			euclidian,
			manhattan,
		};

		private static float euclidian (float pX, float pY,
			float dX, float dY
		)
		{
			return Mathf.Sqrt (manhattan (pX, pY, dX, dY));
		}

		private static float manhattan (float pX, float pY, 
			float dX, float dY)
		{
			float dx = pX - dX; 
			float dy = pY - dY;
			return dx * dx + dy * dy;
		}

		// combination functions
		public delegate float combFunc (float[] dists);

		private static combFunc[] CombFuncs = {
			//			cellular_1, 
			cellular_2, 
			cellular_3, 
			cellular_4
		};
			
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

		private static float randDiff (float[] distArr)
		{
			int idx = Random.Range (0, CombFuncs.Length);
			return CombFuncs [idx] (distArr); 

		}

		private static void _debug (string s)
		{
			if (debug) {
				Debug.Log (s);
			}
		}

	}

}
