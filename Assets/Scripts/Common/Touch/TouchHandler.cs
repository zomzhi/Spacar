using System;
using UnityEngine;

namespace MyCompany.Common.Input
{
	public enum ScreenRect
	{
		LEFT_SCREEN,
		RIGHT_SCREEN,
		WHOLE_SCREEN
	}

	/// <summary>
	/// 管理屏幕上的touch行为
	/// 可模拟鼠标点击与移动设备的触摸
	/// *** 此种写法使用了过多的宏来区分代码块，其实还可以将公有的属性抽象成接口，
	/// *** 派生两个子类鼠标触摸与触屏触摸类来实现具体逻辑，
	/// *** 根据具体平台来实例化相应的实例去完成功能。
	/// </summary>
	public class TouchHandler
	{
		public const float TAP_TIME_THRESHOLD = 0.25f;
		public const float TAP_DISTANCE_THRESHOLD = 50f;

		/// <summary>
		/// 触摸发生时的屏幕坐标
		/// </summary>
		public Vector2 startPos;

		/// <summary>
		/// 触摸发生的时间
		/// </summary>
		public float startTime;

		/// <summary>
		/// 触摸允许的屏幕区域
		/// </summary>
		public ScreenRect touchRect;

		/// <summary>
		/// 是否处于触摸状态
		/// </summary>
		/// <value><c>true</c> if in touch; otherwise, <c>false</c>.</value>
		public bool InTouch{ get; private set; }

		private TouchPhase m_phase;
		/// <summary>
		/// 当前触摸的阶段
		/// </summary>
		public TouchPhase Phase{ get { return m_phase; } }

		private Vector2 m_position;
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

		private Vector2 m_deltaPos;
		/// <summary>
		/// 当前帧与上一帧位置的差量
		/// </summary>
		/// <value>The delta position.</value>
		public Vector2 DeltaPosition
		{
			get
			{
				#if UNITY_EDITOR || UNITY_STANDALONE_WIN
				return m_deltaPos;
				#else
				return touch.deltaPosition;
				#endif
			}
		}

		/// <summary>
		/// 触摸发生后是否移动过
		/// </summary>
		/// <value><c>true</c> if moved; otherwise, <c>false</c>.</value>
		public bool Moved{ get; private set; }

		#region Delegate Events

		public event Action<TouchHandler> onTouchStart;
		public event Action<TouchHandler> onTouchHold;
		public event Action<TouchHandler> onTouchEnd;
		public event Action<TouchHandler> onTap;

		#endregion

		#region Private Member

		private Touch touch;
		private int frameCount = -1;

		/// <summary>
		/// 上一帧的触摸位置
		/// </summary>
		private Vector2 lastPosition;

		#endregion


		/// <summary>
		/// ctor
		/// </summary>
		public TouchHandler () : this (ScreenRect.WHOLE_SCREEN)
		{
		}

		/// <summary>
		/// ctor
		/// </summary>
		public TouchHandler (ScreenRect touchRect)
		{
			this.touchRect = touchRect;
		}

		public void Update ()
		{
			// Only update once in a frame
			if (frameCount != Time.frameCount)
			{
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					InTouch = false;
				}

				// 更新触摸参数
				Vector2 curPos = new Vector2 (UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
				if (InTouch)
				{
					#if UNITY_EDITOR || UNITY_STANDALONE_WIN
					if (UnityEngine.Input.GetMouseButtonUp (0) || !UnityEngine.Input.GetMouseButton (0))
						EndTouch ();
					m_position = curPos;
					#else
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
						EndTouch();
					m_position = touch.position;
					#endif

					m_deltaPos = m_position - lastPosition;
					lastPosition = m_position;
					if (!Moved && InTouch && Position != startPos)
						Moved = true;

					if (!InTouch)
					{
						if (onTouchEnd != null)
							onTouchEnd (this);

						if ((m_position - startPos).magnitude < TAP_DISTANCE_THRESHOLD &&
						    (Time.realtimeSinceStartup - startTime) < TAP_TIME_THRESHOLD && onTap != null)
							onTap (this);
					}

					if (InTouch && onTouchHold != null)
						onTouchHold (this);
				}
				else if (ValidTouchOccur ())
				{
					#if UNITY_EDITOR || UNITY_STANDALONE_WIN
					startPos = m_position = new Vector2 (UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
					m_phase = TouchPhase.Began;
					#else
					startPos = m_position = touch.position;
					m_phase = touch.phase;
					#endif
					InTouch = true;
					startTime = Time.realtimeSinceStartup;

					if (onTouchStart != null)
						onTouchStart (this);
				}

				frameCount = Time.frameCount;
			}
		}

		private bool ValidTouchOccur ()
		{
			float halfScreenWdith = Screen.width * 0.5f;

			#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			Vector2 curPos = new Vector2 (UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
			if (UnityEngine.Input.GetMouseButtonDown (0))
			{
				if (touchRect == ScreenRect.LEFT_SCREEN && curPos.x < halfScreenWdith)
					return true;
				if (touchRect == ScreenRect.RIGHT_SCREEN && curPos.x > halfScreenWdith)
					return true;
				if (touchRect == ScreenRect.WHOLE_SCREEN)
					return true;
			}
			#else
			foreach (Touch tempTouch in UnityEngine.Input.touches)
			{
				if (tempTouch.phase == TouchPhase.Began && touchRect == ScreenRect.LEFT_SCREEN && tempTouch.position.x < halfScreenWdith)
				{
					touch = tempTouch;
					return true;
				}
				if (tempTouch.phase == TouchPhase.Began && touchRect == ScreenRect.RIGHT_SCREEN && tempTouch.position > halfScreenWdith)
				{
					touch = tempTouch;
					return true;
				}
				if (tempTouch.phase == TouchPhase.Began && touchRect == ScreenRect.WHOLE_SCREEN)
				{
					touch = tempTouch;
					return true;
				}
			}
			#endif
			return false;
		}

		private void EndTouch ()
		{
			InTouch = false;
			m_phase = TouchPhase.Ended;
			Moved = false;
		}
	}
}

