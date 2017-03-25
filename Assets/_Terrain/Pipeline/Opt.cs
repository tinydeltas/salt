using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add this script to an object with the Nav script attached. 
namespace Pipeline {
public class Opt : MonoBehaviour {
	// optimization opts
	public int maxTiles = 5;
	public int maxMeshes = 20;

	private Nav n; 

	void Start () {
		ScriptableObject s = GetComponent<ScriptableObject> (); 
	}
	
	// Update is called once per frame
	void Update () {
		// clean up 
		
	}
}

}
