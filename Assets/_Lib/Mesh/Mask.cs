using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MeshLib
{
	public delegate float MaskMethod (Vector3 p, float size);

	public enum MaskTypes
	{
		Circular,
		Round,
		Square,
	};

	public class Mask
	{
		public static MaskMethod[] MaskMethods = {
			RadialTransform, 
//			RoundTransform, 
			SquareTransform
		};

		public static MaskMethod GenRandom ()
		{
			int idx = Random.Range (0, MeshLib.Mask.MaskMethods.Length);
			return MaskMethods [idx];
		}

		//==============================================
		// DELEGATE FUNCTION

		public static float Transform (Vector3 p, float noise, MaskMethod m, float size = 1f)
		{
			if (m == null) {
				return noise; 
			}

			float distance = m (p, size);

			float max_width = size * 0.5f;
			float delta = distance / max_width;
			float gradient = delta * delta;

			noise *= Mathf.Max (0.0f, 1.0f - gradient);
			return noise;
		}

		//==============================================
		// MASK METHODS

		public static float RadialTransform (Vector3 p, float size)
		{
			float distance_x = Mathf.Abs (p.x);
			float distance_y = Mathf.Abs (p.y);
			return Mathf.Sqrt (distance_x * distance_x + distance_y * distance_y); // circular mask
		}

		public static float SquareTransform (Vector3 p, float size)
		{
			float distance_x = Mathf.Abs (p.x);
			float distance_y = Mathf.Abs (p.y);
			return Mathf.Max (distance_x, distance_y);
		}

		//==============================================
		// BROKEN MASK METHODS (todo: fix)

		private float AlphaTransform (float cutoff)
		{
			return 0f;
		}

		//		public static float RoundTransform(Vector3 p, float size) {
		//			float distance_x = Mathf.Abs (p.x);
		//			float distance_y = Mathf.Abs (p.y);
		//			return distance_x*distance_x + distance_y*distance_y;
		//		}
	}
}
