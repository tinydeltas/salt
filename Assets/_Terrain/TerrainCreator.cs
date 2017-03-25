using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;
using NoiseLib;

// Controls basically everything
public class TerrainCreator : MonoBehaviour
{
	[Header ("basic options")]

	[Range (10, 200)]
	public int resolution = 64;

	public float island_level = -1;

	[Range(1, 10)]
	public int num_blocks = 1;

	[Range(1, 10)] 
	public int num_workers = 1;

	public bool island_mode = true;

	// -------------------------------------
	[Header ("noise options")]

	[Range (1, 8)]
	public int octaves = 5;

	[Range (2f, 10f)]
	public float frequency = 3f;

	[Range (1f, 4f)]
	public float lacunarity = 2f;

	[Range (0f, 1f)]
	public float persistence = 0.4f;

	// -------------------------------------
	[Header ("noise ratios")]

	[Range (0f, 1f)]
	public float value_ratio = 0.25f;
	[Range (0f, 1f)]
	public float perlin_ratio = 0.25f;
	[Range (0f, 1f)]
	public float exp_ratio = 0.25f;

	[HideInInspector]
	public float[] ratios;

	public bool use_diamond_square = false;

	[Range (0f, 1f)]
	public static float diamond_ratio = 0.25f;

	// -------------------------------------
	[Header ("island shape opts")]

	public bool mask_island = true;

	public MeshLib.MaskTypes mask_type;

	[Range (0f, 1f)]
	public float island_size = 0.5f;

	// -------------------------------------
	[Header ("texture / aesthetic opts")]
	public Gradient coloring;

	// -------------------------------------
	[Header ("test opts")]

	[ContextMenuItem ("Random configuration!", "genRandom")]

	public int num_tiles = 1;

	// -------------------------------------
	// Private variables
	private int curRes = -1;
	private int curScale;
	private Mesh m = null;

	private Vector3[] verts;
	private Vector3[] norms;
	private Color[] colors;

	void genRandom ()
	{
		//todo 
	}

	// initialize terrain 
	private void init ()
	{
		if (m == null) {
			m = new Mesh (); 
			m.name = "Test mesh more"; 
			GetComponent<MeshFilter> ().mesh = m;
			Debug.Log ("Assigned mesh");
		} 
	}

	private void updateParams ()
	{
		if (resolution != curRes) {
			createNewMesh (resolution);
			curRes = resolution; 
		} 
		ratios = new float[] {
			exp_ratio,
			perlin_ratio,
			value_ratio,
		};
		Vector3 pos = GetComponent<Transform> ().position; 
		pos.y = island_level;
	}

	// every time this component is enabled
	void onEnable ()
	{
		RenderTerrain ();
	}

	public void RenderTerrain ()
	{
		init ();
		updateParams (); 
	
		// get constants 
		Vector3[] vecs = MeshUtil.Constants.UnitVectors2D;

		// extract parameters 
		IHeightMappable<Vector2>[] cls = NoiseLib.Constants.MappableClasses;

		// call the methods. 
		int v = 0; 
		float dx = 1f / resolution;

		for (int i = 0; i <= resolution; i++) {
			Vector3 p0 = Vector3.Lerp (vecs [0], vecs [2], i * dx); 
			Vector3 p1 = Vector3.Lerp (vecs [1], vecs [3], i * dx); 

//			Debug.Log ("dx: " + (i * dx).ToString ());
//			Debug.Log ("p0: " + p0.ToString () + " p1: " + p1.ToString ());

			for (int j = 0; j <= resolution; j++) {
				Vector3 p = Vector3.Lerp (p0, p1, j * dx);
				float height = genNoise (cls, p);

				if (mask_island && island_mode) {
					MeshLib.MaskMethod m = MeshLib.Mask.MaskMethods [(int)mask_type];
					height = MeshLib.Mask.Transform (p, height, island_size, m);
				}

//				Debug.Log ("p: " + p.ToString ());
//				Debug.Log ("Height: " + height.ToString ()); 
//				Debug.Log ("Current vertex: " + verts [v].ToString ());

				verts [v].y = height;
				colors [v] = coloring.Evaluate (height + 0.5f);

				v++;
			}
		}	

		Debug.Log ("Number of verts assigned in RenderTerrain" + v.ToString ()); 
		setMesh (verts, colors); 

		transform.GetComponent<MeshCollider> ().sharedMesh = m;

		Debug.Log ("Set shared mesh");
	}
		

	// creates new mesh (w triangles, etc) given the new resolution
	private void createNewMesh (int resolution)
	{
		m.Clear (); 

		int numVerts = (resolution + 1) * (resolution + 1);

		verts = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		norms = new Vector3[numVerts]; 
		colors = new Color[numVerts];

		int v = 0; // vertex enumerator 
		float u = 1f / resolution;
		for (int i = 0; i <= resolution; i++) {
			for (int j = 0; j <= resolution; j++) {
				verts [v] = new Vector3 (j * u - 0.5f, 0f, i * u - 0.5f); 
				norms [v] = Vector3.up;
				colors [v] = Color.clear;
				uv [v] = new Vector2 (j * u, i * u);

				v++;
			}
		}

		Debug.Log ("Expected vertices: " + numVerts.ToString ()); 
		Debug.Log ("Actual verts: " + v.ToString ());
		Debug.Log ("Created mesh.");
			
		setMesh (verts, colors, norms, uv);
		m.triangles = MeshLib.MeshUtil.GetTriangles (resolution);
	}

	// all-in one operation
	private void setMesh (Vector3[] vertices, Color[] colors, Vector3[] normals = null, Vector2[] uv = null)
	{
		m.colors = colors;
		m.vertices = vertices;

		if (normals != null) {
			m.normals = normals; 
		} else {
			Debug.Log ("Recalculating normals.");
			m.RecalculateNormals (); 
		}
		;

		if (uv != null) {
			Debug.Log ("Resetting uv.");
			m.uv = uv;
		}
	}

	private float genNoise (IHeightMappable<Vector2>[] cls, Vector3 point)
	{
		float finalHeight = 0f;
		for (int i = 0; i < cls.Length; i++) {
			if (ratios [i] == 0f) {
				continue;
			}
			finalHeight += noiseWithOctaves (cls [i], point, octaves, frequency, lacunarity, persistence) * ratios [i];
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
}