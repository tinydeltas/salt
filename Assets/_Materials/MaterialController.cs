// TODO 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLib
{
	
	class MaterialController : MonoBehaviour
	{
		public static string defaultMaterialPath = "TerrainShader";
		public static string metalPath = "Metals/Materials/metal0";

		public static int numMetals = 11;
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
