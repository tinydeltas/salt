using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeder
{
	public LinkedList<Vector3> init;

	public Seeder (float d = 0.6f)
	{
		Density = d;
		init = new LinkedList<Vector3> ();

		// calculate threshold based on the desired density
		// todo
	}

	//getter/setter
	public float Density {
		get;
		private set;
	}

	// seed potential island centers given the tile location.
	// return the island's coordinates in the form of a vector, or null if seeding fails.
	public LinkedList<Vector3> Seed (Vector3 pos, float range)
	{
		Vector3 randPos = pos;
		LinkedList<Vector3> seeds = new LinkedList<Vector3> (); 
		while (Mathf.PerlinNoise (randPos.x, randPos.z) > (1f - Density)) { 
			Debug.Log ("[Seeder] New loc; " + randPos.ToString ());
			seeds.AddLast(randPos);
		} 
		return seeds;	
	}
}
