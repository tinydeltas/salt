using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;

namespace Pipeline
{
	public class OceanTile
	{
		
		
		public static Dictionary<Dir, Vector2> DirVecs = new Dictionary<Dir, Vector2> {
			{Dir.TopLeft, new Vector2 (-1, 1)}, 
			{Dir.Top, new Vector2 (0, 1)}, 
			{Dir.TopRight, new Vector2 (1, 1)}, 
			{Dir.Left, new Vector2 (-1, 0)}, 
			{Dir.Right, new Vector2 (1, 0)}, 
			{Dir.BottomLeft, new Vector2 (-1, -1)}, 
			{Dir.Bottom, new Vector2 (0, -1)}, 
			{Dir.BottomRight, new Vector2 (1, -1)}
		};
			
		// when this tile was created 
		[SerializeField]
		private System.DateTime created = System.DateTime.Now;

		// generate random ID (might be useful later on?) 
		[SerializeField] 
		private System.Guid id = System.Guid.NewGuid(); 

		// initialize a new tile, which controls the islands and stuff
		public OceanTile (Vector3 init, float size, Seeder s)
		{
			Debug.Log ("[Tile] Initializing");
			Debug.Log ("[Tile] Loc: " + init.ToString ());

			Coor = init;
			Size = size;
			Scale = new Vector3 (size/10, 0.1f, size/10);

			activeNeighbors = new Dictionary<Vector2, OceanTile> ();
			activeIslands = new LinkedList<Island> (); 

			// initialize islands 
			LinkedList<Vector3> islePos = s.Seed (Coor, Size);
			foreach (Vector3 p in islePos) {
				Debug.Log ("[Tile] received isle pos: " + p.ToString ());
				activeIslands.AddFirst (new Island (p));  
			}
		}
			
		//==============================================
		//getter/setter functions

		// its location on global map
		[SerializeField]
		public Vector3 Coor { get; private set; }

		// size of each side of the tile 
		[SerializeField]
		public float Size { get; private set; }

		[SerializeField]
		public Vector3 Scale { get; private set; }

		// its active islands
		[SerializeField]
		public  LinkedList<Island> activeIslands { get; private set; } 

		// its active neighbors
		[SerializeField]
		public Dictionary<Vector2, OceanTile> activeNeighbors { get; private set; }


		//==============================================
		// interfacing with class members

		public bool AddNeighbor (Dir d, OceanTile t)
		{
			Vector2 dirVec = DirVecs [d];
			return AddNeighbor (dirVec, t);
		}

		public bool AddNeighbor(Vector2 dirVec, OceanTile t){
			if (activeNeighbors.ContainsKey(dirVec)) {
				Debug.Log ("[Tile] [AddNeighbor] Trying to replace a neighbor which already exists.");
				return false;
			}
			activeNeighbors.Add (dirVec, t); 
			return true;
		}

		public void RemoveNeighbor (Dir d)
		{
			RemoveNeighbor(DirVecs[d]);
		}
			
		public void RemoveNeighbor(Vector2 d) {
			Debug.Log ("[Tile] [RemoveNeighbor] for: " + this.ToString ()); 
			activeNeighbors.Remove (d);
		}

		//==============================================
		// state

		// deactivation for optimization purposes 
		public void deactivateTile ()
		{
			Debug.Log ("[Tile] Deactivating: " + this.ToString());

			// deactivate islands 
			foreach (Island i in activeIslands) {
				i.DestroyIsland ();
			}

			// deactivate ocean thingies? 
		}
		
		//==============================================
		// UTIL

		// checks whether a loc is in tile
		public bool inTile (Vector3 loc)
		{
//			Debug.Log ("Vector: " + loc.ToString () + " Coor:" + Coor.ToString());
			return loc.x >= Coor.x && loc.x < (Coor.x + Size) &&
					loc.z >= Coor.z && loc.z < (Coor.z + Size);
		}

		override
		public string ToString() {
			return "[Coor]" + this.Coor + "\t[Size]" + this.Size
			+ "[#Neighbors]" + activeNeighbors.Count 
			+ "[#Islands]" + activeIslands.Count;

		}

		// checks whether a loc is within percentage of the boundary)
//		public Quadrant withinBoundary (Vector2 loc, float percent)
//		{
//			if (percent > 0.3f) {
//				Debug.LogError ("[Tile] Attempted to calculate cell boundary with unusually high percentage.");
//				return Quadrant.None;
//			}
//			; 
//
//			if (!inTile (loc)) {
//				Debug.LogError ("[Tile] Attempted to check boundary for coordinate not in tile.");
//				return Quadrant.None;
//			}
//
//			Quadrant dir = Quadrant.None;
//
//			float x = loc.x - Coor.x; 
//			float y = loc.y - Coor.y; 
//			float upper = 1f - percent;
//
//			bool xLeft = x < percent * Size;
//			bool yTop = y > upper * Size;
//
//			bool xQuadLeft = x <= Size / 2f; 
//			bool yQuadTop = y > Size / 2f;
//
//			// Upper left quadrant has two cases 
//			if (xLeft && yQuadTop ||
//			   xQuadLeft && yTop) {
//				dir = Quadrant.UpperLeft;
//			}
//
//			bool xRight = x > upper * Size;
//			bool xQuadRight = x > Size / 2f; 
//
//			// upper right 
//			if (xRight && yQuadTop ||
//			   xQuadRight && yTop) {
//				dir = Quadrant.UpperRight;
//			}
//
//			bool yBottom = y < percent * Size;
//			bool yQuadBottom = y <= Size / 2f; 
//
//			// lower left 
//			if (xLeft && yQuadBottom ||
//			   xQuadLeft && yBottom) {
//				dir = Quadrant.LowerLeft;
//			}
//			
//			// lower right
//			if (xRight && yQuadBottom ||
//			   xQuadRight && yBottom) {
//				dir = Quadrant.LowerRight; 
//			}
//
//			if (dir != Quadrant.None) {
//				Debug.Log ("[Tile] Final dir: " + dir);
//			};
//			return dir; 
//		}
	}

}
