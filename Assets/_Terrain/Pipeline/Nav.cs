using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.Water;

using MeshLib;
using MaterialLib;

namespace Pipeline
{
	// attaches to first person controller
	public class Nav : MonoBehaviour
	{
		public static bool debug = false;

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
		public bool testTexture = true;
	
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

		private Vector3 scale;

		private Vector3 curTile = Vector3.one;

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

		private void InitModules ()
		{
			// init everything here so they don't need to be instantiated later 
			MaterialController.Init (); 
			Seeder.Init (islandDensity);
		}

		void OnEnable ()
		{
			InitModules (); 
//			this.opt = new Opt (maxTiles); 

			this.activeTiles = new Dictionary<Vector3, OceanTile> ();
			this.allTiles = new Dictionary<Vector3, OceanTile> ();

			// allocate initial tile
			Transform t = GetComponent<Transform> (); 

			// this will introduce an exception and make the first tile set at (0, 0)
			OceanTile firstTile = addUnexploredTile (GetTileKey (t.position));

			this.curTile = firstTile.Coor;
			this.scale = new Vector3 (minIslandSize, islandHeight, minIslandSize);
			_debug ("Initialized");
		}
	
		// Update is called once per frame
		void Update ()
		{
			Vector3 position = GetComponent<Transform> ().position;

			// update current tile if needed and display its neighborhood
			if (curTile == Vector3.one || !inTile (curTile, position)) {
				Vector3 key = GetTileKey (position);
//				__runOpt (key);

				_debug ("[update] In new tile, updating curTile with key: " + key);
			
				// get the new tile 
				OceanTile tile = activeTiles.ContainsKey (key) ? activeTiles [key] 
					: addUnexploredTile (position);

				curTile = tile.Coor;
			}
		}

		//		private void __runOpt (Vector3 key)
		//		{
		//			if (!runOpt)
		//				return;
		//
		//			// register tile in optimization module
		//			opt.UpdateCache (key);
		//			List<Vector3> cleanups = opt.ClearCache ();
		//
		//			foreach (Vector3 v in cleanups) {
		//				_debug ("[runOpt] destroying tile: " + v.ToString ());
		//				Destroy (GameObject.Find (__conObjectName ("Tile", key)));
		//				allTiles [v] = null;
		//				if (!activeTiles.Remove (v)) {
		//					Debug.LogError ("Coordinate " + v.ToString () + "should be in list of active tiles");
		//					// this should not throw an error
		//				}
		//			}
		//		}

		private OceanTile addUnexploredTile (Vector3 refPos)
		{
			_debug ("[addUnexploredTile] Init, OR Navigating to Unexplored tile.");

			Vector3 init = GetTileKey (refPos);

			OceanTile t; 
			if (curTile != Vector3.one) {
				Vector2 d = getDirFromVec (init, curTile);

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
				addNeighborTile (t.Coor, OceanTile.DirVecs [direction]);
			}
				
			_debug ("New tile added");
			return t;
		}

		// create and link a new tile to its neighbor based on its direction
		private OceanTile addNeighborTile (Vector3 orig, Vector2 d)
		{
			_debug ("[addNeighborTile] adding neighbor "); 
			
			// get prospective key 
			Vector3 init = GetNeighborTileKey (orig, d);

			if (curTile != Vector3.one && curTile == init) {
				return allTiles [curTile]; 
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
			GameObject water = null;

			if (testTile) {
				// create a plane instead of the oecan 
				Color randColor = new Color (Random.value, Random.value, Random.value); 
				plane.GetComponent<Renderer> ().material.color = randColor;
			} else {
				water = Instantiate (Resources.Load (advancedWaterPrefabPath)) as GameObject;

				// modify the scale bc the size of plane is different from the tile size
				Vector3 scale = new Vector3 (tileSize / 100f, 0.1f, tileSize / 100f);
				__transform ("Ocean", convertToAbsCoords (t.Coor), scale, water);

				// we don't want meshrenderer to mess things up
				Destroy (plane.GetComponent<MeshRenderer> ());

				// set the ocean tile as child to keep the inspector clean
//				__setAsParent (water, oceanTiles);
			}
				
			allTiles.Add (init, t); 
			totalTiles++;

			Vector3[] islePos = Seeder.Seed (t.Coor, t.Size);

			Color baseColor = waterColor; 
			foreach (Vector3 p in islePos) {
//				Debug.Log("[init] Received isle pos: " + p.ToString ());

				Island i = new Island (p, scale);
				t.activeIslands.Add (i); 
			
				// change the color of the water around the island
				baseColor = i.Coloring.colorKeys [4].color; // todo? 
				baseColor.a = 0.15f;
	
				// display island
				StartCoroutine (__newIsland (i)); 
			}

			if (!testTile && baseColor != waterColor) {// change water color to be gross 
				__changeWaterColor (water, baseColor, refractColor);
			}
//
			return t;
		}

		private IEnumerator __newIsland (Island i)
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

			__transform ("island", i.Loc, i.Scale, obj); 

			// make the parent of tile gameobject
//			__setAsParent (obj, newObj);

			obj.GetComponent<MeshRenderer> ().material = i.Material;

			if (testTexture) {
				Debug.Log ("Texture: " + i.Texture.width.ToString ());
				i.Texture.filterMode = FilterMode.Trilinear; 
				i.Texture.wrapMode = TextureWrapMode.Clamp;
				obj.GetComponent<MeshRenderer> ().material.mainTexture = i.Texture;
			}
			yield return null;
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


		// checks whether a loc is in tile
		public bool inTile (Vector3 Coor, Vector3 loc)
		{
			return loc.x >= Coor.x && loc.x < (Coor.x + tileSize) &&
			loc.z >= Coor.z && loc.z < (Coor.z + tileSize);
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

		private Vector2 getDirFromVec (Vector3 pos1, Vector3 pos2)
		{
			return new Vector2 ((pos1.x - pos2.x) / tileSize, 
				(pos1.z - pos2.z) / tileSize);
		}

		private void turnAllDebugOff ()
		{
			OceanTile.debug = false;
			Seeder.debug = false; 
			Island.debug = false; 
			MeshLib.MeshUtil.debug = false; 
			TerrainLib.GenericTerrain.debug = false; 
			debug = false; 
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

			if (curTile != Vector3.one) {
				s += "\n[curTile]\t\t" + curTile.ToString ();
			}
			s += "\n";
			return s;
		}

		public void _debug (string message)
		{
			if (debug) {
				Debug.Log ("[Nav log]\t\t" + message);
				Debug.Log (this.ToString ());
			}
		}
	}
}
