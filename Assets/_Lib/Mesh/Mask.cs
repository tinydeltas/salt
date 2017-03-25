using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MeshLib
{
	delegate float MaskMethod(Vector3 p, float size); 

	public enum MaskTypes {
		Circular, 
		Round,
		Square,
	};

	class Mask
	{
		public static MaskMethod[] MaskMethods = {
			RadialTransform, 
			RoundTransform, 
			SquareTransform
		};

		//todo
		public static MaskMethod GenRandom() {
			return RoundTransform;
		}

		public static float Transform(Vector3 p, float noise, float size, MaskMethod m) {
			float distance = m(p, size);

			float max_width = size * 0.5f;
			float delta = distance / max_width;
			float gradient = delta * delta;

			noise *= Mathf.Max(0.0f, 1.0f - gradient);
			//			Debug.Log ("max width: " + max_width.ToString() + " / gradient: " + gradient.ToString() + " / noise: " + noise.ToString ());

			return noise;
		}

		public static float RadialTransform(Vector3 p, float size) {
			float distance_x = Mathf.Abs (p.x);
			float distance_y = Mathf.Abs (p.y);
			return Mathf.Sqrt(distance_x*distance_x + distance_y*distance_y); // circular mask
		}

		public static float RoundTransform(Vector3 p, float size) {
			float distance_x = Mathf.Abs (p.x);
			float distance_y = Mathf.Abs (p.y);
			return distance_x*distance_x + distance_y*distance_y; 
		}

		public static float SquareTransform(Vector3 p, float size) {
			float distance_x = Mathf.Abs (p.x);
			float distance_y = Mathf.Abs (p.y);
			return Mathf.Max(distance_x, distance_y);
		}

		private float AlphaTransform(float cutoff) {
			return 0f;
		}

	}

}
