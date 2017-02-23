using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float IslandMaskMethod(); 

public enum IslandMaskMethodType {
	Voronoi, 
	Radial
}; 

public class IslandShapeMask : MonoBehaviour {

	// -------------------------------------
	// Global user parameters

	public IslandMaskType type; 

	[Range(0f, 1f)] 
	public float roughness = 0.3; 

	public int height = 2;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
