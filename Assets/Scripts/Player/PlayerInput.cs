using UnityEngine;
using System.Collections;
using MyCompany.MyGame.UI;
using MyCompany.Common.Input;

namespace MyCompany.MyGame.Player
{
	[RequireComponent (typeof(PlayerPhysics))]
	public class PlayerInput : MonoBehaviour
	{
		#region Public Member

		public MoveStick moveStick;


		#endregion

		#region Attribute

		private bool m_stickLeft = GameDefine.DEFAULT_JOYSTICK_LEFT;
		/// <summary>
		/// 摇杆是否在屏幕左侧区域
		/// </summary>
		/// <value><c>true</c> if stick left; otherwise, <c>false</c>.</value>
		public bool stickLeft
		{
			get
			{
				return m_stickLeft;
			}
			set
			{
				if (m_stickLeft != value)
				{
//					m_stickLeft = value;
//
//					ClearTouchDelegate ();
//					stickTouch = m_stickLeft ? leftTouch : rightTouch;
//					skillTouch = m_stickLeft ? rightTouch : leftTouch;
//					AddTouchDelegate ();
				}
			}
		}

		#endregion


		#region Private Member

		private PlayerPhysics playerPhysics;

		private float screenWidth;
		private float screenHeight;
		private float halfScreenWidth;
		private float halfScreenHeight;

		private TouchHandler leftTouch;
		private TouchHandler rightTouch;
		private TouchHandler stickTouch;
		private TouchHandler skillTouch;

		private float horizonMove;

		#endregion

		void Awake ()
		{
			playerPhysics = GetComponent<PlayerPhysics> ();

			screenWidth = Screen.width;
			screenHeight = Screen.height;
			halfScreenWidth = screenWidth * 0.5f;
			halfScreenHeight = screenHeight * 0.5f;

			InitTouchHandler ();
		}

		void Start ()
		{
			
		}
			
		void Update ()
		{
			stickTouch.Update ();
			skillTouch.Update ();

			horizonMove = moveStick.HorizonMove;
			playerPhysics.Move (horizonMove);

			UnityLog.Log ("Input FrameCount : " + Time.frameCount);
		}

		void InitTouchHandler ()
		{
			leftTouch = new MouseTouchHandler (0, ScreenRect.LEFT_SCREEN);
			rightTouch = new MouseTouchHandler (1, ScreenRect.RIGHT_SCREEN);

			stickTouch = m_stickLeft ? leftTouch : rightTouch;
			skillTouch = m_stickLeft ? rightTouch : leftTouch;
//			AddTouchDelegate ();
		}

		//		void ClearTouchDelegate ()
		//		{
		//			stickTouch.onTouchHold -= OnStickMove;
		//			stickTouch.onTouchEnd -= OnStickCancle;
		//			skillTouch.onTap -= OnSkillTap;
		//		}
		//
		//		void AddTouchDelegate ()
		//		{
		//			stickTouch.onTouchHold += OnStickMove;
		//			stickTouch.onTouchEnd += OnStickCancle;
		//			skillTouch.onTap += OnSkillTap;
		//		}

		//		void OnStickMove (TouchHandler touchHandler)
		//		{
		//			if (touchHandler.Moved)
		//				moveStick.Show (touchHandler.startPos, touchHandler.Position);
		//		}

		void OnStickCancle (TouchHandler touchHandler)
		{
			moveStick.Deactive ();
		}

		void OnSkillTap (TouchHandler touchHandler)
		{
			UnityLog.Log ("Tapped");
		}

		void GestureSwipe (SwipeGesture gesture)
		{
			Debug.Log ("SwipeGesture  swipe happened " + gesture.swipeDirection + " went from " + gesture.startPosition + " to " + gesture.endPosition);
		}

		//		void GestureSwipeMove (SwipeGesture gesture)
		//		{
		//			Debug.Log ("SwipeGesture  move happened " + gesture.swipePosition + " with direction " + gesture.swipeDirection);
		//		}
		//
		//		void GestureSwipeEnd (SwipeGesture gesture)
		//		{
		//			Debug.Log ("SwipeGesture  end swipe happened " + gesture.endPosition);
		//		}
	}
}

