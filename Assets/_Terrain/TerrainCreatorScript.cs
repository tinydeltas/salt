using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshLib;
using NoiseLib;
using TerrainLib;
using Pipeline;

// Controls basically everything
public class TerrainCreatorScript : MonoBehaviour
{
	[Header ("basic options")]

	[Range (10, 200)]
	public int resolution = 64;

	public float island_level = -1;


	[Range (1, 100)]
	public float islandSize = 50f;

	[Range (1, 100)]
	public float islandHeight = 30f;


	[Range (1, 10)]
	public int num_blocks = 1;

	[Range (1, 10)] 
	public int num_workers = 1;

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

//	public bool use_diamond_square = false;
//
//	[Range (0f, 1f)]
//	public static float diamond_ratio = 0.25f;

	// -------------------------------------
	[Header ("island shape opts")]

	public bool mask_island = true;

	public MeshLib.MaskTypes mask_type;

	// -------------------------------------
	[Header ("texture / aesthetic opts")]
	public Gradient coloring;

	// -------------------------------------
	[Header ("test opts")]

	[ContextMenuItem ("Random configuration!", "genRandom")]

	// -------------------------------------
	// Private variables
	private int curRes = -1;

	private GenericTerrain t;
	private Vector3 scale;

	void genRandom ()
	{
		//todo 
	}

	// initialize terrain
	private void init ()
	{
		if (t == null) {
			t = new GenericTerrain (GetComponent<Transform> ().position, scale); 
		} 
	}

	private void updateParams ()
	{
		if (resolution != curRes) {
			t.newTerrain (resolution); 
			GetComponent<MeshFilter> ().mesh = t.Mesh;
			curRes = resolution; 
		} 

		ratios = new float[] {
			exp_ratio,
			perlin_ratio,
			value_ratio,
		};

		Vector3 pos = GetComponent<Transform> ().position; 
		pos.y = island_level;
	
		t.resolution = resolution; 
		t.noiseRatios = ratios;
//		t.Coloring = coloring;

		//todo: update persistence, size, etc 
	}

	// every time this component is enabled
	void onEnable ()
	{
		scale = new Vector3 (islandSize, islandHeight, islandSize);
		Debug.Log ("in onEnable");
		UpdateTerrain ();
	}

	public void UpdateTerrain ()
	{
		init ();
		updateParams (); 

		// do the thing 
		Debug.Log("[TerrainCreatorScript] About to render Terrain.");

		MaskMethod mask = null; 
		if (mask_island) 
			mask = Mask.MaskMethods [(int)mask_type];
		
		t.renderTerrain (mask, coloring); 

		transform.GetComponent<MeshCollider> ().sharedMesh = t.Mesh;
		Debug.Log ("Set shared mesh");

//		for (int i = 0; i < coloring.colorKeys.Length; i++) {
//			Debug.Log ("COLOR value : " + coloring.colorKeys [i].color);
//			Debug.Log ("COLOR time : " + coloring.colorKeys [i].time);
//
//		}
//
//		for (int i = 0; i < coloring.alphaKeys.Length; i++) {
//			Debug.Log ("ALPHA value : " + coloring.alphaKeys [i].alpha);
//			Debug.Log ("ALPHA time : " + coloring.alphaKeys [i].time);
//
//		}
//
//		Debug.Log ("MODE: " + coloring.mode.ToString ()); 
	}
}