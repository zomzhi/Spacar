using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class GetBridgeInfo : FsmStateAction
	{
		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmObject bridge;

		[UIHint (UIHint.Variable)]
		public FsmVector3 forward;

		[UIHint (UIHint.Variable)]
		public FsmVector3 right;

		[UIHint (UIHint.Variable)]
		public FsmVector3 up;

		LevelBridge levelBridge;

		public override void Reset ()
		{
			bridge = null;
			forward = new FsmVector3 (){ UseVariable = true };
			right = new FsmVector3 (){ UseVariable = true };
			up = new FsmVector3 (){ UseVariable = true };
		}

		public override void OnEnter ()
		{
			if (bridge.IsNone)
			{
				Finish ();
				return;
			}

//			levelBridge = bridge.Value as LevelBridge;
			Debug.Assert (levelBridge != null);

			if (!forward.IsNone)
				forward.Value = levelBridge.Forward;

			if (!right.IsNone)
				right.Value = levelBridge.Right;

			if (!up.IsNone)
				up.Value = levelBridge.Up;

			Finish ();
		}
	}
}
