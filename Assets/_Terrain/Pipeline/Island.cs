using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;
using NoiseLib;
using MaterialLib;
using Util;

namespace Pipeline
{
	class Island 
	{
		private Mesh[] islandMesh = null;

		// constants
		public static float[] defRatio = new float[]{ 0.3f, 0, 0.7f };

		[Header ("Island opts")]
		public int avgSize = 10;
		public int avgHeight = 50;

		// use a flat plane instead of generating the mesh.
		public bool useDummy = true;

		// USED FOR TESTING; public for now
		[SerializeField]
		private Material material;
		[SerializeField]
		private MaskMethod mask;
		[SerializeField]
		private float[] noiseRatios;

		//==============================================
		// constructor

		// creates a random island given a few parameters
		public Island (Vector2 init, 
		              Material mat = null, 
		              MeshLib.Mask m = null, 
		              float[] ratios = null)
		{

			if (mat == null)
				material = MaterialController.GenRandom (); 
			if (m == null)
				mask = Mask.GenRandom ();  // random mask 
			if (ratios == null)
				noiseRatios = defRatio; 

			// initialize size based on gaussian distribution
			Loc = init;
			Scale = new Vector3 (
				GetIslandDimensions (avgSize), 
				GetIslandDimensions (avgSize), 
				GetIslandDimensions (avgHeight)
			);
		}

		private float GetIslandDimensions (int size)
		{
			return size * Math.Gaussian () * size;
		}

		//==============================================
		// getter/setters
		// center, absolute coordinates on world map
		public Vector2 Loc { get; private set; }

		// used for scaling mesh
		public Vector3 Scale { get; private set; }

		//==============================================
		// island interfacing operations

		public void CreateIsland ()
		{
			// create a mesh out of these params (deterministic) 

		}

		public void DisplayIsland ()
		{
			// display the mesh 
			if (islandMesh == null && !useDummy) {
				Debug.Log ("Could not display, need island."); 
				return;
			}

			// transform to the appropriate size.
			// todo: use blocks. 

			if (useDummy) {
				// disable mesh component;  
			} else {
			}
		}

		public void HideIsland ()
		{
			// remove mesh or plane component 
		}

		public void DestroyIsland ()
		{
			// delete this mesh (for memory optimization)  
			islandMesh = null;
		}
	}

}
