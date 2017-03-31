using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;
using NoiseLib;

// Controls basically everything
public class TerrainCreatorScript : MonoBehaviour
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
			m.Clear (); 
			GetComponent<MeshFilter>().mesh = MeshUtil.NewMesh (resolution, m);
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
		UpdateTerrain ();
	}

	public void UpdateTerrain() {
		init ();
		updateParams (); 

		m = renderTerrain (); 
		MeshUtil.SetMesh (m, verts, colors); 
		transform.GetComponent<MeshCollider> ().sharedMesh = m;
		Debug.Log ("Set shared mesh");
	}

	// creates new mesh (w triangles, etc) given the new resolution
	private void createNewMesh (int resolution)
	{

	}
}