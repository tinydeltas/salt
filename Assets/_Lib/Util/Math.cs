using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
	class Math
	{
		// for 2d vectors
		public static float Dot (Vector2 vec1, Vector2 vec2)
		{
			return vec1.x * vec2.x + vec1.y * vec2.y;
		}

		public static float Dot (Vector2 vec1, float x, float y)
		{
			return vec1.x * x + vec1.y * y;
		}

		public static float Dot(int[] vec1, float x2, float y2) {
			return vec1 [0] * x2 + vec1 [1] * y2;
		}

		// for 3d vectors
		public static float Dot (Vector3 vec1, Vector3 vec2)
		{
			return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
		}
			
		public static float Dot(int[] vec1, float x2, float y2, float z2) {
			return vec1[0] * x2 + vec1[1] * y2 + vec1[2]* z2;
		}

		public static int Floor (float x)
		{
			return Mathf.FloorToInt (x);
		}

		//		public static int floor (float x)
		//		{
		//			return x > 0 ? (int)x : (int)x - 1;
		//		}

		public static float Lerp (float a, float b, float t)
		{
			return Mathf.Lerp (a, b, t);
		}
	

		public static Vector2 Lerp (Vector2 a, Vector2 b, float t)
		{
			return Vector2.Lerp (a, b, t);
		}

		public static Vector3 Lerp (Vector3 a, Vector3 b, float t)
		{
			return Vector3.Lerp (a, b, t);
		}

		// Used in Perlin noise
		public static float Fade (float t)
		{
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}
	}

}
