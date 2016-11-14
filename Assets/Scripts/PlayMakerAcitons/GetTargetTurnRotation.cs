using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class GetTargetTurnRotation : FsmStateAction
	{

		[RequiredField]
		public FsmVector3 beforeForward;

		[RequiredField]
		public FsmVector3 beforeRight;

		[RequiredField]
		public FsmVector3 beforeUp;

		[RequiredField]
		public FsmVector3 afterForward;

		[RequiredField]
		public FsmVector3 afterRight;

		[RequiredField]
		public FsmVector3 afterUp;

		public FsmFloat turnAngleOffset;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmQuaternion targetRotation;

		public FsmEvent failEvent;

		public override void Reset ()
		{
			beforeForward = null;
			beforeRight = null;
			afterForward = null;
			afterRight = null;
			turnAngleOffset = null;
			targetRotation = new FsmQuaternion (){ UseVariable = true };
		}

		public override void OnEnter ()
		{
			// check turn left or right
			if (beforeUp.Value == afterUp.Value && afterForward.Value == -beforeRight.Value)
			{
				// left
				targetRotation.Value = Quaternion.LookRotation (afterForward.Value, afterUp.Value);
				if (!turnAngleOffset.IsNone)
					targetRotation.Value *= Quaternion.AngleAxis (-turnAngleOffset.Value, Vector3.up);
			}
			else if (beforeUp.Value == afterUp.Value && afterForward.Value == beforeRight.Value)
			{
				// right
				targetRotation.Value = Quaternion.LookRotation (afterForward.Value, afterUp.Value);
				if (!turnAngleOffset.IsNone)
					targetRotation.Value *= Quaternion.AngleAxis (turnAngleOffset.Value, Vector3.up);
			}
			else
			{
				Debug.LogError ("Turn failed!");
				Fsm.Event (failEvent);
			}
			Finish ();
		}
	}
}
