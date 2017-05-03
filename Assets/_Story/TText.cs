using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using Story;
namespace UILib
{
	public class TText : MonoBehaviour
	{

		public int pause_before{ get; private set;} 
		public int pause_after { get; private set;}
		public Color color {get; private set;}

		public Vector2 pos {get; private set;}
		public Vector2 size {get; private set;} 

		public int font_size {get; private set;}

		public TText (Color c, Vector2 pos, Vector2 size,
			int pb = 0, int pa = 0, 
			int fontsize = Story.StoryConstants.defNarFontSize)
		{
			this.pause_before = pb; 
			this.pause_before = pa; 
			this.color = c;
			this.pos = pos;
			this.size = size;
			this.font_size = fontsize;
		}
	}

}
