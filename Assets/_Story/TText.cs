using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
	public class TText : MonoBehaviour
	{

		private string s = "";
		private int pause_before, pause_after;
		private Color color;

		private Vector2 pos;

		private Text t;



		public TText (string s, 
		             int pb = 0, int pa = 0, 
		             Color c = Color.black, 
		             Vector2 pos = PrologueConstants.defNarratePos)
		{
			this.s = s;
			this.pause_before = pb; 
			this.pause_before = pa; 
			this.color = c;
			this.pos = pos;
		}
	}

}
