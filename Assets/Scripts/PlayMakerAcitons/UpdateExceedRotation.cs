using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class UpdateExceedRotation : FsmStateAction
	{
		[RequiredField]
		public FsmFloat verticalSpeed;

		[RequiredField]
		public FsmFloat gravity;

		[RequiredField]
		public FsmFloat lowHeadAngleSpeed;

		[RequiredField]
		public FsmFloat maxHeightHeadAngle;

		[RequiredField]
		public FsmFloat minFallHeadAngle;

		[RequiredField]
		public FsmFloat rollOneCircleTime;

		[RequiredField]
		public FsmVector3 upDir;

		[RequiredField]
		public FsmVector3 forwardDir;

		public FsmVector3 rightDir;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmQuaternion targetRotation;

		Vector3 lookDir;
		Vector3 lookUpDir;
		float upRotateAngle;
		float initVerticalSpeed;

		Quaternion lookRot;

		float hangTime;
		float spinSpeed;
		float totalSpinAngle;
		float curSpinAngle;
		float timer;

		public override void Reset ()
		{
			verticalSpeed = null;
			lowHeadAngleSpeed = null;
			maxHeightHeadAngle = null;
			minFallHeadAngle = null;
			rollOneCircleTime = null;
			upDir = null;
			forwardDir = null;
			targetRotation = new FsmQuaternion (){ UseVariable = true };
		}
			
		public override void OnEnter ()
		{
			upRotateAngle = 0f;
			initVerticalSpeed = verticalSpeed.Value;

			hangTime = 2 * Mathf.Abs (verticalSpeed.Value / gravity.Value);
			int spinCircles = Mathf.FloorToInt (hangTime / rollOneCircleTime.Value);
//			spinSpeed = 360f * spinCircles / hangTime;
			totalSpinAngle = spinCircles * 360f;
			timer = Time.time;
		}

		public override void OnUpdate ()
		{
			if (verticalSpeed.Value >= 0)
			{
				upRotateAngle = Mathf.LerpAngle (0f, maxHeightHeadAngle.Value, Mathf.Clamp01 ((initVerticalSpeed - verticalSpeed.Value) / initVerticalSpeed));
			}
			else
			{
				upRotateAngle += lowHeadAngleSpeed.Value * Time.deltaTime;
				if (upRotateAngle > minFallHeadAngle.Value)
					upRotateAngle = minFallHeadAngle.Value;
			}
			lookDir = Quaternion.AngleAxis (upRotateAngle, rightDir.Value) * upDir.Value;
			lookUpDir = Vector3.Cross (lookDir, rightDir.Value).normalized;

			lookRot = Quaternion.LookRotation (lookDir, lookUpDir);

			curSpinAngle = Mathf.Lerp (0f, totalSpinAngle, Mathf.Clamp01 ((Time.time - timer) / hangTime));

			lookRot *= Quaternion.AngleAxis (curSpinAngle, Vector3.forward);

			targetRotation.Value = lookRot;
		}
	}
}

