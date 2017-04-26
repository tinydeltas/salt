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
		public int y, z;
		public int v; 
		public int size;
		public Texture2D tex;
		public Color[] colors;
		public TextureTypes t;
	}

	public class TextureController
	{
		public static bool optimize = true;

		public static List<Job> newJobs;

		public static void Init ()
		{
			CellularTemplate.Init ();
			newJobs = new List<Job> ();
		}

		public static Color createColorDefault (float noise)
		{
			return new Color (noise, noise, noise, noise);
		}

		// COMPUTATION HEAVY
		// part of async optimization
		public static Texture2D fillTexture (Texture2D tex, 
		                                     int resolution, 
		                                     int size, 
		                                     TextureTypes t)
		{
			
			Vector3 pt = Vector3.zero;
			int texSize = tex.width * tex.height;
		

			Color[] colors = tex.GetPixels ();

			int v = 0;
			for (int i = 0; i < tex.width; i++) {
				for (int j = 0; j < tex.height; j++) {
					pt.x = (float)i / resolution; 
					pt.y = (float)j / resolution;

					TextureParams p = new TextureParams {
						p = pt,
						t = t,
						v = v, 
						size = texSize, 
						tex = tex, 
						colors = colors,
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


			if (optimize)
				OptController.RegisterTasks (newJobs);

			newJobs.Clear ();
			return tex;
		}


		public static void textureAsyncFunc (Params par)
		{
			float g = 0f;
			TextureParams pa = (TextureParams)par;

			switch (pa.t) {
			case TextureTypes.Cellular: 
				g = CellularTemplate.gen (pa.p); 
				break;
			case TextureTypes.Solid: 
				g = SolidTemplate.gen (pa.p, NoiseLib.Constants.MappableClasses [1]);
				// @todo: fix to be more flexible
				break;
			}

			pa.colors [pa.v] = createColorDefault (g);

			if (pa.v == pa.size - 1) {
				pa.tex.SetPixels (pa.colors);
				pa.tex.Apply ();
				pa.tex.Compress (false);
			}
		}
	}
}
