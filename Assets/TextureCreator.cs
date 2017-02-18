using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : MonoBehaviour {
	[Range (2, 512)]
	public int resolution = 256;
	private Texture2D texture;

	// Use this for initialization
	private void OnEnable () {
		// make a new 2D texture with the given resolution, 
		texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
		texture.name = "Procedural Texture";
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;

		// assign it to the meshrenderer of the object
		FillTexture();
		GetComponent<MeshRenderer>().material.mainTexture = texture;
	}

	public void FillTexture () {
		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}

		// wow this is... computationally inefficient
		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++) {
			for (int x = 0; x < resolution; x++) {
				texture.SetPixel(x, y, new Color((x + 0.5f) * stepSize, (y + 0.5f) * stepSize, 0f));
			}
		}
		texture.Apply();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
