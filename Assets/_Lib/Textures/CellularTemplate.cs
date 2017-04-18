using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using Util.Math;

namespace TextureLib
{
	class CellularTemplate : TextureBuilder
	{

		private static int numDist = 3;

		private static float[] distArr;

		private static uint[] probs = new uint[] {
			393325350,
			1022645910,
			1861739990,
			2700834071,
			3372109335, 
			3819626178,
			4075350088,
			4203212043,
		};

		// for efficiency
		private static float[] curPt = new float[3];

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
			distArr = new float[numDist];

			for (int i = 0; i < probs.Length; i++)
				probs [i] = probs [i] / uint.MaxValue;
		}

		public Color gen (Vector3 pos)
		{
			// based on the paper 

			// hash the cube 
			int h = pos.ToString ().GetHashCode ();

			// create the random number generator by seeding it with the hash 
			Random.InitState (h); 

			// determine the number of feature points 
			int n = probLookup (Random.value);

			// for each feature point, place it in the cube 
			for (int i = 0; i < n; i++) {


				// and find the closest feature point to the input (pos)
				curPt [0] = pos.x + Random.value; 
				curPt [1] = pos.y + Random.value; 
				curPt [2] = pos.z + Random.value; 

				float dist = randDist (pos);

				// by inserting into sorted list 
				distArr = insert (dist, distArr);
			}


			// color distance function 
			float color = randDiff (distArr); 

			if (color > 1 || color < 0) {
				Debug.Log ("color: " + color.ToString ());
			}

			return new Color (color, color, color, 1);
		}
	
		// insertion function

		private float[] insert (float dist, float[] distArr)
		{
			int i = 0;
			while (i < 3 && dist > distArr [i])
				i++;
			if (i < numDist)
				distArr [i] = dist;
			return distArr;
		}
		
		// distance functions

		// yay optimization
		public delegate float distFunc (Vector3 p1);

		private distFunc[] DistFuncs = {
			euclidian, 
			manhattan,
		};

		private static float euclidian (Vector3 p1)
		{
			return Mathf.Sqrt (manhattan (p1));
		}

		private static float manhattan (Vector3 p1)
		{
			float dx = p1.x - curPt [0]; 
			float dy = p1.y - curPt [1];
			float dz = p1.z - curPt [2];
			return dx*dx + dy*dy + dz*dz;
		}

		private float randDist (Vector3 p1)
		{
//			int idx = Random.Range (0, DistFuncs.Length);
			return DistFuncs [1] (p1);
		}

		// combination functions
		public delegate float combFunc (float[] dists);

		private combFunc[] CombFuncs = {
			cellular_1, 
			cellular_2, 
			cellular_3, 
			cellular_4
		};

		// c1 = -1
		private static float cellular_1 (float[] dists)
		{
			return -1f * dists [0]; 
		}

		// c2 = 1
		private static float cellular_2 (float[] dists)
		{
			return dists [0];
		}

		// c3 = 1
		private static float cellular_3 (float[] dists)
		{
			return dists [2]; 
		}

		// c1 = -1 and c2 = 1
		private static float cellular_4 (float[] dists)
		{
			return -1 * dists [0] + dists [1];
		}

		private float randDiff (float[] distArr)
		{
//			int idx = Random.Range (0, CombFuncs.Length);
			return CombFuncs [3](distArr); 

		}

	}

}
