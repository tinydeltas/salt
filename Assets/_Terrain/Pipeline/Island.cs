using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;
using NoiseLib;
using MaterialLib;
using TerrainLib;
using Util;

namespace Pipeline
{
	public class Island : GenericTerrain
	{

		// USED FOR TESTING; public for now
		[SerializeField]
		public MaskMethod Mask;

		private static float islandLevel= -1f;


		//==============================================
		// constructor

		// creates a random island given a few parameters
		public Island (Vector3 init, 
					 	Vector3 scale, 
		               Material mat = null,  
		               float[] ratios = null, 
					   MeshLib.MaskMethod m = null)
			: base (init, scale, mat, ratios)

		{
			Loc = new Vector3 (Loc.x, islandLevel, Loc.z); 

			if (m == null)
				Mask = MeshLib.Mask.SquareTransform;
	
			Debug.Log (this.ToString ());
		}

		//==============================================
		// island interfacing operations

		public Mesh CreateAndDisplayIsland ()
		{
			Debug.Log ("[Island] About to create Island");
			newTerrain (); 
			// create a mesh out of these params (deterministic) 
		
			Debug.Log ("[Island] About to render Island");
			renderTerrain(Mask); 
			return this.Mesh; 
		}


		public void HideIsland ()
		{
			// remove mesh or plane component 
		}

		public void DestroyIsland ()
		{
			this.destroyTerrain (); 
		}


		//==============================================
		// UTIL


	}

}
