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
	public class Island
	{
		private Mesh islandMesh = null;

		// constants
		public static float[] defRatio = new float[]{ 0.3f, 0, 0.7f };

		[Header ("Island opts")]
		public float avgSize = 2f;
		public float avgHeight = 1f;

		// USED FOR TESTING; public for now
		[SerializeField]
		private MaskMethod mask;
		[SerializeField]
		private float[] noiseRatios;

		//==============================================
		// constructor

		// creates a random island given a few parameters
		public Island (Vector3 init, 
		               Material mat = null, 
		               MeshLib.MaskMethod m = null, 
		               float[] ratios = null)
		{
			if (mat == null)
				mat = new Material (Shader.Find ("Diffuse"));
			if (m == null)
				m = MeshLib.Mask.RoundTransform; 
			if (ratios == null)
				ratios = defRatio;
			
			material = mat; 
			mask = m; 
			noiseRatios = ratios;

			// initialize size based on gaussian distributionj
			Loc = init;
			Scale = new Vector3 (
				GetIslandDimensions (avgSize), 
				GetIslandDimensions (avgHeight),
				GetIslandDimensions (avgSize) 
			);

			Debug.Log (this.ToString ());

		}

		private float GetIslandDimensions (float size)
		{
			return Mathf.Abs (size * Math.Gaussian ());
		}

		//==============================================
		// getter/setters
		// center, absolute coordinates on world map
		[SerializeField]
		public Vector3 Loc { get; private set; }

		// used for scaling mesh
		[SerializeField]
		public Vector3 Scale { get; private set; }

		[SerializeField]
		public Material material { get; private set; }

		//==============================================
		// island interfacing operations

		public Mesh CreateIsland ()
		{
			islandMesh = new Mesh ();

			// create a mesh out of these params (deterministic) 

			return islandMesh;
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


		//==============================================
		// UTIL

		override
		public string ToString ()
		{

			return "[Loc]: " + this.Loc.ToString () +
			"\n[Scale]: " + this.Scale.ToString ()
			+ "\n[avgHeight]: " + this.avgHeight.ToString () + "\t[avgSize]: " + this.avgSize.ToString ();
		}
	}

}
