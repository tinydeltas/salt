using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;
using NoiseLib;
using MaterialLib;
using TextureLib;
using TerrainLib;
using Util;

namespace Pipeline
{
	public class Island : GenericTerrain
	{
		public static new bool debug = false;

		//==============================================
		// PRIVATE VARIABLES


		private static int islandRange = 5;

		//==============================================
		// CONSTRUCTOR

		// creates a random island given a few parameters
		public Island (Vector3 init, 
		               Vector3 scale, 
		               IHeightMappable<Vector2> method,
						TextureTypes t,
			int density,
		               MeshLib.MaskMethod m = null)
			: base (init, scale, method, t, density)
		{
			// adjust the y coordinate of the island
			this.Loc = new Vector3 (Loc.x, -1 * Random.Range(1, islandRange), Loc.z); 

			// assign the type of mask
			this.Mask = m != null ? m : MeshLib.Mask.GenRandom ();

			if (this.Mask == null) {
				Debug.Log ("WHAT1");
			}



			_debug ("Initialized");

			StartCreateIsland (); 
		}

		public MaskMethod Mask { get; private set; }

		//==============================================
		// ISLAND OPERATIONS

		public void StartCreateIsland ()
		{
			_debug ("Creating island");
			newTerrain (); 

			// create a mesh out of these params (deterministic) 
			renderTerrain (Mask); 
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
			+ "\n[islandHeightRange]\t\t" + islandRange
			+ "\n";
		}

		new
		public void _debug (string message)
		{
			if (debug) {
				Debug.Log ("[Island log] " + message);
				Debug.Log (this.ToString ());
				Debug.Log ("[Terrain info] " + base.ToString ());
			}
		}
	}
}
