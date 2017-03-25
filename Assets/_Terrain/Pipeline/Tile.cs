using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;

namespace Pipeline
{
	public class OceanTile 
	{

		public static SortedDictionary<Dir, Vector2> DirVecs = new SortedDictionary<Dir, Vector2> {
			{Dir.TopLeft, new Vector2 (-1, 1)}, 
			{Dir.Top, new Vector2 (0, 1)}, 
			{Dir.TopRight, new Vector2 (1, 1)}, 
			{Dir.Left, new Vector2 (-1, 0)}, 
			{Dir.Right, new Vector2 (1, 0)}, 
			{Dir.BottomLeft, new Vector2 (-1, -1)}, 
			{Dir.Bottom, new Vector2 (0, -1)}, 
			{Dir.BottomRight, new Vector2 (1, -1)}
		};
		
		// state that is kept track of within the tile
		[SerializeField]
		private LinkedList<Island> activeIslands;

		// initialize a new tile, which controls the islands and stuff
		public OceanTile (Vector2 init, float size, Seeder s)
		{
			Debug.Log ("[Tile] Initializing");
			Coor = init;
			Size = size;

			activeNeighbors = new SortedDictionary<Vector2, OceanTile> (); 

			// create set of islands 
			activeIslands = new LinkedList<Island> (); 

			LinkedList<Vector2> islePos = s.Seed (Coor, Size);
			foreach (Vector2 pos in islePos) {
				activeIslands.AddFirst (new Island (AdjustPos (pos)));  
			}

			Debug.Log ("[Tile] Islands created: " + islePos);

			//todo: add ocean tile component with correct dimensions ? 

		}

		private Vector2 AdjustPos (Vector2 islePos)
		{
			Vector2 absVec = new Vector2 (
				                Coor.x + Size * islePos.x, 
				                Coor.y + Size * islePos.y
			                );

			return absVec;
		}

		//==============================================
		//getter/setter functions

		// ID for the tile; its location on global map
		public Vector2 Coor { get; private set; }

		public float Size { get; private set; }

		[SerializeField]
		public SortedDictionary<Vector2, OceanTile> activeNeighbors {
			get;
			private set;
		}

		public void AddNeighbor (Dir d, OceanTile t)
		{
			Vector2 dirVec = DirVecs [d];
			if (activeNeighbors) {
				Debug.Log ("Trying to replace a neighbor which already exists.");
				return;
			}
			;
			activeNeighbors.Add (dirVec, t); 
		}

		//==============================================
		// for optimization purposes
		public void RemoveNeighbor (Dir d)
		{
			activeNeighbors.Remove (DirVecs[d]);
		}
			
		public void RemoveNeighbor(Vector2 d) {
			activeNeighbors.Remove (d);
		}

		public void deactivateTile ()
		{
			Debug.Log ("[Tile] Deactivating entire tile.");

			// deactivate islands 
			foreach (Island i in activeIslands) {
				i.DestroyIsland ();
			}

			// deactivate ocean thingies? 
		}
		
		//==============================================
		// UTIL

		// checks whether a loc is in tile
		public bool inTile (Vector2 loc)
		{
			return loc.x >= Coor.x && loc.x < (Coor.x + Size) &&
			loc.y >= Coor.y || loc.y < (Coor.y + Size);
		}

		// checks whether a loc is within percentage of the boundary)
		public Quadrant withinBoundary (Vector2 loc, float percent)
		{
			if (percent > 0.3f) {
				Debug.LogError ("[Tile] Attempted to calculate cell boundary with unusually high percentage.");
				return Quadrant.None;
			}
			; 

			if (!inTile (loc)) {
				Debug.LogError ("[Tile] Attempted to check boundary for coordinate not in tile.");
				return Quadrant.None;
			}

			Quadrant dir = Quadrant.None;

			float x = loc.x - Coor.x; 
			float y = loc.y - Coor.y; 
			float upper = 1f - percent;

			bool xLeft = x < percent * Size;
			bool yTop = y > upper * Size;

			bool xQuadLeft = x <= Size / 2f; 
			bool yQuadTop = y > Size / 2f;

			// Upper left quadrant has two cases 
			if (xLeft && yQuadTop ||
			   xQuadLeft && yTop) {
				dir = Quadrant.UpperLeft;
			}

			bool xRight = x > upper * Size;
			bool xQuadRight = x > Size / 2f; 

			// upper right 
			if (xRight && yQuadTop ||
			   xQuadRight && yTop) {
				dir = Quadrant.UpperRight;
			}

			bool yBottom = y < percent * Size;
			bool yQuadBottom = y <= Size / 2f; 

			// lower left 
			if (xLeft && yQuadBottom ||
			   xQuadLeft && yBottom) {
				dir = Quadrant.LowerLeft;
			}
			
			// lower right
			if (xRight && yQuadBottom ||
			   xQuadRight && yBottom) {
				dir = Quadrant.LowerRight; 
			}

			if (dir != Quadrant.None) {
				Debug.Log ("[Tile] Final dir: " + dir);
			};
			return dir; 
		}
	}

}
