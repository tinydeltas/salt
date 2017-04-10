using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshUtil
{
	
	public class Constants
	{
		public static Vector3 _00 = new Vector3 (-0.5f, -0.5f);
		public static Vector3 _10 = new Vector3 (0.5f, -0.5f);
		public static Vector3 _01 = new Vector3 (-0.5f, 0.5f);
		public static Vector3 _11 = new Vector3 (0.5f, 0.5f);

		public static Vector3[] UnitVectors2D = {
			_00, _10, _01, _11
		};
	}
}
