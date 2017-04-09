using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;
using MeshLib;
using Util;

namespace TerrainLib
{
	public class GenericTerrain
	{
		private float factor = 10f;

		private static float[] defRatio = new float[]{ 0.8f, 0.5f, 0f };
		private float noiseRange = 0.2f;
		public float[] noiseRatios;

		public int resolution = 180;

		public int octaves = 5;
		public float frequency = 3f;
		public float lacunarity = 2f;
		public float persistence = 0.4f;

		public static Color[] defaultColors = new Color[] {

			Color.black, Color.blue, Color.gray, Color.white, Color.cyan, Color.yellow
		};

		public GenericTerrain (Vector3 init,
		                       Vector3 scale, 
		                       Material mat = null, 
		                       float[] ratios = null)
		{
			if (mat == null)
				mat = new Material (Shader.Find ("TerrainShader"));
			
			if (ratios == null)
				ratios = defRatio;

			material = mat; 
			noiseRatios = ratios;

			Loc = init;
			Scale = new Vector3 (
				scale.x + GetDimensions (scale.x / factor), 
				scale.y + GetDimensions (scale.y / factor),
				scale.z + GetDimensions (scale.z / factor) 
			);

			Coloring = randomGradient ();
		}

		private float GetDimensions (float size)
		{
			return size * Math.Gaussian ();
		}

		private Gradient randomGradient ()
		{

			Gradient g = new Gradient (); 
			g.mode = GradientMode.Blend;

			int n = Random.Range (3, 8); 

			GradientColorKey[] keys = new GradientColorKey[n]; 
			for (int i = 0; i < n; i++) {
				keys [i].color = Random.ColorHSV ();
				float loc = (float)i / n;
				keys [i].time = loc;
			}

			GradientAlphaKey[] alpha = new GradientAlphaKey[2];
			alpha [0].alpha = 1f; 
			alpha [1].alpha = 1f; 
			alpha [0].time = 0f; 
			alpha [1].time = 1f;

			g.SetKeys (keys, alpha);

//			for (int i = 0; i < g.colorKeys.Length; i++) {
//				Debug.Log ("COLOR value : " + g.colorKeys [i].color);
//				Debug.Log ("COLOR time : " + g.colorKeys [i].time);
//
//			}
//
//			for (int i = 0; i < g.alphaKeys.Length; i++) {
//				Debug.Log ("ALPHA value : " + g.alphaKeys [i].alpha);
//				Debug.Log ("ALPHA time : " + g.alphaKeys [i].time);
//
//			}

			return g;
		}

		//==============================================
		// getter/setters
		// center, absolute coordinates on world map
		[SerializeField]
		public Vector3 Loc { get; protected set; }

		// used for scaling mesh
		[SerializeField]
		public Vector3 Scale { get; set; }

		[SerializeField]
		public Material material { get; private set; }

		public Mesh Mesh { get; private set; }

		public Gradient Coloring { get; private set; }

		//==============================================
		// Originally taken from TerrainCreatorScript


		public void renderTerrain (MaskMethod mask = null, Gradient coloring = null)
		{
			if (this.Mesh == null) {
				Debug.LogError ("[GenericTerrain] Mesh should not be null!!!");
				return;
			}

			Debug.Log ("[GenericTerrain][renderTerrain] about to determine the vertices and colors for the terrain."); 

			// get constants 
			Vector3[] vecs = MeshUtil.Constants.UnitVectors2D;

			// extract parameters 
			IHeightMappable<Vector2>[] cls = NoiseLib.Constants.MappableClasses;
			float dx = 1f / resolution;

			if (coloring == null) {
//				Debug.Log ("Using default color");
				coloring = Coloring;
			}

			Color[] colors = this.Mesh.colors;
			Vector3[] vertices = this.Mesh.vertices;

			float maxHeight = 0f; 
			float minHeight = 1f;

			int v = 0; 

			noiseRatios = new float[defRatio.Length]; 
			for (int i = 0; i < defRatio.Length; i++) {
				noiseRatios [i] = Mathf.Min (1f, defRatio [i] + GetDimensions (noiseRange));
			}

			for (int i = 0; i <= resolution; i++) {
				Vector3 p0 = Vector3.Lerp (vecs [0], vecs [2], i * dx); 
				Vector3 p1 = Vector3.Lerp (vecs [1], vecs [3], i * dx); 

				for (int j = 0; j <= resolution; j++) {
					Vector3 p = Vector3.Lerp (p0, p1, j * dx);

					Vector3 n = new Vector3 (p.x + Loc.x, p.y + Loc.z);
					float height = genNoise (cls, n);
//
//					if (Random.Range(0, 50) == 40) {
//						Debug.Log ("Point: " + p.ToString ());
//						Debug.Log ("Loc: " + Loc.ToString ());
//						Debug.Log ("Height" + height.ToString ());
//						Debug.Log ("Modified: " + modified.ToString ());
//						Debug.Log ("Color: " + newColor.ToString ());
//					}
//
//					if (height > maxHeight)
//						maxHeight = height; 
//					if (height < minHeight)
//						minHeight = height; 
//					
					float modified = Mathf.Min (height + 0.5f, 1f);
					// THEN apply the mask
					if (mask != null) {
						height = MeshLib.Mask.Transform (p, height, mask);
					} 
						
					Color newColor = coloring.Evaluate (modified);

					colors [v] = newColor;
					vertices [v].y = height;
					v++;
				}
			}
//			Debug.Log ("Min height: " + minHeight.ToString ()); 
//			Debug.Log ("Max height: " + maxHeight.ToString ());
//			Debug.Log ("Difference: " + (maxHeight - minHeight).ToString ());
//			Debug.Log ("[GenericTerrain][renderTerrain] about to set Mesh and generate triangles!");
	
			this.Mesh.vertices = vertices;
			this.Mesh.colors = colors;
			this.Mesh.RecalculateNormals (); 
		}


		private float genNoise (IHeightMappable<Vector2>[] cls, Vector3 point)
		{
			float finalHeight = 0f;
	
			for (int i = 0; i < cls.Length; i++) {
				if (noiseRatios [i] == 0f) {
					continue;
				}
				finalHeight += noiseWithOctaves (cls [i], point, octaves, frequency, lacunarity, persistence) * noiseRatios [i];
			}
			return finalHeight;
		}

		private float noiseWithOctaves (IHeightMappable<Vector2> cl, Vector3 point, int octaves, float freq, float lacuna, float persist)
		{
			float sum = cl.noise (point * freq);

			float amplitude = 1f;
			float range = 1f;

			for (int o = 1; o < octaves; o++) {
				freq *= lacuna;
				amplitude *= persist;
				range += amplitude;
				sum += cl.noise (point * freq) * amplitude;
			}
			return sum / range;
		}

		//==============================================
		// terrain lifecycle operations

		public void newTerrain (int res = 0)
		{
			Debug.Log ("[GenericTerrain] creating new Terrain or Resizing"); 

			if (this.Mesh == null)
				this.Mesh = new Mesh ();
			else
				this.Mesh.Clear (); 

			if (res != 0) {
				resolution = res;
			}

			this.Mesh = MeshLib.MeshUtil.NewMesh (resolution, this.Mesh);
		}

		public void destroyTerrain ()
		{
			this.Mesh = null;
		}

		override
		public string ToString ()
		{

			return "[Loc]: " + this.Loc.ToString () +
			"\n[Scale]: " + this.Scale.ToString ()
			+ "\n[avgHeight]: " + this.Scale.y.ToString () + "\t[avgSize]: " + this.Scale.x.ToString ();
		}

	}

}
