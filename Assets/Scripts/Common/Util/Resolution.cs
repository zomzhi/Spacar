using UnityEngine;

namespace MyCompany.Common
{
	public class Resolution
	{
		#region Public attributes

		public int height;
		public int width;

		public float Aspect
		{
			get { return height / (float)width; }
		}

		#endregion

		#region Private methods

		private static void _Swap<T> (ref T param1, ref T param2)
		{
			T tmp = param1;
			param1 = param2;
			param2 = tmp;
		}

		#endregion

		#region Public methods​

		public static Resolution FromScreen ()
		{
			return new Resolution () { height = Screen.height, width = Screen.width };
		}

		public static void ApplyToScreen (Resolution res)
		{
			Debug.Log (string.Format ("Set Resolution width: {0} height: {1}", res.width, res.height));
			Screen.SetResolution (res.width, res.height, Screen.fullScreen);
		}

		public static Resolution FindGood (Resolution ref1, Resolution ref2, Resolution current, float margin, float currDpi, float minDpi)
		{
			float aspect1 = ref1.Aspect;
			float aspect2 = ref2.Aspect;

			if (aspect1 > aspect2)
			{
				_Swap (ref ref1, ref ref2);
				_Swap (ref aspect1, ref aspect2);
			}

			float currAspect = current.Aspect;

			Resolution output = new Resolution ();
			float factor = (currAspect - aspect1) / (aspect2 - aspect1);
			if (factor < margin)
			{
				output.height = ref1.height;
			}
			else if (factor > (1.0f - margin))
			{
				output.height = ref2.height;
			}
			else
			{
				output.height = Mathf.RoundToInt ((ref1.height + (ref2.height - ref1.height) * factor) * 0.25f) * 4;
			}

			if (currDpi > 0)
			{
				int minDpiHeight = Mathf.RoundToInt ((current.height * minDpi / currDpi) * 0.25f) * 4;
				if (output.height < minDpiHeight)
				{
					output.height = minDpiHeight;
				}
			}

			if (output.height > current.height)
			{
				return current;
			}

			output.width = Mathf.RoundToInt (output.height / currAspect);
			return output;
		}

		#endregion

	}
}