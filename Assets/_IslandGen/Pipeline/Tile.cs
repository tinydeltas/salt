using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pipeline
{
	public class OceanTile
	{

		public static bool debug = false;

		//==============================================
		// CONSTANTS

		public static Dictionary<Dir, Vector2> DirVecs = new Dictionary<Dir, Vector2> {
			{ Dir.TopLeft, new Vector2 (-1, 1) }, 
			{ Dir.Top, new Vector2 (0, 1) }, 
			{ Dir.TopRight, new Vector2 (1, 1) }, 
			{ Dir.Left, new Vector2 (-1, 0) }, 
			{ Dir.Right, new Vector2 (1, 0) }, 
			{ Dir.BottomLeft, new Vector2 (-1, -1) }, 
			{ Dir.Bottom, new Vector2 (0, -1) }, 
			{ Dir.BottomRight, new Vector2 (1, -1) }
		};

		//==============================================
		// ADMIN & ID
			
		// when this tile was created
		private System.DateTime created = System.DateTime.Now;

		// generate random ID (might be useful later on?)
		private System.Guid id = System.Guid.NewGuid ();

		//==============================================
		// CONSTRUCTOR

		// initialize a new tile, which controls the islands and stuff
		public OceanTile (Vector3 init, float size)
		{
			Coor = init;
			Size = size;
			Scale = new Vector3 (size / 10, 0.1f, size / 10);

			activeIslands = new List<Island> (); 

			_debug ("Initialized");
		}

		//==============================================
		// MEMBERS

		// its location on global map
		public Vector3 Coor { get; private set; }

		// size of each side of the tile
		public float Size { get; private set; }

		// how much to expand size o ftile by
		public Vector3 Scale { get; private set; }

		// its active islands
		public  List<Island> activeIslands { get; private set; }

		public GameObject waterObj { get; set; }

		//==============================================
		// state

		// deactivation for optimization purposes
		public void deactivateTile ()
		{
			_debug ("Deactivating tile: " + this.ToString ());

			// deactivate islands 
			foreach (Island i in activeIslands) {
				i.DestroyIsland ();
			}

			// deactivate ocean thingies? 
		}
		
		//==============================================
		// UTIL


		//==============================================
		// DOCUMENTATION AND DEBUGGING

		override
		public string ToString ()
		{
			return "[Tile info]"
			+ "\n[Coor]\t" + this.Coor.ToString ()
			+ "\t[Size]\t" + this.Size.ToString ()
			+ "\t[Scale]\t" + this.Scale.ToString ()
			+ "\t[Created]\t\t" + this.created.ToString ()
//			+ "\n[#Neighbors]\t\t" + activeNeighbors.Count.ToString ()
			+ "\t[#Islands]\t\t" + activeIslands.Count.ToString ()
			+ "\n";

		}

		public void _debug (string message)
		{
			if (debug) {
				Debug.Log ("[Tile log]\t\t" + message);
				Debug.Log (this.ToString ());
			}
		}
	}

}
