using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Story
{
	public class StoryController : MonoBehaviour
	{

		// Use this for initialization
		void OnGUI()
		{
			GUI.Label (new Rect (10, 10, 100, 100), "welcome.");
		}
	
		void __showText(StoryEvent se) {	

		}
	}

}
