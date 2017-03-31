﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshLib;

namespace Pipeline
{
	// attaches to first person controller
	public class Nav : MonoBehaviour
	{
		// constants
		public const string advancedWaterPrefabPath = "Water4Advanced";
		
		// basic options
		public NeighborType type = NeighborType.vonNeumann;
		public float tileSize = 10f;
		public bool testTile = false;
		public bool testIsland = true;
		public float islandDensity = 0.5f;
	
		[SerializeField]
		private static Dictionary<Vector3, OceanTile> activeTiles = null;
		[SerializeField]
		private static Dictionary<Vector3, OceanTile> allTiles = null;
		[SerializeField]
		private static OceanTile curTile = null;
		[SerializeField] 
		private static int totalTiles = 0;
		[SerializeField] 
		private static int numActive = 0;

		private static Dictionary<NeighborType, Dir[]> neighborToDir = 
			new Dictionary<NeighborType, Dir[]> { 
				{ NeighborType.vonNeumann, new Dir[4] { Dir.Top, Dir.Left, Dir.Right, Dir.Bottom } }, {
					NeighborType.Complete,
					new Dir[8] {
						Dir.Top,
						Dir.Left,
						Dir.Right,
						Dir.Bottom,
						Dir.TopLeft,
						Dir.TopRight,
						Dir.BottomLeft,
						Dir.BottomRight
					}
				}
			};

		private Seeder seeder;

		// Use this for initialization
		void Start ()
		{
			Debug.Log ("[Nav] Initializing");
			seeder = new Seeder (islandDensity);
			activeTiles = new Dictionary<Vector3, OceanTile> ();
			allTiles = new Dictionary<Vector3, OceanTile> ();

			// start player at origin 
			Transform t = GetComponent<Transform> (); 
			t.position = new Vector3 (0.5f, 0, 0.5f);

			// allocate initial tile
			Debug.Log ("[Nav] Adding initial tile");

			// this will introduce an exception and make the first tile set at (0, 0)
			OceanTile firstTile = addUnexploredTile (GetTileKey (t.position));
			curTile = firstTile;
		}
	
		// Update is called once per frame
		void Update ()
		{
			Vector3 position = GetComponent<Transform> ().position; 
			// update current tile if needed and display its neighborhood
			if (curTile == null || !curTile.inTile (position)) {
				Vector3 key = GetTileKey (position);

				Debug.Log ("[Nav] [update] In new tile, updating curTile with key: " + key);
			
				// get the new tile 
				OceanTile tile; 
				if (!activeTiles.ContainsKey (key)) {
					tile = addUnexploredTile (position);
				} else {
					tile = activeTiles [key];
				}
				curTile = tile;
				Debug.Log ("[Nav] \t Navigated to tile: " + curTile.ToString ());
			}
		}

		private OceanTile addUnexploredTile (Vector3 refPos)
		{
			Debug.Log ("[Nav] [addUnexploredTile] Init, OR Navigating to Unexplored tile.");
			Debug.Log ("[Nav] [addUnexploredTile] Active tiles: " + numActive.ToString ()); 
			Debug.Log ("[Nav] [addUnexploredTile] Total (displayed) tiles: " + totalTiles.ToString ());

			Vector3 init = GetTileKey (refPos);
			Debug.Log ("[Nav] \t init: " + init.ToString ());


			OceanTile t; 
			if (curTile != null) {
				Vector2 d = getDirFromVec (init, curTile.Coor);
				if (type == NeighborType.vonNeumann && isCornerDir (d)) {
					// only make new tile if it's an edge case 
					t = addNeighborTile (curTile, d);
				} else {
					// just try to figure it out using neighbors list
					//				Vector2 dir = getDirFromVec (refPos, curTile.Coor);
					Debug.Log ("Direction: " + d.ToString ());
					t = curTile.activeNeighbors [d];
				}
			} else {
				// the original! 
				t = __newTile (init);
			}
				
			// in all cases, officially added to active "explored" tiles list
			activeTiles.Add (init, t);
			numActive++;

			// add neighbors (some may already be filled in
			Dir[] dirs = neighborToDir [type];
			foreach (Dir direction in dirs) {
				addNeighborTile (t, OceanTile.DirVecs [direction]);
			}
				
			Debug.Log ("[Nav] New tile added: " + t.ToString ());
			return t;
		}

		// create and link a new tile to its neighbor based on its direction
		private OceanTile addNeighborTile (OceanTile orig, Vector2 d)
		{
			// get prospective key 
			Vector3 init = GetNeighborTileKey (orig.Coor, d);
			Vector2 o = oppositeDir (d);

			Debug.Log ("[Nav] \t initVec: " + init.ToString ());

			if (curTile != null && curTile.Coor == init) {
				// make sure this is reall....
				// don't need to make new plane, it already exists
				orig.AddNeighbor (d, curTile);
				return curTile; 
			} else if (allTiles.ContainsKey (init)) {
				// quick indexing 
				OceanTile exists = allTiles [init];
				orig.AddNeighbor (d, exists); 
				return exists;
			}

			Debug.Log ("[Nav] [addNeighborTile] adding neighbor for" + orig.ToString ()
			+ " with direction " + d);	 
	
			OceanTile t = __newTile (init);
			t.AddNeighbor (oppositeDir (d), orig);
			orig.AddNeighbor (d, t);

			return t;
		}

		//==============================================
		// DISPLAY FUNCTIONS

		// functions with two _'s interacts with gamecomponents
		private OceanTile __newTile (Vector3 init)
		{
			OceanTile t = new OceanTile (init, tileSize, seeder); 
			Debug.Log ("[Nav] [init] new tile, adding ground"); 

			// always have something to stand on for now 
			GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane); 
			__transform ("Tile", convertToAbsCoords(t.Coor), t.Scale, plane);

			if (testTile) {
				// Debug.Log ("[Nav] [init] in test mode, adding plane with random color");
				// create a plane instead of the oecan 
				Color randColor = new Color (Random.value, Random.value, Random.value); 
				plane.GetComponent<Renderer> ().material.color = randColor;
			} else {
				//Debug.Log ("[Nav] Retrieved object from: " + advancedWaterPrefabPath);
				GameObject ground = Instantiate (Resources.Load (advancedWaterPrefabPath)) as GameObject;
				__transform ("Ocean", convertToAbsCoords(t.Coor), t.Scale, ground);

				// lower the plane a little bit 
				Vector3 pos = plane.transform.position; 
				plane.transform.position = new Vector3 (pos.x, -0.75f, pos.z);
				plane.GetComponent<Renderer> ().material.color = Color.clear;
			}

			allTiles.Add (init, t); 
			totalTiles++;

			// display all the islands associated with the tile 
			foreach (Island i in t.activeIslands) {
				__newIsland (i);
			}
			return t;
		}

		private void __newIsland (Island i)
		{
			GameObject obj;
			if (testIsland) {
				Debug.Log ("[Nav] Adding new test island");
				obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				obj.GetComponent<MeshRenderer> ().material = i.material;

//				Color randColor = new Color (Random.value, Random.value, Random.value); 
//				mesh.GetComponent<Renderer> ().material.color = randColor;

			} else {
				Debug.Log ("[Nav] Adding new island");
				Mesh m = i.CreateIsland (); 
				// create empty game object 
				obj = new GameObject(); 
				obj.AddComponent<MeshFilter> ();
				obj.GetComponent<MeshFilter>().mesh = m;
			}

			__transform ("island_test" + totalTiles.ToString(), i.Loc, i.Scale, obj); 
		}

		//==============================================
		// UTIL

		private void __transform (string name, Vector3 coords, Vector3 scale, GameObject obj)
		{
			obj.name = name + " " + totalTiles.ToString ();
			obj.transform.position = coords;
			obj.transform.localScale = scale;
		}

		private bool isCornerDir (Vector2 v)
		{ 
			Dictionary<Dir, Vector2> dv = OceanTile.DirVecs;
			return v == dv [Dir.TopLeft] ||
			v == dv [Dir.TopRight] ||
			v == dv [Dir.BottomLeft] ||
			v == dv [Dir.BottomRight];
		}

		// assumes size of tile is constant across all tiles
		private Vector3 GetTileKey (Vector3 pos)
		{
			return new Vector3 (Mathf.Floor (pos.x / tileSize) * tileSize, 0f,
				Mathf.Floor (pos.z / tileSize) * tileSize);
		}

		private Vector3 GetNeighborTileKey (Vector3 refPos, Vector2 mult)
		{
			return new Vector3 (refPos.x + mult.x * tileSize, 0f,
				refPos.z + mult.y * tileSize); 
		}

		private Vector3 convertToAbsCoords (Vector3 pos)
		{
			return new Vector3 (pos.x + tileSize / 2f, pos.y, pos.z + tileSize / 2f);

		}

		private Vector2 oppositeDir (Vector2 vec)
		{
			return new Vector2 (vec.x * -1, vec.y * -1); 
		}

		private Vector2 getDirFromVec (Vector3 pos1, Vector3 pos2)
		{
			return new Vector2 ((pos1.x - pos2.x) / tileSize, 
				(pos1.z - pos2.z) / tileSize);
		}
	}

}