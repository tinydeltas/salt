using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSeeder : MonoBehaviour {

	// -------------------------------------
	// Global user parameters

	[Range(0f, 1f)]
	public float islandDensity = 0.2; 

	[Range(128, 1024)]
	public int resolution = 128; 

	// -------------------------------------
	// Private variables 
	private int res; 

	// Maybe don't want this to happen every time we hit play though 
	void onEnable () {
		
	}
}
