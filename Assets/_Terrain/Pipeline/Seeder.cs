using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeder
{
	public LinkedList<Vector3> init;

	public Seeder (float d = 0.9f)
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
		Debug.Log ("[Seeder] Density: " + Density.ToString ());

		Vector3 randPos = pos; 
		randPos.x += Random.Range (0, range); 
		randPos.z += Random.Range (0, range); 

		LinkedList<Vector3> seeds = new LinkedList<Vector3> (); 

		while (Mathf.PerlinNoise (randPos.x, randPos.z) > (1f - Density)) { 
			Debug.Log ("[Seeder] New loc; " + randPos.ToString ());
			seeds.AddFirst(randPos);

			randPos.x = Random.Range (pos.x, pos.x + range); 
			randPos.z = Random.Range (pos.z, pos.z + range); 
		} 
		return seeds;	
	}
}
