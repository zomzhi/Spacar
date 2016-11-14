using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class RotateAroundAxis : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmVector3 axis;

		[RequiredField]
		public FsmVector3 point;

		[RequiredField]
		public FsmFloat angle;

		public bool everyFrame;

		Transform trans;

		public override void Reset ()
		{
			gameObject = null;
			axis = null;
			point = null;
			angle = null;
			everyFrame = false;
		}

		public override void OnEnter ()
		{
			trans = gameObject.GameObject.Value.transform;
			DoRotate ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			DoRotate ();
		}

		void DoRotate ()
		{
			trans.RotateAround (point.Value, axis.Value, angle.Value);
		}
	}
}
