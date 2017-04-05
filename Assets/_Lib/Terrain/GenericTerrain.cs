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

		private float avgSize = 5f;
		private float avgHeight = 5f;
		private float factor = 20f;

		private static float[] defRatio = new float[]{ 1.0f, 0.1f, 0f };
		private float noiseRange = 0.1f;
		public float[] noiseRatios;

		public int resolution = 32;

		public int octaves = 4;
		public float frequency = 3f;
		public float lacunarity = 2f;
		public float persistence = 0.4f;

		public static Color[] defaultColors = new Color[]{

			Color.black, Color.blue, Color.gray, Color.white
		};

		public GenericTerrain (Vector3 init, 
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
				avgSize + GetDimensions (avgSize/factor), 
				avgHeight + GetDimensions (avgHeight/factor),
				avgSize + GetDimensions (avgSize/factor) 
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

			int n = Random.Range (3, 7); 
//			int n = 3;

			GradientColorKey[] keys = new GradientColorKey[n]; 
			GradientAlphaKey[] alpha = new GradientAlphaKey[n];
//
			for (int i = 0; i < n; i++) {
				if (i % 2 == 0) {
					keys [i].color = defaultColors [Random.Range (0, defaultColors.Length)];
				} else {
					keys [i].color = Color.green;

				}
				float loc = (i + 1)/n;

				keys [i].time = loc;
				alpha [i].alpha = 1f; 
				alpha [i].time = loc;
			}

			g.SetKeys (keys, alpha);
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
				Debug.Log ("Using default color");
				coloring = Coloring;
			}

			Color[] colors = this.Mesh.colors;
			Vector3[] vertices = this.Mesh.vertices;

			float maxHeight = 0f; 
			float minHeight = 1f;

			int v = 0; 

			noiseRatios = new float[defRatio.Length]; 

			for(int i = 0; i < defRatio.Length; i++) {
				noiseRatios[i] = Mathf.Min(1f, defRatio[i] + GetDimensions(noiseRange));
			}

			for (int i = 0; i <= resolution; i++) {
				Vector3 p0 = Vector3.Lerp (vecs [0], vecs [2], i * dx); 
				Vector3 p1 = Vector3.Lerp (vecs [1], vecs [3], i * dx); 

				for (int j = 0; j <= resolution; j++) {
					Vector3 p = Vector3.Lerp (p0, p1, j * dx);

//					p = new Vector3 (p.x + Loc.x, p.y + Loc.z);

					float height = genNoise (cls, p);

					float modified = Mathf.Min(height + 0.5f, 1f);
					Color newColor = coloring.Evaluate (modified);
					colors [v] = newColor;

					if (Random.Range(0, 50) == 40) {
						Debug.Log (p.ToString ());
						Debug.Log ("Loc: " + Loc.ToString ());
						Debug.Log ("Height" + height.ToString ());
						Debug.Log ("Modified: " + modified.ToString ());
						Debug.Log ("Color: " + newColor.ToString ());
					}


					if (height > maxHeight)
						maxHeight = height; 
					if (height < minHeight)
						minHeight = height; 
					

					// THEN apply the mask
					if (mask != null) {
						height = MeshLib.Mask.Transform (p, height, mask);
					} else {
						Debug.Log ("No mask");

					}

					vertices [v].y = height;
					v++;
				}
			}
			Debug.Log ("Min height: " + minHeight.ToString ()); 
			Debug.Log ("Max height: " + maxHeight.ToString ());
			Debug.Log ("Difference: " + (maxHeight - minHeight).ToString ());
			Debug.Log ("[GenericTerrain][renderTerrain] about to set Mesh and generate triangles!");
	
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
				+ "\n[avgHeight]: " + this.avgHeight.ToString () + "\t[avgSize]: " + this.avgSize.ToString ();
		}

	}

}
