using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using Story;
namespace UILib
{
	public class TText : MonoBehaviour
	{

		private string s = "";
		private int pause_before, pause_after;
		private Color color;

		private Vector2 pos;

		private Text t;


		public TText (string s, 
			Color c, 
			Vector2 pos,
		             int pb = 0, int pa = 0
			)
		{
			this.s = s;
			this.pause_before = pb; 
			this.pause_before = pa; 
			this.color = c;
			this.pos = pos;
		}
	}

}
