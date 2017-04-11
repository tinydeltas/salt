using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.Water;

using MeshLib;

namespace Pipeline
{
	// attaches to first person controller
	public class Nav : MonoBehaviour
	{
		//==============================================
		// SCRIPT OPTIONS
		public NeighborType type = NeighborType.Complete;

		// size options
		[Range (50, 500)] 
		public float tileSize = 250f;

		[Range (0.3f, 0.55f)]
		public float islandDensity = 0.4f;

		[Range (1, 500)]
		public float minIslandSize = 200f;

		[Range (1, 500)]
		public float islandHeight = 150f;

		// color options
		private static Color oceanColor = new Color ((float)50 / 255, (float)197 / 255, (float)213 / 255, 0.4f);
		private static Color refractColor = new Color ((float)225 / 255, (float)165 / 255, (float)190 / 255, 0.4f);
		public Color waterColor = oceanColor;

		// testing options
		public bool testTile = false;
		public bool testIsland = false;
	
		// optimization options
		public bool runOpt = true;

		[Range (5, 100)]
		public int maxTiles = 5;

		//==============================================
		// PRIVATE VARIABLES
	
		private Dictionary<Vector3, OceanTile> activeTiles = null;
		private Dictionary<Vector3, OceanTile> allTiles = null;

		private static int totalTiles = 0;
		private static int numActive = 0;

		private Seeder seeder;
		private Opt opt;
		private Vector3 scale;

		private OceanTile curTile = null;

		//==============================================
		// CONSTANTS

		public string advancedWaterPrefabPath = "Water4Advanced";

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

		void OnEnable ()
		{
			this.seeder = new Seeder (islandDensity);
			this.opt = new Opt (maxTiles); 

			this.activeTiles = new Dictionary<Vector3, OceanTile> ();
			this.allTiles = new Dictionary<Vector3, OceanTile> ();

			// allocate initial tile
			Transform t = GetComponent<Transform> (); 

			// this will introduce an exception and make the first tile set at (0, 0)
			OceanTile firstTile = addUnexploredTile (GetTileKey (t.position));

			this.curTile = firstTile;
			this.scale = new Vector3 (minIslandSize, islandHeight, minIslandSize);
			_debug ("Initialized");
		}
	
		// Update is called once per frame
		void Update ()
		{
			Vector3 position = GetComponent<Transform> ().position;

			// update current tile if needed and display its neighborhood
			if (curTile == null || !curTile.inTile (position)) {
				Vector3 key = GetTileKey (position);
//				__runOpt (key);

				_debug ("[update] In new tile, updating curTile with key: " + key);
			
				// get the new tile 
				OceanTile tile = activeTiles.ContainsKey (key) ? activeTiles [key] 
					: addUnexploredTile (position);

				curTile = tile;
			}
		}

		private void __runOpt (Vector3 key)
		{
			if (!runOpt)
				return;

			// register tile in optimization module 
			opt.UpdateCache (key); 
			List<Vector3> cleanups = opt.ClearCache ();

			foreach (Vector3 v in cleanups) {
				_debug ("[runOpt] destroying tile: " + v.ToString ());
				Destroy (GameObject.Find (__conObjectName ("Tile", key)));
				allTiles [v] = null;
				if (!activeTiles.Remove (v)) {
					Debug.LogError ("Coordinate " + v.ToString () + "should be in list of active tiles");
					// this should not throw an error
				}
			}
		}

		private OceanTile addUnexploredTile (Vector3 refPos)
		{
			_debug ("[addUnexploredTile] Init, OR Navigating to Unexplored tile.");

			Vector3 init = GetTileKey (refPos);

			OceanTile t; 
			if (curTile != null) {
				Vector2 d = getDirFromVec (init, curTile.Coor);

				if (type == NeighborType.vonNeumann && isCornerDir (d)) {
					// only make new tile if it's an edge case 
					t = addNeighborTile (curTile, d);
				} else {
					t = allTiles [init];
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
				
			_debug ("New tile added: " + t.ToString ());
			return t;
		}

		// create and link a new tile to its neighbor based on its direction
		private OceanTile addNeighborTile (OceanTile orig, Vector2 d)
		{
			_debug ("[addNeighborTile] adding neighbor for" + orig.ToString ()
			+ " with direction " + d);	 
			
			// get prospective key 
			Vector3 init = GetNeighborTileKey (orig.Coor, d);

			if (curTile != null && curTile.Coor == init) {
				return curTile; 
			} else if (allTiles.ContainsKey (init)) {
				return allTiles [init];
			}

			return  __newTile (init);
		}

		//==============================================
		// DISPLAY FUNCTIONS

		// functions with two _'s interacts with gamecomponents
		private OceanTile __newTile (Vector3 init)
		{
			OceanTile t = new OceanTile (init, tileSize); 
			_debug ("Adding new tile"); 

			// always have something to stand on for now 
			GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane); 
			__transform ("Tile", convertToAbsCoords (t.Coor), t.Scale, plane);

			// initialize and display islands associated with tile
			List<Vector3> islePos = seeder.Seed (t.Coor, t.Size);

			Color baseColor = waterColor; 
			foreach (Vector3 p in islePos) {
				_debug ("[init] Received isle pos: " + p.ToString ());

				Island i = new Island (p, scale);
				t.activeIslands.Add (i);  

				// display island
				GameObject islandObj = __newIsland (i);

				// make the parent of tile gameobject
				__setAsParent (islandObj, plane);

				// change the color of the water around the island
				baseColor = i.Coloring.colorKeys [4].color; // todo? 
				baseColor.a = 0.15f;
			}

			if (testTile) {
				// create a plane instead of the oecan 
				Color randColor = new Color (Random.value, Random.value, Random.value); 
				plane.GetComponent<Renderer> ().material.color = randColor;
			} else {
				GameObject water = Instantiate (Resources.Load (advancedWaterPrefabPath)) as GameObject;

				// modify the scale bc the size of plane is different from the tile size
				Vector3 scale = new Vector3 (tileSize / 100f, 0.1f, tileSize / 100f);
				__transform ("Ocean", convertToAbsCoords (t.Coor), scale, water);

				// lower the plane a little bit 
				Vector3 pos = plane.transform.position; 
				plane.transform.position = new Vector3 (pos.x, -1f, pos.z);

				// we don't want meshrenderer to mess things up
				Destroy (plane.GetComponent<MeshRenderer> ());

				// set the ocean tile as child to keep the inspector clean
				__setAsParent (water, plane);

				// change water color to be gross 
				__changeWaterColor (water, baseColor, refractColor);
			}

			allTiles.Add (init, t); 
			totalTiles++;

			return t;
		}

		private GameObject __newIsland (Island i)
		{
			GameObject obj;
			if (testIsland) {
				_debug ("Adding new test island");
				obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			} else {
				_debug ("Adding new island");
				Mesh m = i.CreateAndDisplayIsland (); 
				// create empty game object 
				obj = __createObjWithMesh (m);
			}

			obj.GetComponent<MeshRenderer> ().material = i.material;
			__transform ("island", i.Loc, i.Scale, obj); 

			return obj;
		}


		//==============================================
		// UNITY ENGINE UTIL

		private GameObject __createObjWithMesh (Mesh m)
		{
			GameObject obj = new GameObject (); 
			obj.AddComponent<MeshCollider> ().sharedMesh = m;
			obj.AddComponent<MeshFilter> ().mesh = m;
		
			obj.AddComponent<MeshRenderer> ();

			return obj;
		}

		private void __transform (string name, Vector3 coords, Vector3 scale, GameObject obj)
		{
			obj.name = __conObjectName (name, coords);
			obj.transform.position = coords;
			obj.transform.localScale = scale;
		}

		private void __setAsParent (GameObject child, GameObject parent)
		{
			child.GetComponent<Transform> ().parent = parent.transform;
		}

		private void __changeWaterColor (GameObject water, Color b, Color reflection)
		{
			WaterBase w = water.GetComponent<WaterBase> (); 
			w.sharedMaterial.SetColor ("_BaseColor", b); 
			w.sharedMaterial.SetColor ("_ReflectionColor", reflection); 
		}

		private string __conObjectName (string name, Vector3 coords)
		{
			return name + " " + coords.ToString ();
		}

		//==============================================
		// UTIL

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

		private Vector2 getDirFromVec (Vector3 pos1, Vector3 pos2)
		{
			return new Vector2 ((pos1.x - pos2.x) / tileSize, 
				(pos1.z - pos2.z) / tileSize);
		}

		//==============================================
		// DOCUMENTATION AND DEBUGGING

		override
		public string ToString ()
		{
			string s = "[Nav info]"
			           + "\n[#Active]\t\t" + activeTiles.Count.ToString ()
			           + "\t[numActive]\t\t" + numActive.ToString ()
			           + "\n[#Tiles]\t\t" + allTiles.Count.ToString ()
			           + "\t[totalTiles]\t\t" + totalTiles.ToString (); 

			if (curTile != null) {
				s += "\n[curTile]\t\t" + curTile.ToString ();
			}
			s += "\n";
			return s;
		}

		public void _debug (string message)
		{
			Debug.Log ("[Nav log]\t\t" + message);
			Debug.Log (this.ToString ());
		}
	}
}
