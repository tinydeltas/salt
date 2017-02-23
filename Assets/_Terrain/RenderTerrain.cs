using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTerrain : MonoBehaviour {

	// -------------------------------------
	// Global user parameters

	public Gradient colors; 

	[Range(1, 8)] 
	public int octaves = 3; 

	public int frequency;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
