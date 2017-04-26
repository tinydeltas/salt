using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TerrainLib;
 
public class TerrainWorker : ThreadJob {
	int ops;

	public TerrainWorker(int ops) {
		this.ops = ops;
	}

	protected override void threadFunc() {
		List<Job> jobs;

		lock(OptController.jobQueue) {
			int amtToRemove = Mathf.Min (
				                  OptController.jobQueue.Count, 
				                  ops);
			
			jobs = OptController.jobQueue.GetRange(0, amtToRemove); 
			OptController.jobQueue.RemoveRange (0, amtToRemove); 
		};
			
		foreach (Job j in jobs) {
			// execute the function and the callbacks.
			j.func(j.par);
		}
	}

	protected override void onFinish()
	{
		Debug.Log ("Finished from thread: " + this.ToString ());
	}
}
