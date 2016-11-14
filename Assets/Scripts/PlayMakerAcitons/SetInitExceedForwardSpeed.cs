using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class SetInitExceedForwardSpeed : FsmStateAction
	{
		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmFloat forwardSpeed;

		public FsmFloat multiplier;

		public override void Reset ()
		{
			forwardSpeed = null;
			multiplier = null;
		}

		public override void OnEnter ()
		{
			if (!multiplier.IsNone)
				forwardSpeed.Value = forwardSpeed.Value * multiplier.Value;
			Finish ();
		}
	}
}
