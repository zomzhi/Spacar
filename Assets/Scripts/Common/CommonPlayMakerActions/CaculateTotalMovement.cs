using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class CaculateTotalMovement : FsmStateAction
	{
		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmVector3 totalMovement;

		[CompoundArray ("Movement", "Speed", "Move Direction")]
		public FsmFloat[] speed;
		public FsmVector3[] moveDir;

		public bool everyFrame;

		Vector3 result;

		public override void Reset ()
		{
			totalMovement = new FsmVector3 (){ UseVariable = true };
			speed = new FsmFloat[1];
			moveDir = new FsmVector3[1];
			everyFrame = true;
		}

		public override void OnEnter ()
		{
			DoCaculateTotalMovement ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			DoCaculateTotalMovement ();
		}

		void DoCaculateTotalMovement ()
		{
			if (totalMovement.IsNone)
				return;

			result = Vector3.zero;
			for (int i = 0; i < speed.Length; i++)
			{
				result += speed [i].Value * moveDir [i].Value;
			}
			totalMovement.Value = result;
		}
	}
}

