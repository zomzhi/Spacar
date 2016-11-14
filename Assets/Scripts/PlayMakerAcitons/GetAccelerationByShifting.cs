using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class GetAccelerationByShifting : FsmStateAction
	{
		[RequiredField]
		public FsmFloat maxShifting;

		[RequiredField]
		public FsmFloat velocity;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmFloat acceleration;

		public override void Reset ()
		{
			maxShifting = null;
			velocity = null;
			acceleration = null;
		}

		public override void OnEnter ()
		{
			acceleration.Value = (velocity.Value * velocity.Value) / (2 * maxShifting.Value);
			Finish ();
		}
	}
}

