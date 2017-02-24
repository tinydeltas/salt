﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
{

	public static float dot (Vector3 vec1, Vector3 vec2)
	{
		return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
	}

	public static float dot (Vector2 vec1, Vector2 vec2)
	{
		return vec1.x * vec2.x + vec1.y * vec2.y;
	}

	public static float dot (Vector2 vec1, float x, float y)
	{
		return vec1.x * x + vec1.y * y;
	}

	public static int floor (float x)
	{
		return Mathf.FloorToInt (x);
	}

	//		public static int floor (float x)
	//		{
	//			return x > 0 ? (int)x : (int)x - 1;
	//		}

	public static float lerp (float a, float b, float t)
	{
		return Math.lerp (a, b, t);
	}

	public static Vector2 lerp (Vector2 a, Vector2 b, float t)
	{
		return Vector2.Lerp (a, b, t);
	}

	public static Vector3 lerp (Vector3 a, Vector3 b, float t)
	{
		return Vector3.Lerp (a, b, t);
	}

	// Used in Perlin noise
	public static float fade (float t)
	{
		return t * t * t * (t * (t * 6f - 15f) + 10f);
	}
}
