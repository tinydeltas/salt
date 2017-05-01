using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;
using MeshLib;
using Util;
using MaterialLib;
using TextureLib;
using Pipeline;

namespace TerrainLib
{
	public class Params {

	}

	public class NoiseParams : Params
	{
		public Vector3 p;
		public int v;
		public MaskMethod mask;
		public GenericTerrain t;
	};

	public struct NoiseReturns
	{
		public float height;
		public Color color;
		public int v;
	}

	public delegate void asyncFunc (Params p);

	public class GenericTerrain
	{
		public static bool debug = false;
		public static bool optimize = true;
		public bool finished = false;

		//==============================================
		// PRIVATE VARIABLES

		private static float sizeFactor = 10f;
		private const int defDensity = 10;

		//==============================================
		// supposed to be private (todo)

		public static int resolution = 128;
		public static int textureResolution = 32;

		public static int octaves = 6;
		public static float frequency = 4f;
		public static float lacunarity = 2f;
		public static float persistence = 0.4f;

		//==============================================
		// CONSTRUCTOR

		public GenericTerrain (Vector3 init,
		                       Vector3 scale,  
			noiseFunc method,  
		                       TextureTypes t = TextureTypes.NoTexture,
		                       int density = defDensity)
		{
			this.Method = method; 
			this.Material = MaterialController.GenDefault ();

			// set default values 
			this.Texture = new TTexture (init, textureResolution, density, t);

			this.Loc = init;
			this.Scale = new Vector3 (
				Mathf.Max(scale.x + GetDimensions (scale.x / sizeFactor), 200), 
				Mathf.Max(scale.y + GetDimensions (scale.y / sizeFactor), 200),
				Mathf.Max(scale.z + GetDimensions (scale.z / sizeFactor), 200) 
			);

			this.Coloring = randomGradient ();

			_debug ("Initialized");
		}

		//==============================================
		// MEMBERS

		// used for scaling mesh
		public Vector3 Scale { get; set; }

		// noise method used
		public noiseFunc Method { get; protected set; }

		// center, absolute coordinates on world map
		public Vector3 Loc { get; protected set; }

		// material of the island
		public Material Material { get; protected set; }

		// texture used in rendering the island
		public TTexture Texture { get; protected set; }

		// island mesh
		public Mesh Mesh { get; private set; }
		public Color[] Colors { get; private set; } 
		public Vector3[] Vertices { get; private set; }
		public int MeshCount { get; protected set; } 

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
			float dx = 1f / resolution;

			if (coloring == null) {
				coloring = Coloring;
			}

			// counter for vertices
			int v = 0; 

			Colors = this.Mesh.colors;
			Vertices = this.Mesh.vertices;

			for (int i = 0; i <= resolution; i++) {
				Vector3 p0 = Vector3.Lerp (vecs [0], vecs [2], i * dx); 
				Vector3 p1 = Vector3.Lerp (vecs [1], vecs [3], i * dx); 

				for (int j = 0; j <= resolution; j++) {

					// localposition
					Vector3 p = Vector3.Lerp (p0, p1, j * dx);

					NoiseParams par = new NoiseParams {
						p = p,
						v = v, 
						mask = mask,
						t = this,
					}; 
				
					if (optimize) {
						OptController.RegisterTask (new Job {
							func = noiseAsyncFunc, 
							par = par,
						});
					} else {
						noiseAsyncFunc (par); 
					}

					v++;
				}
			}

			Debug.Log ("Registered all tasks: " + OptController.jobQueue.Count.ToString ());

			this.Texture.fill();
		}
			
		// async stuff
		public static void noiseAsyncFunc (Params par)
		{
			NoiseParams pa = (NoiseParams)par;
			GenericTerrain t = pa.t;
			// adjusted for global position
			Vector3 n = new Vector3 (pa.p.x + t.Loc.x, pa.p.y + t.Loc.z);

			float height = genNoise (pa.p, t.Method);
			// apply maske
			height = MeshLib.Mask.Transform (pa.p, height, pa.mask);

			// readjust height to find color
			Color newColor = t.Coloring.Evaluate (height + 0.5f);

			t.Colors [pa.v] = newColor; 
			t.Vertices [pa.v].y = height;

			if (pa.v == t.MeshCount - 1) { 
				t.Mesh.vertices = t.Vertices;
				t.Mesh.colors = t.Colors;
				t.Mesh.RecalculateNormals (); 

				t.finished = true;
			}

		}

		public static float genNoise (Vector3 point, noiseFunc method)
		{
			float freq = frequency;
			float sum = method(point * freq);

			float amplitude = 1f;
			float range = 1f;

			for (int o = 1; o < octaves; o++) {
				freq *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += method (point * freq) * amplitude;
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
				
			if (res > 0) {
				resolution = res;
			}

			this.Mesh = MeshLib.MeshUtil.NewMesh (resolution, this.Mesh);
			this.MeshCount = this.Mesh.vertices.Length;
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
