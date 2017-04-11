using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeder
{
	public static bool debug = false;

	//==============================================
	// CONSTRUCTOR

	public static void Init (float d = 0.9f)
	{
		Density = d;
		seeds = new List<Vector3> (); 

		// calculate threshold based on the desired density
		// todo
		_debug ("Initialized");
	}

	//==============================================
	// MEMBERS

	public static float Density {
		get;
		private set;
	}

	public static List<Vector3> seeds  { get; private set; }

	// seed potential island centers given the tile location.
	// return the island's coordinates in the form of a vector, or null if seeding fails.
	public static Vector3[] Seed (Vector3 pos, float range)
	{
		seeds.Clear ();

		Vector3 randPos = pos; 
		randPos.x += Random.Range (0, range); 
		randPos.z += Random.Range (0, range); 

		while (Mathf.PerlinNoise (randPos.x, randPos.z) > (1f - Density)) { 
			seeds.Add (randPos);

			randPos.x = Random.Range (pos.x, pos.x + range); 
			randPos.z = Random.Range (pos.z, pos.z + range); 
		} 

		_debug ("Seeding new islands");
		return seeds.ToArray();	
	}

	//==============================================
	// DOCUMENTATION AND DEBUGGING

	override
	public string ToString ()
	{
		return "[Seeder info]"
		+ "\n[Density]\t\t" + Density
		+ "\n[Seeds]\t\t" + seeds.Count
		+ "\n";

	}

	public static void _debug (string message)
	{
		if (debug) {
			Debug.Log ("[Seeder log]\t\t" + message);
//			Debug.Log (ToString ());
		}
	}
}
