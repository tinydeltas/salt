using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeder
{
	static float threshold = 0.5f;

	public Seeder (float d = 0.3f)
	{
		Density = d;

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
	public LinkedList<Vector2> Seed (Vector2 pos, float range)
	{
		Vector2 randPos = pos;
		LinkedList<Vector2> seeds = new LinkedList<Vector2> (); 
		while (randPos == pos || Mathf.PerlinNoise (randPos.x, randPos.y) > threshold) {
			randPos.x = pos.x + Random.Range(0, range); 
			randPos.y = pos.y + Random.Range(0, range); 
			seeds.AddLast(randPos);
		} 
		return seeds;	
	}
}
