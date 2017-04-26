using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;
namespace TextureLib
{
	public class SolidTemplate 
	{


		public static float gen (Vector3  pos, 
			IHeightMappable<Vector2> noiseFunc)
		{
			return noiseFunc.noise (pos);
		}

	}

}
