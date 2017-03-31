using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;

namespace TerrainLib
{
	public class TerrainCreator
	{

		public TerrainCreator ()
		{




		}


		//==============================================
		// Originally taken from TerrainCreatorScript


		public void renderTerrain (int resolution)
		{
			// get constants 
			Vector3[] vecs = MeshUtil.Constants.UnitVectors2D;

			// extract parameters 
			IHeightMappable<Vector2>[] cls = NoiseLib.Constants.MappableClasses;

			// call the methods. 
			int v = 0; 
			float dx = 1f / resolution;

			for (int i = 0; i <= resolution; i++) {
				Vector3 p0 = Vector3.Lerp (vecs [0], vecs [2], i * dx); 
				Vector3 p1 = Vector3.Lerp (vecs [1], vecs [3], i * dx); 

				//			Debug.Log ("dx: " + (i * dx).ToString ());
				//			Debug.Log ("p0: " + p0.ToString () + " p1: " + p1.ToString ());

				for (int j = 0; j <= resolution; j++) {
					Vector3 p = Vector3.Lerp (p0, p1, j * dx);
					float height = genNoise (cls, p);

					if (mask_island && island_mode) {
						MeshLib.MaskMethod m = MeshLib.Mask.MaskMethods [(int)mask_type];
						height = MeshLib.Mask.Transform (p, height, island_size, m);
					}

					//				Debug.Log ("p: " + p.ToString ());
					//				Debug.Log ("Height: " + height.ToString ()); 
					//				Debug.Log ("Current vertex: " + verts [v].ToString ());

					verts [v].y = height;
					colors [v] = coloring.Evaluate (height + 0.5f);

					v++;
				}
			}
		}


		private float genNoise (IHeightMappable<Vector2>[] cls, Vector3 point)
		{
			float finalHeight = 0f;
			for (int i = 0; i < cls.Length; i++) {
				if (ratios [i] == 0f) {
					continue;
				}
				finalHeight += noiseWithOctaves (cls [i], point, octaves, frequency, lacunarity, persistence) * ratios [i];
			}
			return finalHeight;
		}

		private float noiseWithOctaves (IHeightMappable<Vector2> cl, Vector3 point, int octaves, float freq, float lacuna, float persist)
		{
			float sum = cl.noise (point * freq);

			float amplitude = 1f;
			float range = 1f;

			for (int o = 1; o < octaves; o++) {
				freq *= lacuna;
				amplitude *= persist;
				range += amplitude;
				sum += cl.noise (point * freq) * amplitude;
			}
			return sum / range;
		}

	}

}
