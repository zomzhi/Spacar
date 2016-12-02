using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class MyQuaternionRotateTowards : FsmStateAction
	{
		[RequiredField]
		public FsmQuaternion targetRotation;

		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmFloat angleSpeed;

		public bool perSecond;

		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmQuaternion resultRotation;

		public override void Reset ()
		{
			targetRotation = null;
			gameObject = null;
			angleSpeed = null;
			perSecond = true;
		}

		public override void OnEnter ()
		{
			
		}

		public override void OnUpdate ()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget (gameObject);
			if (perSecond)
				resultRotation.Value = Quaternion.RotateTowards (go.transform.rotation, targetRotation.Value, angleSpeed.Value * Time.deltaTime);
			else
				resultRotation.Value = Quaternion.RotateTowards (go.transform.rotation, targetRotation.Value, angleSpeed.Value);
		}
	}

}

