using UnityEngine;
using System.Collections;
using MyCompany.Common.Input;
using MyCompany.MyGame;

namespace HutongGames.PlayMaker.Actions
{
	public enum ETouchType
	{
		STICK_TOUCH,
		SKILL_TOUCH
	}

	[ActionCategory ("Touch")]
	public class GetTouchParam : FsmStateAction
	{
		public ETouchType touchType;

		[UIHint (UIHint.Variable)]
		public FsmFloat horizonMove;

		public bool everyFrame;

		TouchHandler touch;

		public override void Reset ()
		{
			touchType = ETouchType.STICK_TOUCH;
			horizonMove = null;
			everyFrame = true;
		}

		public override void OnEnter ()
		{
			if (touchType == ETouchType.STICK_TOUCH)
				touch = InputManager.Instance.StickTouch;
			else if (touchType == ETouchType.SKILL_TOUCH)
				touch = InputManager.Instance.SkillTouch;

			horizonMove.Value = GetHorizonMove (touch);
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			if (touchType == ETouchType.STICK_TOUCH)
				touch = InputManager.Instance.StickTouch;
			else if (touchType == ETouchType.SKILL_TOUCH)
				touch = InputManager.Instance.SkillTouch;

			horizonMove.Value = GetHorizonMove (touch);
		}

		float GetHorizonMove (TouchHandler touch)
		{
			float result = 0f;
			if (touch.Moved)
				result = Mathf.Clamp ((touch.Position.x - touch.startPos.x) / GameDefine.STICK_RANGE, -1f, 1f);
			return result;
		}
	}
}

