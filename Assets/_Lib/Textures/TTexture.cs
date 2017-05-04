using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TerrainLib;


namespace TextureLib
{
	public class TTextureParams : Params
	{
		public TTexture texMeta;
		public Texture2D tex;
		public Vector2 localLoc;
	}


	public class TTexture
	{
		public static bool debug = false;

		private List<Job> jobs = new List<Job> ();

		public TTexture (Vector3 loc, 
		                 int resolution, 
		                 int density, 
		                 TextureTypes type, 
		                 bool byTile = false)
		{
			
			this.resolution = resolution; 
			this.density = density;

			this.Loc = loc;
			this.Type = type;

			this.totalPix = (resolution * density) * (resolution * density);

			this.byTile = byTile;

			if (this.Type != TextureTypes.NoTexture) {
				this.Tex = new Texture2D (resolution * density, 
					resolution * density);
				this.Colors = this.Tex.GetPixels ();
			}

			this.finished = false;
		}

		public int resolution { get ; private set; }

		public int density { get ; private set; }

		public int totalPix { get; private set; }

		public Vector3 Loc { get; private set; }

		public TextureTypes Type { get; private set; }

		public Texture2D Tex { get ; set; }

		public Color[] Colors { get; private set; }

		public bool finished { get ; set ; }

		public bool byTile { get; private set; }

		public void fill ()
		{
			if (Type == TextureTypes.NoTexture)
				return;
			
			if (byTile)
				fillByTile ();
			else
				fillOne ();
		}

		public void fillOne ()
		{
			TTextureParams p = new TTextureParams {
				texMeta = this,
				tex = this.Tex,
				localLoc = Vector2.zero,
			};
				
			TextureController.fillTexture (p);
		}

		public void fillByTile ()
		{
			for (int i = 0; i < density; i++) {
				for (int j = 0; j < density; j++) {
					TTextureParams p = new TTextureParams {
						texMeta = this, 
						tex = new Texture2D (resolution, resolution),
						localLoc = new Vector2 (i, j)
					};

					Job job = new Job {
						func = TextureController.fillTexture, 
						par = p
					};
					jobs.Add (job);
				}
			}

			_debug ("# of jobs: " + jobs.Count.ToString ());
			OptController.RegisterTasks (jobs);
			jobs.Clear ();
		}

		public void addTexture (Texture2D tex, Vector2 loc)
		{
			_debug ("Merging into master tex: " + tex.width.ToString ());
		
			Color[] tPix = tex.GetPixels (); 
			for (int i = 0; i < tPix.Length; i++) {
				int nRow = (int)i / resolution;
				int nCol = i % resolution; 

				int gRow = resolution * (int)loc.x + nRow;
				int gCol = resolution * (int)loc.y + nCol; 
				int idx = gRow * this.Tex.width + gCol;
			
				Colors [idx] = tPix [i];
			}



			if (loc.x == density - 1 && loc.y == density - 1) {
				this.Tex.SetPixels (Colors); 
				this.Tex.Apply ();

				finished = true;
			}
		}

		public void _debug (string s)
		{
			if (debug) {
				Debug.Log ("[TTexture] " + s);
			}
		}
	}
}
