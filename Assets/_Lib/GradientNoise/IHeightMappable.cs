using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float noiseMethod (float x, float y);
public enum noiseMethodType
{
	Exp,
	Perlin,
	DiamondSquare,
	Value}
;

public interface IHeightMappable<T>
{
	T noise (Vector2 v);
}
