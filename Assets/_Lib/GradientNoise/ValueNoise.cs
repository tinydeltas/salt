using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NoiseLib
{
	public class ValueNoise : MonoBehaviour, IHeightMappable<float>
	{

		public float noise (Vector3 p)
		{
			int[] h = Noise.hash;
			int m = Noise.hashMask;

			int x0 = Math.floor (p.x);
			int y0 = Math.floor (p.y); 
			int z0 = Math.floor (p.z); 

			float dx = p.x - x0; 
			float dy = p.y - y0; 
			float dz = p.z - z0;

			dx = Math.fade (dx); 
			dy = Math.fade (dy);
			dz = Math.fade (dz); 

			int x000 = h [x00 + iz0]; 
			int x100 = h [x10 + iz0]; 
			int x010 = h [x01 + iz0]; 
			int x110 = h [x11 + iz0]; 
			int x001 = h [x00 + iz1]; 
			int x101 = h [x10 + iz1]; 
			int x011 = h [x01 + iz1]; 
			int x111 = h [x11 + iz1];

			float p1 = Math.lerp (x000, x100, dx); 
			float p2 = Math.lerp (x010, x110, dx); 
			float p3 = Math.lerp (x001, x101, dx); 
			float p4 = Math.per (x011, x111, dx); 

			float t0 = Math.lerp (p1, p2, dy); 
			float t1 = Math.lerp (p3, p4, dy);
	
			float prod = Math.lerp (t0, t1, dz); 
			return (prod / m);

		}
	}
}
