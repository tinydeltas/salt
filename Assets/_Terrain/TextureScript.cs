using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TextureLib;

public class TextureScript : MonoBehaviour
{
	public int resolution = 4;
	public TextureTypes type = 0;

	[Range(1, 10)]
	public int size = 4;

	private TTexture tex;

	private int curRes = -1;

	void Awake ()
	{
		TextureController.Init ();
		OptController.Init ();
		tex = new TTexture (Vector3.zero, resolution, (int) size, type);
	}
		
	// Update is called once per frame
	void Update ()
	{
		if (curRes != resolution) {
			Debug.Log ("New resolution: " + resolution.ToString ());

			tex.fillByTile();
			curRes = resolution;
		}

		if (tex.finished) {
			Debug.Log ("finished tiing: ");
			GetComponent<MeshRenderer> ().material.mainTexture = tex.Tex;
			tex.finished = false;
		}
	}
}
