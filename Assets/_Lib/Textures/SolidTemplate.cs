using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;
namespace TextureLib
{
	public class SolidTemplate 
	{
		public static float gen (Vector3  pos)
		{
			return Constants._perlin.noise (pos) + 0.5f;
		}

	}

}
