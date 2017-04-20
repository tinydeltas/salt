using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextureLib
{

	public enum TextureTypes
	{
		Solid,
		Cellular, 
	};

	public interface TextureBuilder
	{
		Color gen (Vector3 point, int cubeX, int cubeY);
	}

	public class TextureController
	{
		public static TextureBuilder[] TextureClasses = {
			new SolidTemplate (), 
			new CellularTemplate (),
		};

		public static TextureBuilder RandomTextureBuilder ()
		{
			int idx = Random.Range (0, TextureClasses.Length);
			TextureBuilder tb = TextureClasses [idx];
			return tb;
		}
	}
}
