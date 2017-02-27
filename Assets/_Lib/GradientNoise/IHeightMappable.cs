using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeightMappable<T>
{
	float noise (T v);
}
