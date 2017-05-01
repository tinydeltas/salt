using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UILib
{
	public class Util : MonoBehaviour
	{

		public Text toText (TText t)
		{
			Text text = new Text (); 

			text.fontSize = TextConstants.defHQFontSize;
		}
	}

}
