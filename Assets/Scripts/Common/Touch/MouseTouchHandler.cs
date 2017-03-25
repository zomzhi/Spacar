using UnityEngine;
using System.Collections;
using System;

namespace MyCompany.Common.Input
{
	public class MouseTouchHandler : TouchHandler, ITouchEvent
	{
		/// <summary>
		/// Ctor
		/// </summary>
		public MouseTouchHandler (int index) : base()
		{
			MouseButtonIndex = index;
			ScreenArea = ScreenRect.WHOLE_SCREEN;
			m_phase = TouchPhase.Ended;
		}

		public MouseTouchHandler(int index, ScreenRect rect) : base()
		{
			MouseButtonIndex = index;
			ScreenArea = rect;
			m_phase = TouchPhase.Ended;
		}

		public event Action<TouchHandler> onTouchStart;
		public event Action<TouchHandler> onTouchHold;
		public event Action<TouchHandler> onTouchEnd;
		public event Action<TouchHandler> onTap;


		private Vector2 lastMousePos;
		private Vector2 tempVec = Vector2.zero;

		public ScreenRect ScreenArea{ get; private set; }
		public int MouseButtonIndex{ get; private set; }

		public override void Update ()
		{
			// Only update once in a frame
			if (frameCount != Time.frameCount)
			{
				if (UnityEngine.Input.GetMouseButtonDown (MouseButtonIndex))
				{
					tempVec.x = UnityEngine.Input.mousePosition.x;
					tempVec.y = UnityEngine.Input.mousePosition.y;
					if(ValidScreenPos(tempVec, ScreenArea))
					{
						m_position = lastMousePos = startPos = tempVec;
						startTime = Time.realtimeSinceStartup;
						m_phase = TouchPhase.Began;
						InTouch = true;
						Moved = false;
						m_deltaPos = Vector2.zero;
						if (onTouchStart != null)
							onTouchStart (this);
					}
				}
				else if (UnityEngine.Input.GetMouseButton (MouseButtonIndex))
				{
					m_position.x = UnityEngine.Input.mousePosition.x;
					m_position.y = UnityEngine.Input.mousePosition.y;

					if (!ValidScreenPos (Position, ScreenRect.WHOLE_SCREEN))
					{
						if (InTouch)
						{
							if (onTouchEnd != null)
								onTouchEnd (this);
						}
						m_phase = TouchPhase.Canceled;
						InTouch = false;
						lastMousePos = m_position;
					}
					else if(InTouch)
					{
						if (lastMousePos == m_position)
						{
							m_phase = TouchPhase.Stationary;
							m_deltaPos = Vector2.zero;
						}
						else
						{
							m_phase = TouchPhase.Moved;
							m_deltaPos = m_position - lastMousePos;
							if (!Moved)
								Moved = true;
						}
						if (onTouchHold != null)
							onTouchHold (this);
						lastMousePos = m_position;
					}
				}
				else if (UnityEngine.Input.GetMouseButtonUp (MouseButtonIndex))
				{
					Moved = false;
					m_position.x = UnityEngine.Input.mousePosition.x;
					m_position.y = UnityEngine.Input.mousePosition.y;
					if (InTouch)
					{
						m_phase = TouchPhase.Ended;
						InTouch = false;
						m_deltaPos = m_position - lastMousePos;

						if (onTap != null && (m_position - startPos).magnitude < TAP_DISTANCE_THRESHOLD &&
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

