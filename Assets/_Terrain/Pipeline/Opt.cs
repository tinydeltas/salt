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
		private static Hashtable nodes = null;
		private static LinkedList<Vector3> visitedTiles = null;

		//==============================================
		// CONSTRUCTOR

		public Opt (int maxTiles)
		{
			visitedTiles = new LinkedList<Vector3> (); 
			nodes = new Hashtable ();

			_debug ("Initialized");
		}

		//==============================================
		// LRU CACHE

		public void UpdateCache (Vector3 coor)
		{
			this.ToString (); 

			if (!nodes.ContainsKey (coor)) {
				_debug ("Adding new entry to cache: ");
				LinkedListNode<Vector3> node = visitedTiles.AddFirst (coor);
				nodes.Add (coor, node); 
			} else {
				// exists in cache, find it and move it 
				LinkedListNode<Vector3> node = (LinkedListNode<Vector3>)nodes [coor]; 

				// patch the empty part

				// move to head
			
			}
		}

		public void ClearCache ()
		{
			// compress tiles that haven't been visited recently in the queue

			for (int i = Mathf.Min (maxTiles, nodes.Count - 1); i < nodes.Count; i++) {
				nodes.Remove (nodes [i]);
			}

		}

		//==============================================
		// UTIL

		//==============================================
		// DOCUMENTATION AND DEBUGGING

		override
		public string ToString ()
		{
			return "[Opt info]"
			+ "\n[#Tiles]\t\t" + nodes.Count.ToString ()
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
