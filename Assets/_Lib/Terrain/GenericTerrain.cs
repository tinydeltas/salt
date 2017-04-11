using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;
using MeshLib;
using Util;
using MaterialLib;

namespace TerrainLib
{
	public class GenericTerrain
	{
		public static bool debug = false;

		//==============================================
		// PRIVATE VARIABLES

		private static float sizeFactor = 10f; 

		//==============================================
		// supposed to be private (todo)

		public int resolution = 128;

		public int octaves = 6;
		public float frequency = 4f;
		public float lacunarity = 2f;
		public float persistence = 0.4f;

		//==============================================
		// CONSTRUCTOR

		public GenericTerrain (Vector3 init,
		                       Vector3 scale, 
		                       Material mat = null, 
			IHeightMappable<Vector2> method = null)
		{
			this.Method = method != null ? method : Constants.MappableClasses [0];
			this.Material = mat != null ? mat : MaterialController.GenRandom ();
	
			this.Loc = init;
			this.Scale = new Vector3 (
				scale.x + GetDimensions (scale.x / sizeFactor), 
				scale.y + GetDimensions (scale.y / sizeFactor),
				scale.z + GetDimensions (scale.z / sizeFactor) 
			);

			this.Coloring = randomGradient ();

			_debug ("Initialized");
		}

		//==============================================
		// MEMBERS

		// PUBLIC SET  

		// used for scaling mesh
		public Vector3 Scale { get; set; }

		// noise method used 
		public IHeightMappable<Vector2> Method {get; private set; }

		// center, absolute coordinates on world map
		public Vector3 Loc { get; protected set; }

		// material of the island
		public Material Material { get; private set; }

		// island mesh
		public Mesh Mesh { get; private set; }

		// color of the island
		public Gradient Coloring { get; private set; }

		//==============================================
		// Originally taken from TerrainCreatorScript

		public void renderTerrain (MaskMethod mask = null, Gradient coloring = null)
		{
			if (this.Mesh == null) {
				Debug.LogError ("[GenericTerrain] Mesh should not be null!!!");
				return;
			}

			_debug ("[renderTerrain] about to determine the vertices and colors for the terrain."); 

			// get constants 
			Vector3[] vecs = MeshUtil.Constants.UnitVectors2D;

			// extract parameters 
//			IHeightMappable<Vector2>[] cls = NoiseLib.Constants.MappableClasses;
			float dx = 1f / resolution;

			if (coloring == null) {
				coloring = Coloring;
			}

			// counter for vertices
			int v = 0; 

			// make unique noiseRatios for this island

			Color[] colors = this.Mesh.colors;
			Vector3[] vertices = this.Mesh.vertices;

			for (int i = 0; i <= resolution; i++) {
				Vector3 p0 = Vector3.Lerp (vecs [0], vecs [2], i * dx); 
				Vector3 p1 = Vector3.Lerp (vecs [1], vecs [3], i * dx); 

				for (int j = 0; j <= resolution; j++) {

					Vector3 p = Vector3.Lerp (p0, p1, j * dx);
					Vector3 n = new Vector3 (p.x + Loc.x, p.y + Loc.z);

					float height = genNoise (n);

					// apply maske
					height = MeshLib.Mask.Transform (p, height, mask);

					// readjust height to find color
					float modified = height + 0.5f;
					Color newColor = coloring.Evaluate (modified);

					colors [v] = newColor;
					vertices [v].y = height;

					v++;
				}
			}
	
			this.Mesh.vertices = vertices;
			this.Mesh.colors = colors;
			this.Mesh.RecalculateNormals (); 
		}

		private float genNoise (Vector3 point)
		{
			float freq = frequency;
			float sum = Method.noise (point * freq);

			float amplitude = 1f;
			float range = 1f;

			for (int o = 1; o < octaves; o++) {
				freq *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Method.noise (point * freq) * amplitude;
			}
			return sum / range;
		}
			
		//==============================================
		// TERRAIN LIFECYCLE OPERATIONS

		public void newTerrain (int res = 0)
		{
			if (this.Mesh == null) {
				_debug ("[newTerrain] making new terrain with res: " + res);
				this.Mesh = new Mesh ();
			} else {
				_debug ("[newTerrain] resizing terrain and starting fresh with res: " + res.ToString ());
				this.Mesh.Clear (); 
			}
				
			if (res > 0 ) {
				resolution = res;
			}

			this.Mesh = MeshLib.MeshUtil.NewMesh (resolution, this.Mesh);
		}

		public void destroyTerrain ()
		{
			_debug ("Destroying terrain");
			this.Mesh = null;
		}

		//==============================================
		// UTIL

		private float GetDimensions (float size)
		{
			return size * Math.Gaussian ();
		}

		private Gradient randomGradient ()
		{
			Gradient g = new Gradient (); 
	
			int n = Random.Range (6, 8); 

			GradientColorKey[] keys = new GradientColorKey[n]; 
			for (int i = 0; i < n; i++) {
				keys [i].color = Random.ColorHSV ();
				keys [i].time = (float)i / n;
			}

			GradientAlphaKey[] alpha = new GradientAlphaKey[2];
			alpha [0].alpha = 1f; 
			alpha [1].alpha = 1f; 
			alpha [0].time = 0f; 
			alpha [1].time = 1f;

			g.mode = GradientMode.Blend;
			g.SetKeys (keys, alpha);
			return g;
		}

		//==============================================
		// DOCUMENTATION AND DEBUGGING

		override
		public string ToString ()
		{
			return "[GenericTerrain info]"
			+ "\n[Loc]\t" + this.Loc.ToString ()
			+ "\n[Scale]\t" + this.Scale.ToString ()
			+ "\n[avgHeight]\t" + this.Scale.y.ToString ()
			+ "\t[avgSize:x]\t" + this.Scale.x.ToString ()
			+ "\n[sizeFactor]\t" + sizeFactor.ToString ();
		}

		public void _debug (string message)
		{ 
			if (debug) {
				Debug.Log ("[GenericTerrain log]\t\t" + message);
				Debug.Log (this.ToString ());
			}
		}
	}
}
