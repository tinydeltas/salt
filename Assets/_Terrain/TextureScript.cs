using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TextureLib;

public class TextureScript : MonoBehaviour
{
	public int resolution = 4;
	public TextureTypes type = 0;
	public float size = 1f;
	private TextureBuilder tb;
	private Texture2D tex;

	private int curRes = -1;

	void Awake ()
	{
		tb = TextureController.TextureClasses [(int)type];
		tex = new Texture2D (resolution, resolution);
	}
		
	// Update is called once per frame
	void Update ()
	{
		if (curRes != resolution) {
			Debug.Log ("New resolution: " + resolution.ToString ());
			tex = new Texture2D (resolution * (int) size, resolution * (int) size);

			CellularTemplate.fillTexture (tex, resolution, (int) size, tb);
			GetComponent<MeshRenderer> ().material.mainTexture = tex;

			curRes = resolution;
		}


	}
}
