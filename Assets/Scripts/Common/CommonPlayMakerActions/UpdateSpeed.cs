using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class UpdateSpeed : FsmStateAction
	{
		[RequiredField]
		public FsmFloat targetSpeed;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmFloat currentSpeed;

		[RequiredField]
		public FsmFloat acceleration;

		public bool everyFrame;

		public override void Reset ()
		{
			targetSpeed = null;
			currentSpeed = new FsmFloat (){ UseVariable = true };
			acceleration = null;
			everyFrame = true;
		}

		public override void OnEnter ()
		{
			UpdateSpeedFunc ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			UpdateSpeedFunc ();
		}

		void UpdateSpeedFunc ()
		{
			if (!Mathf.Approximately (currentSpeed.Value, targetSpeed.Value))
			{
				if (currentSpeed.Value < targetSpeed.Value)
				{
					currentSpeed.Value += acceleration.Value * Time.deltaTime;
					if (currentSpeed.Value > targetSpeed.Value)
						currentSpeed.Value = targetSpeed.Value;
				}
				else if (currentSpeed.Value > targetSpeed.Value)
				{
					currentSpeed.Value -= acceleration.Value * Time.deltaTime;
					if (currentSpeed.Value < targetSpeed.Value)
						currentSpeed.Value = targetSpeed.Value;
				}
			}
		}
	}
}

