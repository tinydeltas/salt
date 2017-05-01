using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoiseLib;
using TerrainLib;

namespace TextureLib
{

	public enum TextureTypes
	{
		NoTexture,
		Solid,
		Cellular,
	};

	public class TextureParams : Params
	{
		public Vector3 p;
		public int count;
		public TTextureParams par;
		public Color[] colors;
	}

	public class TextureController
	{
		public static bool optimize = true;
		public static List<Job> newJobs;

		public static void Init ()
		{
			newJobs = new List<Job> ();
		}

		public static Color createColorDefault (float noise)
		{
			return new Color (noise, noise, noise, noise);
		}

		// COMPUTATION HEAVY
		// part of async optimization
		public static void fillTexture (Params par)
		{
			TTextureParams pars = (TTextureParams)par;

			TTexture t = pars.texMeta; 
			Texture2D tex = pars.tex;

			Vector3 pt = Vector3.zero;
			Color[] colors = tex.GetPixels ();

			int v = 0;
			for (int i = 0; i < tex.width; i++) {
				for (int j = 0; j < tex.height; j++) {
					pt.x = t.Loc.x + pars.localLoc.x + (float)i / t.resolution; 
					pt.y = t.Loc.z + pars.localLoc.y + (float)j / t.resolution;

					TextureParams p = new TextureParams {
						p = pt,
						par = pars, 
						count = v,
						colors=  colors,
					};

					if (optimize) {
						Job job = new Job {
							func = textureAsyncFunc, 
							par = p
						};
						newJobs.Add (job);
					} else {
						textureAsyncFunc (p);
					}
							 
					v++;
				}
			}
		
			if (optimize) {
				OptController.RegisterTasks (newJobs);
			}

			newJobs.Clear ();
		}


		public static void textureAsyncFunc (Params par)
		{
			float g = 0f;
			TextureParams pa = (TextureParams)par;
			TTexture t = pa.par.texMeta;
			Texture2D tex = pa.par.tex;

			switch (t.Type) {
			case TextureTypes.Cellular: 
				g = CellularTemplate.gen(pa.p); 
				break;
			case TextureTypes.Solid: 
				g = SolidTemplate.gen (pa.p);
				// @todo: fix to be more flexible
				break;
			}
				
			pa.colors [pa.count] = createColorDefault (g);

			int tileSize = tex.width * tex.height;
			if (pa.count == tileSize - 1) {
				tex.SetPixels (pa.colors);
				tex.Apply ();
				tex.Compress (false);

				if (tileSize == t.totalPix) {
					t.Tex = tex;
					t.finished = true;
					Debug.Log ("Finished: ");
				} else
					t.addTexture (tex, pa.par.localLoc);
			}
		}
	}
}
