using System;
using MyCompany.Common;
using MyCompany.Common.Input;
using UnityEngine;

namespace MyCompany.MyGame
{
	/// <summary>
	/// 输入管理
	/// InputManager继承自MonoBehavior，对于输入的检测和更新放在Update函数里，
	/// 该逻辑必须先于其他MonoBehavior的Update，所以需要在Script Execution Order
	/// 中将该脚本的执行顺序先于Default Time（或其他需要访问InputManager的脚本）
	/// </summary>
	public class InputManager : MonoSingleton<InputManager>
	{
		#region Attribute

		private bool m_stickLeft = GameDefine.DEFAULT_JOYSTICK_LEFT;
		/// <summary>
		/// 摇杆是否在屏幕左侧区域还是右侧区域
		/// </summary>
		/// <value><c>true</c> if stick left; otherwise, <c>false</c>.</value>
		public bool StickLeft
		{
			get
			{
				return m_stickLeft;
			}
			set
			{
				if (m_stickLeft != value)
				{
					m_stickLeft = value;
					if (onStickLeftChange != null)
						onStickLeftChange (m_stickLeft);
				}
			}
		}

		public event Action<bool> onStickLeftChange;

		/// <summary>
		/// 移动Touch
		/// </summary>
		/// <value>The stick touch.</value>
		public TouchHandler StickTouch
		{
			get{ return StickLeft ? leftTouch : rightTouch; }
		}

		/// <summary>
		/// 技能Touch
		/// </summary>
		/// <value>The skill touch.</value>
		public TouchHandler SkillTouch
		{
			get{ return StickLeft ? rightTouch : leftTouch; }
		}

		#endregion

		#region Private Member

		/// <summary>
		/// 控制屏幕左区域摇杆
		/// </summary>
		private TouchHandler leftTouch;

		/// <summary>
		/// 控制屏幕右区域摇杆
		/// </summary>
		private TouchHandler rightTouch;

		private float screenWidth;
		private float screenHeight;
		private float halfScreenWidth;
		private float halfScreenHeight;

		#endregion

		#region Protected Field

		protected override void OnAwake ()
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			halfScreenWidth = screenWidth * 0.5f;
			halfScreenHeight = screenHeight * 0.5f;

			InitTouchHandler ();
		}

		#endregion

		#region Private Field

		private void InitTouchHandler ()
		{
			leftTouch = new TouchHandler (ScreenRect.LEFT_SCREEN);
			rightTouch = new TouchHandler (ScreenRect.RIGHT_SCREEN);
		}

		void Update ()
		{
			leftTouch.Update ();
			rightTouch.Update ();
		}

		#endregion

	}
}

