using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshLib;

namespace Pipeline
{
	// attaches to first person controller
	public class Nav : MonoBehaviour
	{
		[SerializeField]
		private SortedDictionary<OceanTile, bool[]> activeTiles = null;
		[SerializeField]
		private OceanTile curTile;

		private Seeder seeder;

		// basic options
		public static float tileSize = 50f;
		public static float boundarySize = 0.2f;

		// Use this for initialization
		void Start ()
		{
			Debug.Log ("[Nav] Initializing");
			seeder = new Seeder ();

			// start player at origin 
			Transform t = GetComponent<Transform> (); 
			t.position = new Vector3 (0, 0, 0);

			// allocate initial tile 
			activeTiles = new SortedDictionary<OceanTile, bool[]> ();

			OceanTile firstTile = new OceanTile (Vector2.zero, tileSize, seeder);
			activeTiles.Add (firstTile, new bool[4]);

			curTile = firstTile;
		}
	
		// Update is called once per frame
		void Update ()
		{
			Transform t = GetComponent<Transform> (); 

			// update current tile if needed 
			if (!curTile.inTile (t.position)) {
				Vector2 key = GetTileKey (t.position);
				OceanTile newTile = activeTiles [key];

				Debug.Log ("[Nav] In new tile, updating curTile");
				Debug.Log ("[Nav] Current tile coor: " + key.ToString ());

				if (newTile == null) {
					Debug.LogError ("[Nav] ActiveTiles doesn't contain current tile.");
					newTile = new OceanTile (key, tileSize, seeder);
					activeTiles.Add (key, newTile); 
				}
				curTile = newTile;
			}

			// generate surrounding tiles if within boundary
			Quadrant quad = Quadrant.None;
			if ((quad = curTile.withinBoundary (t.position, boundarySize)) != Quadrant.None) {
				Vector2 key = curTile.Coor;
				Debug.Log ("[Nav] Within boundary, generating three new tiles!");
				Debug.Log ("[Nav] Current tile coor: " + key.ToString ());
				switch (quad) {
				case Quadrant.LowerLeft: 
					//Debug.Log ("[Nav] \tfor lower left quad.");
					GenAndAddTile (key, Dir.BottomLeft); 
					GenAndAddTile (key, Dir.Left); 
					GenAndAddTile (key, Dir.Bottom);
					break; 
				case Quadrant.LowerRight: 
					//Debug.Log ("[Nav] \tfor lower right quad.");
					GenAndAddTile (key, Dir.Bottom); 
					GenAndAddTile (key, Dir.BottomRight);
					GenAndAddTile (key, Dir.Right);
					break;
				case Quadrant.UpperLeft: 
					//Debug.Log ("[Nav] \tfor upper left quad.");
					GenAndAddTile (key, Dir.Left); 
					GenAndAddTile (key, Dir.TopLeft); 
					GenAndAddTile (key, Dir.Top); 
					break; 
				case Quadrant.UpperRight: 
					//Debug.Log ("[Nav] \tfor upper right quad.");
					GenAndAddTile (key, Dir.Top); 
					GenAndAddTile (key, Dir.TopRight); 
					GenAndAddTile (key, Dir.Right);
					break;
				}
			}
			;
		}

		// assumes size of tile is constant across all tiles
		private Vector2 GetTileKey (Vector2 pos)
		{
			return new Vector2 (Mathf.Floor (pos.x / tileSize), 
				Mathf.Floor (pos.y / tileSize));
		}

		private void GenAndAddTile (Vector2 refPos, Dir d)
		{
			Vector2 init = GetNeighborTileKey (refPos, d); 
			OceanTile t = new OceanTile (init, tileSize, seeder); 

			// link the new tile with its neighbors 
			curTile.AddNeighbor (d, t);
			t.AddNeighbor (d, curTile); // todo: fix 

			// add to list of active tiles since it contains an active component 
			if (!activeTiles.ContainsKey (refPos)) {
				activeTiles.Add (refPos, t);
			}
		}

		private Vector2 GetNeighborTileKey (Vector2 refPos, Dir d)
		{
			Vector2 mult = OceanTile.DirVecs [d]; 
			return new Vector2 (refPos.x + mult.x * tileSize, 
				refPos.y + mult.y * tileSize); 
		}

	}

}
