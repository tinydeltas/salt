using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TextureLib;
public class TextureScript : MonoBehaviour {
	public int resolution = 128;
	public TextureTypes type = 0;

	private TextureBuilder tb; 
	private Texture2D tex;

	private int curRes = -1;

	void Awake() {
		tb = TextureController.TextureClasses[(int) type];
		tex = new Texture2D(resolution, resolution);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (curRes != resolution) {
			tex = new Texture2D (resolution, resolution);
		}

		for (int i = 0; i < resolution; i++) {
			for (int j = 0; j < resolution; j++) {
				Vector3 pt = new Vector3 (i, j, 0);
				tex.SetPixel (i, j, tb.gen (pt));
			}
		}
		tex.Apply();
		GetComponent<MeshRenderer> ().material.mainTexture = tex;
	}
}
