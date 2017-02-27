using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshUtil
{
	
	public class Constants
	{

		public static Vector2 _00 = new Vector2 (-0.5f, -0.5f);
		public static Vector2 _10 = new Vector2 (0.5f, -0.5f);
		public static Vector2 _01 = new Vector2 (-0.5f, 0.5f);
		public static Vector2 _11 = new Vector2 (0.5f, 0.5f);

		public static Vector2[] UnitVectors2D = {
			_00, _10, _01, _11
		};
	}

}
