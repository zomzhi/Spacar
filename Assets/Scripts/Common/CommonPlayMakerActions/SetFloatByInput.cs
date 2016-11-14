using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class SetFloatByInput : FsmStateAction
	{
		[RequiredField]
		public FsmFloat input;

		[RequiredField]
		public FsmFloat floatValue;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmFloat floatVariable;

		public bool everyFrame;

		public override void Reset ()
		{
			input = null;
			floatVariable = null;
			floatVariable = new FsmFloat (){ UseVariable = true };
			everyFrame = true;
		}

		public override void OnEnter ()
		{
			SetFloat ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			SetFloat ();
		}

		void SetFloat ()
		{
			if (Mathf.Approximately (input.Value, 0f))
				floatVariable.Value = 0f;
			else
				floatVariable.Value = input.Value > 0 ? floatValue.Value : -floatValue.Value;
		}
	}
}

