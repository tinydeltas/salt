using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Story
{
	public class StoryController : MonoBehaviour
	{
		StoryEvent curEvent;
		GameObject nObj, hObj;

		// Use this for initialization
		void Start()
		{
//			nObj = Instantiate (Resources.Load ("Characters/narrator")) as GameObject;
//			nObj.GetComponent<Text> ().text = "";

//			hObj = Instantiate (Resources.Load ("Characters/hq")) as GameObject;
//			hObj.GetComponent<Text> ().text = "";

//			registerEvent (StoryMarker._00Initial);
		}

		public void registerEvent (StoryMarker se) {
			curEvent = StoryConstants.allEvents [se];
			showText (curEvent.lines [0]);
			curEvent.lines.RemoveAt (0);
		}

		void onMouseClick() {
			showText (curEvent.lines [0]);
			curEvent.lines.RemoveAt (0);
		}

		private void showText(Line l) {
			Text t = nObj.GetComponent<Text>(); 
			t.text = l.t;
		}
	}

}
