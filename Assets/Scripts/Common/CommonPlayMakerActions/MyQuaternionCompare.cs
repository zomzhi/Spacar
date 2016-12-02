using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory (ActionCategory.Logic)]
	public class MyQuaternionCompare : FsmStateAction
	{
		[RequiredField]
		public FsmQuaternion rotationA;

		[RequiredField]
		public FsmQuaternion rotationB;

		public FsmEvent equalEvent;

		public FsmEvent notEqualEvent;

		public bool everyFrame;

		public override void Reset ()
		{
			rotationA = null;
			rotationB = null;
			equalEvent = null;
			notEqualEvent = null;
			everyFrame = true;
		}

		public override void OnEnter ()
		{
			DoCompare ();
			if (!everyFrame)
				Finish ();
		}

		public override void OnUpdate ()
		{
			DoCompare ();
		}

		void DoCompare ()
		{
			if (rotationA == rotationB)
				Fsm.Event (equalEvent);
			else
				Fsm.Event (notEqualEvent);
		}
	}
}


