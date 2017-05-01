using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;
namespace TextureLib
{
	class CellularTemplate 
	{
		public static float gen (Vector2 p)
		{
			return VoronoiNoise.noise (p) + 0.5f;
		}
	}
}
