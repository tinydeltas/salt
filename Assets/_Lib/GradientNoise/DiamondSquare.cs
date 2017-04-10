// TODO 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = Util.Math;
using Me = MeshLib.MeshUtil;

namespace NoiseLib
{
	public interface MeshNoise
	{
		Vector3[] CreateMesh (int squareSize);
	}

	class DiamondSquare : MeshNoise
	{
		public bool useGaussian = true;

		public int maxIteration = 10;
		private int iterationCount = 0;

		Vector3[] vertices;
		int res;
		int numVerts;

		private void init ()
		{
			// initialize array with pseudorandom numbers 
			vertices [0] = assignRandomVec (); // top left
			vertices [res - 1] = assignRandomVec (); // top right
			vertices [numVerts - 1 - res] = assignRandomVec (); // bottom left??
			vertices [numVerts - 1] = assignRandomVec (); // bottom right
		}


		private Vector3 assignRandomVec (float offset = 1f)
		{
			return new Vector3 (assignRandom (offset), assignRandom (offset));
		}

		private float assignRandom (float offset = 1f)
		{
			if (useGaussian) {
				return M.Gaussian (offset);
			}
			return Random.Range (0, offset);
		}

		public DiamondSquare (int squareSize = 128)
		{
			res = squareSize;
			numVerts = (res + 1) * (res + 1);
			vertices = new Vector3[numVerts];
		}

		public Vector3[] CreateMesh (int squareSize)
		{
			iterationCount = 0; // reset the count 
			int numVerts = squareSize + 1;

			init (); 
			int v = 0;
			while (v <= numVerts && iterationCount < maxIteration) {
				iterate (vertices);
				iterationCount++;
			}
			return vertices;

		}

		private void iterate (Vector3[] vertices)
		{
			int idx = iterationCount;// TODO
			int[] neighbors = (iterationCount % 2 == 0) ? Me.getSquareNeighbors (res, idx) 
				: Me.getDiamondNeighbors (res, idx);

			float sumX = 0; 
			float sumY = 0;
			foreach (int i in neighbors) {
				if (i == -1) {
					continue;
				}
				sumX += vertices [i].x; 
				sumY += vertices [i].y;

			}
		
			vertices [idx] = new Vector3 (sumX + assignRandom (), sumY + assignRandom ());
		}
	}
}
