using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainSettings : MonoBehaviour {
	[Range(1, 10)]
	public int num_blocks = 1;

	[Range(1, 10)] 
	public int num_workers = 1;

	public bool island_mode = true;

	// -------------------------------------
	[Header ("noise options")]

	[Range (1, 8)]
	public int octaves = 1;

	[Range (2f, 10f)]
	public float frequency = 3f;

	[Range (1f, 4f)]
	public float lacunarity = 2f;

	[Range (0f, 1f)]
	public float persistence = 0.5f;

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

	public TerrainSettings() {
		ratios = new float[] {
			exp_ratio,
			perlin_ratio,
			value_ratio,
		};
	}

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
	[Header ("global seeding opts")]

	[Range (0f, 1f)]
	public float island_density = 0.2f;

	// -------------------------------------
	[Header ("test opts")]

	[ContextMenuItem ("Random configuration!", "genRandom")]

	public int num_tiles = 1;
}
