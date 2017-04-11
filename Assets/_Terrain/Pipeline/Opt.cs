using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add this script to an object with the Nav script attached.
namespace Pipeline
{
	public class Opt
	{
		//==============================================
		// PRIVATE VARIABLES

		private int maxTiles = 10;
		//	public int maxMeshes = 20;

		// used to implement lru cache
		private static List<Vector3> visitedTiles = null;

		//==============================================
		// CONSTRUCTOR

		public Opt (int maxTiles)
		{
			visitedTiles = new List<Vector3>(); 

			_debug ("Initialized");
		}

		//==============================================
		// LRU CACHE

		public void UpdateCache (Vector3 coor)
		{
			_debug ("Updating cache");

			// exists in cache, find it and move it 
			if (visitedTiles.Contains(coor))
				visitedTiles.Remove(coor); 

			// move to head
			visitedTiles.Insert(0, coor);
		}

		public List<Vector3> ClearCache ()
		{
			// compress tiles that haven't been visited recently in the queue
			List<Vector3> cleanup = new List<Vector3>(); 

			for (int i = maxTiles; i < visitedTiles.Count; i++) {
				Vector3 key = visitedTiles [i]; 
				visitedTiles.Remove (key);
				cleanup.Add(key);

				_debug ("Removing: " + key.ToString ()); 
			}

			return cleanup;
		}

		//==============================================
		// UTIL

		//==============================================
		// DOCUMENTATION AND DEBUGGING

		override
		public string ToString ()
		{
			return "[Opt info]"
				+ "\n[#Tiles]\t\t" + visitedTiles.Count.ToString ()
			+ "\n[Max allowed]\t\t" + maxTiles.ToString ()
			+ "\n";
		}

		public void _debug (string message)
		{
			Debug.Log ("[Opt log]\t\t" + message);
			Debug.Log (this.ToString ());
		}
	}
}
