using System;
using UnityEngine;

namespace MyCompany
{
	public static class UnityLog
	{
		public static bool logMessage = true;

		public static void Log (object message)
		{
			if (Debug.isDebugBuild && logMessage)
				Debug.Log (message);
		}

		public static void Log (object message, UnityEngine.Object context)
		{
			if (Debug.isDebugBuild && logMessage)
				Debug.Log (message, context);
		}

		public static void LogError (object message)
		{
			Debug.LogError (message);
		}

		public static void LogError (object message, UnityEngine.Object context)
		{
			Debug.LogError (message, context);
		}

		public static void LogWarning (object message)
		{
			Debug.LogWarning (message);
		}

		public static void LogWarning (object message, UnityEngine.Object context)
		{
			Debug.LogWarning (message, context);
		}

		public static void Assert (bool condition)
		{
			Debug.Assert (condition);
		}

		public static void Assert (bool condition, UnityEngine.Object context)
		{
			Debug.Assert (condition, context);
		}

		public static void Assert (bool condition, object message)
		{
			Debug.Assert (condition, message);
		}

		public static void Assert (bool condition, object message, UnityEngine.Object context)
		{
			Debug.Assert (condition, message, context);
		}
	}
}

