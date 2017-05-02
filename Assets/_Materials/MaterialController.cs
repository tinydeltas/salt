// TODO 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLib
{
	
	class MaterialController : MonoBehaviour
	{
		public const string defaultMaterialPath = "TerrainShader";
		public const string metalPath = "Metals/Materials/metal0";
		public const int numMetals = 11;

		public const int numM = 17;
		public const string mPath = "Materials/";

		public static int numMaterials;

		//==============================================
		// CONSTRUCTOR

		public static void Init ()
		{
			Materials = new List<Material> (); 

			Materials.Add (new Material (Shader.Find (defaultMaterialPath)));

			// load metal imports 
			for (int i = 1; i < numMetals + 1; i++) {
				string path = string.Join ("", 
					              new string[] { metalPath, i.ToString (), "_diffuse" });
				Materials.Add (Resources.Load (path) as Material);
			}

			for (int i = 1; i < numM + 1; i++) {
				Materials.Add (Resources.Load (mPath + i.ToString ()) as Material);
			}

			numMaterials = Materials.Count;
		}

		public static Material GenRandom ()
		{
			return Materials [Random.Range (0, numMaterials)];
		}

		public static Material GenDefault() {
			return Materials [0];
		}

		//==============================================
		// MEMBERS

		public static List<Material> Materials { get ; private set; }
	}
}
