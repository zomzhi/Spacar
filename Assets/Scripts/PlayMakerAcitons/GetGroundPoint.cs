using UnityEngine;
using System.Collections;
using MyCompany.MyGame;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class GetGroundPoint : FsmStateAction
	{
		[RequiredField]
		public FsmVector3 origin;

		[RequiredField]
		public FsmVector3 upVector;

		public FsmFloat touchGroundHeight;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmVector3 groundPoint;

		public override void Reset ()
		{
			origin = null;
			upVector = null;
			touchGroundHeight = null;
			groundPoint = new FsmVector3 (){ UseVariable = true };
		}

		public override void OnEnter ()
		{
			groundPoint.Value = origin.Value + upVector.Value * GameDefine.BLOCK_TALL;
			if (!touchGroundHeight.IsNone)
				groundPoint.Value += upVector.Value * touchGroundHeight.Value;
			Finish ();
		}
	}
}


