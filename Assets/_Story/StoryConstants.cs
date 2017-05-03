using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UILib;

namespace Story
{
	public class Character
	{
		public string name { get; private set; }  
		public TText style { get; private set; }

		public Character (string name, TText style)
		{
			this.name = name; 
			this.style = style;
		}
	}

	public struct StoryEvent
	{
		public List<Line> lines;
	}

	public struct Line {
		public Character c;
		public string t;
	}

	public enum StoryMarker {
		_00Initial, 
		_01Setup, 
		_02Instructions, 
		_03IslandReached, 
		_04Instructions
	}

	public class StoryConstants
	{			
		public static Vector2 defNarratePos = new Vector2 (10, 100);
		public static Vector2 defHQPos = new Vector2 (100, 50);
		public static Vector2 defNarrateSize = new Vector2 (200, 100);
		public const int defNarFontSize = 16;
		public const int defHQFontSize = 12;

		public static Color defColor = Color.black;
		public static Color hqColor = Color.blue;
		public static Color npcColor = Color.gray;

		public static TText defaultStyle = new TText (npcColor, defNarratePos, defNarrateSize);
		public static TText narratorStyle = new TText(defColor, defNarratePos, defNarrateSize); 
		public static TText hqStyle = new TText(hqColor, defHQPos, defNarrateSize);

		public static Dictionary <string, TText> allStyles = new Dictionary<string, TText> {
			{ "narrator", narratorStyle }, 
			{ "hq", hqStyle }, 
			{ "npc", defaultStyle },
		};

		public static Character jay = new Character ("Jay", allStyles ["narrator"]);
		public static Character hq = new Character ("HQ", allStyles ["hq"]);
		public static Character mom = new Character ("Mom", allStyles ["npc"]);

		public static Dictionary<string, Character> allCharacters = new Dictionary<string, Character> {
			{ "Jay", jay},
			{ "HQ", hq}, 
			{ "Mom", mom}
		};


		public static StoryEvent _00InitialEvent = new StoryEvent {
			lines = new List<Line>()
		};

		public static Dictionary<StoryMarker, StoryEvent> allEvents = 
				new Dictionary<StoryMarker, StoryEvent> {
			{StoryMarker._00Initial,  _00InitialEvent},
			{StoryMarker._01Setup, _00InitialEvent},
			{StoryMarker._02Instructions, _00InitialEvent},
			{StoryMarker._03IslandReached, _00InitialEvent },
			{StoryMarker._04Instructions, _00InitialEvent},
		};



	}
}
