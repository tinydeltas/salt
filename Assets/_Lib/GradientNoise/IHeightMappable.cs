using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseLib
{
	public interface IHeightMappable<T>
	{
		float noise (T v);

		float _noise (Vector3 v);
	}


	public enum MappableTypes
	{
		Exp,
		Perlin,
		Value
	}

	public enum OtherTypes
	{
		DiamondSquare,
	}

}
