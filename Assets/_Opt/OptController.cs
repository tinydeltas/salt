using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TerrainLib;

public struct Job
{
	public asyncFunc func;
	public Params par;
}

public class OptController : MonoBehaviour
{
	// user options
	private bool debug = false;

	[Range (1, 10)]
	public int numThreads = 10;

	[Range (10, 1000)]
	public int opsPerIteration = 500;

	// private variables
	private static TerrainWorker[] tWorkers;

	public static List<Job> jobQueue;
	public static List <ThreadJob> doneWorkers;

	// interfacing fcns
	public static void RegisterTask (Job j)
	{
		lock (jobQueue)
			jobQueue.Add (j);
	}

	public static void RegisterTasks (List<Job> j)
	{
		lock (jobQueue)
			jobQueue.AddRange (j);
	}

	// initialization
	public static void Init ()
	{
		Debug.Log ("Making new job queue");

		jobQueue = new List<Job> (); 
		doneWorkers = new List<ThreadJob> ();
	}

	public void Start ()
	{
		int opsPerWorker = opsPerIteration / numThreads;
		tWorkers = new TerrainWorker[numThreads];

		for (int i = 0; i < numThreads; i++) {
			_debug ("Creating new thread: " + i.ToString ());
			tWorkers [i] = new TerrainWorker (opsPerWorker); 
			tWorkers [i].start ();
		}
	}

	// on each frame
	public void Update ()
	{
		// grab a couple tasks and do them. 
		for (int i = 0; i < doneWorkers.Count; i++) {
			doneWorkers [i].run ();
			doneWorkers.RemoveAt (i);
		}
	}

	private void _debug (string s)
	{
		if (debug) {
			Debug.Log ("[optController] " + s);
		}
	}
}
