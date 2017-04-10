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
		//==============================================
		// PRIVATE VARIABLES

		private MaskMethod Mask;
		private static float islandLevel = -1f;
	
		//==============================================
		// CONSTRUCTOR

		// creates a random island given a few parameters
		public Island (Vector3 init, 
		               Vector3 scale, 
		               Material mat = null,  
		               float[] ratios = null, 
		               MeshLib.MaskMethod m = null)
			: base (init, scale, mat, ratios)
		{
			// adjust the y coordinate of the island
			this.Loc = new Vector3 (Loc.x, islandLevel, Loc.z); 

			// assign the type of mask
			this.Mask = m != null ? m : MeshLib.Mask.GenRandom ();

			if (this.Mask == null) {
				Debug.Log ("WHAT1");
			}

			_debug ("Initialized");
		}

		//==============================================
		// ISLAND OPERATIONS

		public Mesh CreateAndDisplayIsland ()
		{
			_debug ("Creating island");
			newTerrain (); 

			// create a mesh out of these params (deterministic) 
			renderTerrain (Mask); 
			return this.Mesh; 
		}

		public void HideIsland ()
		{
			_debug ("Hiding island");
			// remove mesh or plane component 

		}

		public void DestroyIsland ()
		{
			_debug ("Destroying island.");
			this.destroyTerrain (); 
		}

		//==============================================
		// UTIL

		//==============================================
		// DOCUMENTATION AND DEBUGGING

		public string ToString ()
		{
			return "[Island info]"
			+ "\n[Mask]\t\t" + this.Mask.ToString ()
			+ "\n[islandLevel]\t\t" + islandLevel
			+ "\n";
		}

		new
		public void _debug (string message)
		{
			Debug.Log ("[Island log] " + message);
			Debug.Log (this.ToString ());
			Debug.Log ("[Terrain info] " + base.ToString ());
		}
	}
}
