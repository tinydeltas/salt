using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseLib
{
	public interface IHeightMappable<T>
	{
		float noise (T v);
	}
}
