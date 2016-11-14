using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory (ActionCategory.Math)]
	public class GetMaxFloatValue : FsmStateAction
	{
		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmFloat resultValue;

		[RequiredField]
		public FsmFloat value1;

		[RequiredField]
		public FsmFloat value2;

		public bool everyFrame;

		public override void Reset ()
		{
			everyFrame = false;
		}

		public override void OnEnter ()
		{
			GetMaxValue ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			GetMaxValue ();
		}

		void GetMaxValue ()
		{
			resultValue.Value = Mathf.Max (value1.Value, value2.Value);
		}
	}
}

