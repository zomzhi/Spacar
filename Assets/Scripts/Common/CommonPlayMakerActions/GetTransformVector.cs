using UnityEngine;
using System.Collections;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory (ActionCategory.Transform)]
	public class GetTransformVector : FsmStateAction
	{
		public enum TransformVector
		{
			Forward,
			Right,
			Up
		}

		[RequiredField]
		public FsmOwnerDefault gameObject;

		public TransformVector direction;

		[RequiredField]
		public FsmVector3 result;

		public bool everyFrame;

		Transform trans;

		public override void Reset ()
		{
			gameObject = null;
			direction = TransformVector.Forward;
			result = new FsmVector3 (){ UseVariable = true };
			everyFrame = false;
		}

		public override void OnEnter ()
		{
			trans = Fsm.GetOwnerDefaultTarget (gameObject).transform;
			GetVector ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			if (everyFrame)
				GetVector ();
		}

		void GetVector ()
		{
			if (direction == TransformVector.Forward)
				result.Value = trans.forward;
			else if (direction == TransformVector.Right)
				result.Value = trans.right;
			else if (direction == TransformVector.Up)
				result.Value = trans.up;
		}
	}
}

