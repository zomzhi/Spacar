using System;
using UnityEngine;

namespace MyCompany.Common.Util
{
	public class MathUtils
	{
		public static bool RandomBool ()
		{
			return UnityEngine.Random.value < 0.5f;
		}
	}
}

