using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class GetLookRotation : FsmStateAction
	{
		[RequiredField]
		public FsmVector3 lookAtDirection;

		public FsmVector3 upDirection;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmQuaternion rotation;

		public bool everyFrame;

		public override void Reset ()
		{
			lookAtDirection = null;
			upDirection = null;
			rotation = new FsmQuaternion (){ UseVariable = true };
			everyFrame = false;
		}

		public override void OnEnter ()
		{
			GetRotation ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			if (everyFrame)
				GetRotation ();
		}

		void GetRotation ()
		{
			if (upDirection.IsNone)
				rotation.Value = Quaternion.LookRotation (lookAtDirection.Value);
			else
				rotation.Value = Quaternion.LookRotation (lookAtDirection.Value, upDirection.Value);
		}
	}
}


