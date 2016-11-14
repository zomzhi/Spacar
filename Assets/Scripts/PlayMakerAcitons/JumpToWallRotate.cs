using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class JumpToWallRotate : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmFloat jumpSpeed;

		[RequiredField]
		public FsmFloat gravity;

		[RequiredField]
		public FsmVector3 forward;

		[RequiredField]
		public FsmVector3 up;

		[RequiredField]
		public FsmVector3 right;

		[RequiredField]
		public FsmFloat raiseAngleSpeed;

		public FsmFloat spinSpeed;

		Transform trans;
		float spinAngle;

		Vector3 lookDir;
		Vector3 upDir;

		public override void Reset ()
		{
			gameObject = null;
			jumpSpeed = null;
			gravity = null;
			forward = null;
			up = null;
			right = null;
			spinSpeed = null;
		}

		public override void OnEnter ()
		{
			trans = Fsm.GetOwnerDefaultTarget (gameObject).transform;

			spinAngle = 0f;

			lookDir = forward.Value;
		}

		public override void OnUpdate ()
		{
			// FIXME: 最好是设一个targetRotation，用QuaternionRotateTowards来设置旋转
			lookDir = Vector3.RotateTowards (lookDir, up.Value, raiseAngleSpeed.Value * Time.deltaTime, 0f);
			upDir = Vector3.Cross (lookDir, right.Value);
			trans.rotation = Quaternion.LookRotation (lookDir, upDir);

			if (!spinSpeed.IsNone)
				spinAngle -= spinSpeed.Value * Time.deltaTime;

			trans.rotation *= Quaternion.AngleAxis (spinAngle, Vector3.forward);
		}
	}
}


