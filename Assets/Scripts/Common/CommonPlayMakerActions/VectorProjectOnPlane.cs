using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory (ActionCategory.Vector3)]
	public class VectorProjectOnPlane : FsmStateAction
	{
		[RequiredField]
		public FsmVector3 vector;

		[RequiredField]
		public FsmVector3 planeNormal;

		[RequiredField]
		public FsmVector3 storeResult;

		public bool everyFrame;

		public override void Reset ()
		{
			vector = null;
			planeNormal = null;
			storeResult = new FsmVector3 (){ UseVariable = true };
			everyFrame = false;
		}

		public override void OnEnter ()
		{
			ProjectOnPlane ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			if (everyFrame)
				ProjectOnPlane ();
		}

		void ProjectOnPlane ()
		{
			storeResult.Value = Vector3.ProjectOnPlane (vector.Value, planeNormal.Value);
		}
	}
}
