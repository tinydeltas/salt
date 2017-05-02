using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadJob 
{
	// manages the threadpool of tasks
	// and the routing of results into the correct places

	// how to retrieve appropriate results?
	// how many results to retrieve?

	// when you want to do computation:
	// send it into the queue
	private bool isDone = false;
	private object handle = new object ();
	private System.Threading.Thread t = null;

	public ThreadJob () {

	}

	public bool done {

		get {
			bool tmp; 
			lock (handle)
				tmp = isDone; 
			return tmp;
		}

		set {
			lock (handle)
				isDone = value;
		}
	}

	public virtual void start() {
		t = new System.Threading.Thread (run); 
		t.Start (); 
	}

	public void run() {
		threadFunc (); 
		isDone = true;
		lock (OptController.doneWorkers)
			OptController.doneWorkers.Add (this);
	}

	public virtual void stop() {
		t.Abort ();
	}

	public virtual bool update() {
		if (isDone) {
			onFinish ();
			return true;
		}
		return false;
	}

	public IEnumerator WaitFor() {
		while (!update())
			yield return null;
	}
		
	protected virtual void onFinish( ){ 
	
	}

	protected virtual void threadFunc() { }
}
