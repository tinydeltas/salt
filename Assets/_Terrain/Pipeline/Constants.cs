using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum Quadrant{
//	None, UpperLeft, UpperRight, LowerLeft, LowerRight
//}
//
public enum Dir {
	None,
	TopLeft, Top, TopRight, 
	Left, Right, 
	BottomLeft, Bottom, BottomRight
}

public enum NeighborType {
	Complete, vonNeumann
}

