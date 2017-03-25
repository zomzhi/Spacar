using UnityEngine;
using System.Collections;
using System;

namespace MyCompany.Common.Input
{
	public enum ScreenRect
	{
		LEFT_SCREEN,
		RIGHT_SCREEN,
		WHOLE_SCREEN
	}

	public abstract class TouchHandler : ITouchEvent
	{
		public TouchHandler ()
		{
			InTouch = false;
			Moved = false;
		}

		public string debugString;

		public const float TAP_TIME_THRESHOLD = 0.25f;
		public const float TAP_DISTANCE_THRESHOLD = 10f;

		#region ITouchEvent implementation

		public event Action<TouchHandler> onTouchStart;

		public event Action<TouchHandler> onTouchHold;

		public event Action<TouchHandler> onTouchEnd;

		public event Action<TouchHandler> onTap;

		#endregion

		/// <summary>
		///  触摸发生时的屏幕坐标
		/// </summary>
		public Vector2 startPos;

		/// <summary>
		/// 触摸发生的时间
		/// </summary>
		public float startTime;

		/// <summary>
		/// 是否处于触摸状态
		/// </summary>
		/// <value><c>true</c> if in touch; otherwise, <c>false</c>.</value>
		public bool InTouch{ get; protected set; }

		protected TouchPhase m_phase;

		/// <summary>
		/// 当前触摸的阶段
		/// </summary>
		/// <value>The phase.</value>
		public TouchPhase Phase{ get { return m_phase; } }

		protected Vector2 m_position;

		/// <summary>
		/// 当前触摸的屏幕坐标
		/// </summary>
		/// <value>The position.</value>
		public Vector2 Position
		{
			get
			{
				if (!InTouch)
					return Vector2.zero;
				else
					return m_position;
			}
		}

		protected Vector2 m_deltaPos;

		/// <summary>
		/// 当前帧与上一帧的差量
		/// </summary>
		/// <value>The delta position.</value>
		public Vector2 DeltaPosition
		{
			get{ return m_deltaPos; }
		}

		/// <summary>
		/// 触摸发生后是否移动过
		/// </summary>
		/// <value><c>true</c> if moved; otherwise, <c>false</c>.</value>
		public bool Moved{ get; protected set; }

		protected int frameCount = -1;

		public virtual void Update ()
		{
			
		}


		protected bool ValidScreenPos (Vector2 pos, ScreenRect rect)
		{
			if(rect == ScreenRect.RIGHT_SCREEN)
			{
				return pos.x >= Screen.width * 0.5f && pos.x < Screen.width && pos.y >= 0 && pos.y <= Screen.height;
			}
			else if(rect == ScreenRect.LEFT_SCREEN)
			{
				return pos.x >= 0 && pos.x < Screen.width * 0.5f && pos.y >= 0 && pos.y <= Screen.height;
			}
			else
			{
				return pos.x >= 0 && pos.x <= Screen.width && pos.y >= 0 && pos.y <= Screen.height;
			}
		}
	}
}

