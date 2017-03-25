using UnityEngine;
using System.Collections;
using System;

namespace MyCompany.Common.Input
{
	public class DeviceTouchHandler : TouchHandler, ITouchEvent
	{
		/// <summary>
		/// Ctor
		/// </summary>
		public DeviceTouchHandler () : base ()
		{
			InTouch = false;
			Moved = false;
			ScreenArea = ScreenRect.WHOLE_SCREEN;
			m_phase = TouchPhase.Ended;
		}

		public DeviceTouchHandler(ScreenRect rect)
		{
			InTouch = false;
			Moved = false;
			ScreenArea = rect;
			m_phase = TouchPhase.Ended;
		}

		public event Action<TouchHandler> onTouchStart;
		public event Action<TouchHandler> onTouchHold;
		public event Action<TouchHandler> onTouchEnd;
		public event Action<TouchHandler> onTap;

		public ScreenRect ScreenArea{ get; private set; }

		int trackTouchID;
		Touch trackTouch;
		bool foundTouch = false;

		public override void Update ()
		{
			if (frameCount != Time.frameCount)
			{
				if (!InTouch)
				{
					for (int i = 0; i < UnityEngine.Input.touches.Length; i++)
					{
						Touch touch = UnityEngine.Input.GetTouch (i);
						if (touch.phase == TouchPhase.Began && ValidScreenPos(touch.position, ScreenArea))
						{
							trackTouch = touch;
							trackTouchID = touch.fingerId;
							startPos = m_position = touch.position;
							startTime = Time.realtimeSinceStartup;
							InTouch = true;
							m_phase = TouchPhase.Began;

							if (onTouchStart != null)
								onTouchStart (this);
							break;
						}
					}
				}
				else
				{
					foundTouch = false;
					for(int i = 0; i < UnityEngine.Input.touches.Length; i++)
					{
						Touch touch = UnityEngine.Input.GetTouch(i);
						if(touch.fingerId == trackTouchID)
						{
							foundTouch = true;
							trackTouch = touch;
							break;
						}
					}
					if(!foundTouch)
					{
						InTouch = false;
						m_phase = TouchPhase.Canceled;
						m_deltaPos = Vector2.zero;
						Moved = false;
						return;
					}
					m_position = trackTouch.position;
					m_deltaPos = trackTouch.deltaPosition;
					m_phase = trackTouch.phase;
					if (m_phase == TouchPhase.Moved)
					{
						if (!Moved)
						{
							Moved = true;
						}

						if (onTouchHold != null)
							onTouchHold (this);
					}
					else if (m_phase == TouchPhase.Stationary)
					{
						if (onTouchHold != null)
							onTouchHold (this);
					}
					else if (m_phase == TouchPhase.Ended || m_phase == TouchPhase.Canceled)
					{
						InTouch = false;
						Moved = false;

						if (onTap != null && m_phase == TouchPhase.Ended && (m_position - startPos).magnitude < TAP_DISTANCE_THRESHOLD &&
						    Time.realtimeSinceStartup - startTime < TAP_TIME_THRESHOLD)
							onTap (this);

						if (onTouchEnd != null)
							onTouchEnd (this);
					}
				}
			}
		}
	}
}

