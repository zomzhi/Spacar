using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class TurnExceedBridgeRotate : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmFloat spinSpeed;

		Transform trans;
		Quaternion startRot;
		float spinAngle;

		public override void Reset ()
		{
			gameObject = null;
			spinSpeed = null;
		}

		public override void OnEnter ()
		{
			trans = Fsm.GetOwnerDefaultTarget (gameObject).transform;
			startRot = trans.rotation;
			spinAngle = 0f;
		}

		public override void OnUpdate ()
		{
			trans.rotation *= Quaternion.AngleAxis (spinSpeed.Value * Time.deltaTime, Vector3.forward);
		}
	}
}
