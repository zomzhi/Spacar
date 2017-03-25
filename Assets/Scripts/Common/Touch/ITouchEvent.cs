using UnityEngine;
using System.Collections;
using System;

namespace MyCompany.Common.Input
{
	public interface ITouchEvent
	{
		event Action<TouchHandler> onTouchStart;
		event Action<TouchHandler> onTouchHold;
		event Action<TouchHandler> onTouchEnd;
		event Action<TouchHandler> onTap;
	}
}

