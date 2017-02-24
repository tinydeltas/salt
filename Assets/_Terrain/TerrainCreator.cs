using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls basically everything
public class TerrainCreator : MonoBehaviour
{

	// basic options

	[Range (128, 1024)]
	public int resolution = 256;

	public float scale = 1;

	// noise opts / ratios
	public float value = 0.25f;
	public float perlin = 0.25f;
	public float exp = 0.25f;
	public float voro = 0.25;

	[Range (1, 8)]
	public int octaves = 1;

	[Range (2, 10)]
	public int frequency = 3;

	// island opts

	public IslandMaskMethodType type;

	[Range (0f, 1f)]
	public float islandDensity = 0.2f;

	public float avgHeight = 2;


	// texture opts
	public Gradient coloring;

	// -------------------------------------
	// Private variables
	private int res;

	void onEnable ()
	{

	}

}
