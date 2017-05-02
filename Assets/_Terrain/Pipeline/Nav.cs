using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.Water;

using NoiseLib;
using MeshLib;
using MaterialLib;
using TextureLib;

namespace Pipeline
{
	// attaches to first person controller
	public class Nav : MonoBehaviour
	{
		public static bool debug = false;

		//==============================================
		// SCRIPT OPTIONS
		public NeighborType neighborType = NeighborType.Complete;

		[Header ("nav / world options")]

		public bool islandMode = true;
		public bool storyMode = false;

		[Range (50, 1000)] 
		public float tileSize = 250f;

		[Range (0.3f, 0.55f)]
		public float islandDensity = 0.4f;

		[Range (100, 500)]
		public float minIslandSize = 200f;

		[Range (100, 500)]
		public float islandHeight = 200f;

		[Header ("noise options")]
		public Constants.MappableNoiseTypes noiseType = 
			NoiseLib.Constants.MappableNoiseTypes.Exp;
	
		[Header ("texture options")]
		public TextureTypes textureType = TextureTypes.Cellular;
		public int textureDensity = 10;

		// color options
		private static Color oceanColor = new Color ((float)50 / 255, (float)197 / 255, (float)213 / 255, 0.5f);
		private static Color refractColor = new Color ((float)225 / 255, (float)165 / 255, (float)190 / 255, 0.2f);
		public Color waterColor = oceanColor;

		[Header ("testing options")]
		public bool testTile = true;
		public bool testIsland = true;

		//==============================================
		// PRIVATE VARIABLES
		private List<Island> queuedIslands = null;
		private List<Island> allUntexturedIslands = null;

		private Dictionary<Vector3, OceanTile> activeTiles = null;
		private Dictionary<Vector3, OceanTile> allTiles = null;
	
		private static int totalTiles = 0;
		private static int numActive = 0;

		private Vector3 scale;
		private Vector3 curTile = Vector3.one;

		private static noiseFunc method;

		// story
		private bool firstIsland = false;

		// for optimization purposes
		private static GameObject allTrash;
		private static Material[] allSkyboxes;
		private static GameObject[] allTrashMaterials;
		private static GameObject waterObj;
		private static PhysicMaterial pm;

		//==============================================
		// CONSTANTS
		public const string advancedWaterPrefabPath = "Water4Advanced";
		public const string lighthousePrefabPath = "Prefabs/Lighthouse07/Lighthouse07";
		public const string defSkyboxPath = "Skyboxes/";
		public const string trashPath = "Aging/";
		public const string physicsPath = "Metal";

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
			VoronoiNoise.Init ();
			MaterialController.Init (); 
			TextureController.Init (); 
			OptController.Init ();
			Seeder.Init (islandDensity);

			method = Constants.NoiseFuncs [(int)noiseType];
		}

		private void InitResources ()
		{
			pm = Resources.Load (physicsPath) as PhysicMaterial;
			waterObj = Resources.Load (advancedWaterPrefabPath) as GameObject;

			__initSkyboxes ();
			__initTrash (); 

			allTrash = new GameObject ();
			allTrash.name = "all trash";
		}

		void OnEnable ()
		{
			InitModules ();
			InitResources ();

			this.queuedIslands = new List<Island> ();
			this.allUntexturedIslands = new List<Island> ();

			this.activeTiles = new Dictionary<Vector3, OceanTile> ();
			this.allTiles = new Dictionary<Vector3, OceanTile> ();

			// allocate initial tile
			Transform t = GetComponent<Transform> (); 

			// this will introduce an exception and make the first tile set at (0, 0)
			OceanTile firstTile = addUnexploredTile (GetTileKey (t.position));

			this.curTile = firstTile.Coor;
			this.scale = new Vector3 (minIslandSize, islandHeight, minIslandSize);

			// jumpstart the story 
			_debug ("Initialized");
		}
	
		// Update is called once per frame
		void Update ()
		{
			Vector3 position = GetComponent<Transform> ().position;

			// update current tile if needed and display its neighborhood
			if (curTile == Vector3.one || !inTile (curTile, position)) {
				Vector3 key = GetTileKey (position);

				_debug ("[update] In new tile, updating curTile with key: " + key);
			
				// get the new tile 
				OceanTile tile = activeTiles.ContainsKey (key) ? activeTiles [key] 
					: addUnexploredTile (position);

				// reset colors 
				Color waterColor = oceanColor; 
				Color refract = refractColor;

				if (tile.activeIslands.Count != 0) {
					waterColor = Color.red; 
					waterColor.a = 0.5f;

					refract = Random.ColorHSV ();
					refract.a = 0.1f;

					if (!firstIsland) {
						firstIsland = true;
						__storySetUp (tile.activeIslands [0]);
					}
					__changeSkybox ();
				}

				__changeWaterColor (tile.waterObj, waterColor, refract);

				curTile = tile.Coor;
			}

			__displayIslands ();
		}

		private void __initTrash ()
		{
			allTrashMaterials = new GameObject[7];

			string path;
			for (int i = 1; i < 8; i++) {
				path = trashPath + i.ToString ();
				allTrashMaterials [i - 1] = Resources.Load (path) as GameObject;
			}
		}

		private void __instantiateTrash (Island i)
		{
			if (!storyMode)
				return;
			
			Debug.Log ("Instantiating trash");

			int nTrash = Random.Range (3, 40);
			GameObject obj;

			for (int j = 0; j < nTrash; j++) {
				int n = Random.Range (1, 7);
				obj = allTrashMaterials [n - 1];
				obj = Instantiate (obj);
				Vector3 loc = i.Loc + new Vector3 (
					              Random.Range (1, i.Scale.x), 
					              50, 
					              Random.Range (1, i.Scale.z));

				__transform ("trash", loc, Vector3.one, obj); 

				__setAsParent (obj, allTrash);
			}
		}

		private void __initSkyboxes ()
		{
			allSkyboxes = new Material[14];
			string s;
			for (int i = 2; i < 16; i++) {
				s = i.ToString ();
				if (i < 10)
					s = "0" + s;
				s = defSkyboxPath + "sky" + s;
				allSkyboxes [i - 2] = Resources.Load (s) as Material;
			}
		}

		private void __changeSkybox ()
		{
			if (!storyMode)
				return; 

			RenderSettings.skybox = allSkyboxes [Random.Range (0, allSkyboxes.Length)];
			DynamicGI.UpdateEnvironment ();

			Debug.Log ("Changed skybox");
		}

		private void __storySetUp (Island i)
		{
			if (!storyMode)
				return;
			
			Debug.Log ("Instantiating lighthouse!!");

			GameObject obj = Instantiate (Resources.Load (lighthousePrefabPath)) as GameObject;
			__transform ("lighthouse", i.Loc + Vector3.one, Vector3.one * 3, obj);
		}

		private void  __displayIslands ()
		{
			for (int i = 0; i < allUntexturedIslands.Count; i++) {
				Island isl = allUntexturedIslands [i];
				if (isl.Texture.finished) {
					__applyTextureToObject (isl.Texture.Tex, isl.obj);
					allUntexturedIslands.Remove (isl);
				}
			}

			for (int i = 0; i < queuedIslands.Count; i++) {
				Island isl = queuedIslands [i];
				// display island
				if (isl.finished) {
					Debug.Log ("Island finished rendering");
					__newIsland (isl); 
					queuedIslands.Remove (isl);

					// also check to see if tex is available 
					if (isl.Texture.finished) {
						__applyTextureToObject (isl.Texture.Tex, isl.obj);
					} else if (textureType != TextureTypes.NoTexture) {
						allUntexturedIslands.Add (isl);
					}
				} 
			}
		}

		private OceanTile addUnexploredTile (Vector3 refPos)
		{
			_debug ("[addUnexploredTile] Init, OR Navigating to Unexplored tile.");

			Vector3 init = GetTileKey (refPos);

			OceanTile t; 
			if (curTile != Vector3.one) {
				Vector2 d = getDirFromVec (init, curTile);

				if (neighborType == NeighborType.vonNeumann && isCornerDir (d)) {
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
			Dir[] dirs = neighborToDir [neighborType];
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

			MeshCollider mc = plane.GetComponent<MeshCollider> ();
			mc.convex = true; 
			mc.sharedMaterial = pm;

			// initialize and display islands associated with tile
			GameObject water = null;

			if (testTile) {
				// create a plane instead of the oecan 
				Color randColor = Random.ColorHSV (); 
				plane.GetComponent<Renderer> ().material.color = randColor;
			} else {
				water = Instantiate (waterObj);

				// modify the scale bc the size of plane is different from the tile size
				Vector3 scale = new Vector3 (tileSize / 100f, 0.1f, tileSize / 100f);
				__transform ("Ocean", convertToAbsCoords (t.Coor), scale, water);

				// we don't want meshrenderer to mess things up
				Destroy (plane.GetComponent<MeshRenderer> ());

				// set the ocean tile as child to keep the inspector clean
				__setAsParent (water, plane);

				t.waterObj = water;
			}

			allTiles.Add (init, t); 
			totalTiles++;

			if (testIsland) {
				Vector3[] islePos = Seeder.Seed (t.Coor, t.Size);
				foreach (Vector3 p in islePos) {
					Debug.Log ("Queued island");
					Island i = new Island (p, scale, method, textureType, textureDensity);

					t.activeIslands.Add (i); 
					queuedIslands.Add (i);

					__instantiateTrash (i);
				}
			}
	
			return t;
		}

		private void __newIsland (Island i)
		{
			GameObject obj = null;

			if (testIsland) {
				_debug ("Adding new test island");
				obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			} else {
				_debug ("Adding new island");
				obj = __createObjWithMesh (i.Mesh);
			}

			__transform ("island", i.Loc, i.Scale, obj); 

			// apply material 
			obj.GetComponent<MeshRenderer> ().material = i.Material;
			i.obj = obj;
		}

		private void __applyTextureToObject (Texture2D tex, 
		                                     GameObject obj)
		{
			Debug.Log ("Applying texture to island.");
			if (textureType != TextureTypes.NoTexture) {
				tex.Compress (false);
				tex.filterMode = FilterMode.Trilinear; 
				tex.wrapMode = TextureWrapMode.Clamp;
				obj.GetComponent<MeshRenderer> ().material.mainTexture = tex;
			}
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
			if (water == null)
				return;
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
	}
}
