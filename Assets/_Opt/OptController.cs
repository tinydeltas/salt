using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TerrainLib;

public struct Job {
	public asyncFunc func;
	public Params par;
}

public class OptController : MonoBehaviour {
	// user options 

	[Range(1, 10)]
	public int numThreads = 4;

	[Range(10, 1000)]
	public int opsPerIteration = 400;

	// private variables 
	private static TerrainWorker[] tWorkers;
	public static List<Job> jobQueue; 

	// interfacing fcns 
	public static void RegisterTask(Job j) {
//		Debug.Log ("Adding job: " + j.ToString ());
		lock(jobQueue) 
			jobQueue.Add(j);
	}

	public static void RegisterTasks(List<Job> j) {
		lock (jobQueue) 
			jobQueue.AddRange (j);
	}

	// initialization 
	public static void Init() {
		Debug.Log ("Making new job queue");
		jobQueue = new List<Job> (); 
	}

	public void Start () {
		int opsPerWorker = opsPerIteration / numThreads;
		tWorkers = new TerrainWorker[numThreads];

		for (int i = 0; i < numThreads; i++) {
//			Debug.Log ("Creating new thread: " + i.ToString ());
			tWorkers [i] = new TerrainWorker (opsPerWorker); 
			tWorkers [i].start ();
		}
	}

	// on each frame
	public void Update() {
		// grab a couple tasks and do them. 
		foreach (TerrainWorker t in tWorkers) {
			if (t.done) {
//				Debug.Log ("Starting thread: " + t.ToString ()); 
				t.run ();
			}
		}
	}
}
