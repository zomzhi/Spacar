using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory (ActionCategory.Transform)]
	public class MoveTowardsAlways : FsmStateAction
	{
		[RequiredField]
		[Tooltip ("The GameObject to Move")]
		public FsmOwnerDefault gameObject;

		[Tooltip ("A target GameObject to move towards. Or use a world Target Position below.")]
		public FsmGameObject targetObject;

		[Tooltip ("A world position if no Target Object. Otherwise used as a local offset from the Target Object.")]
		public FsmVector3 targetPosition;

		[Tooltip ("The maximum movement speed. HINT: You can make this a variable to change it over time.")]
		public FsmFloat maxSpeed;

		[HasFloatSlider (0, 5)]
		[Tooltip ("Distance at which the move is considered finished, and the Finish Event is sent.")]
		public FsmFloat finishDistance;

		[Tooltip ("Event to send when the Finish Distance is reached.")]
		public FsmEvent finishEvent;

		Vector3 targetPos;
		GameObject go;
		GameObject goTarget;

		public override void Reset ()
		{
			gameObject = null;
			targetObject = null;
			targetPosition = null;
			maxSpeed = 10f;
			finishDistance = 0.1f;
			finishEvent = null;
		}

		public override void OnEnter ()
		{
			go = Fsm.GetOwnerDefaultTarget (gameObject);
		}

		public override void OnUpdate ()
		{
			goTarget = targetObject.Value;
			if (goTarget == null && targetPosition.IsNone)
				return;

			if (targetObject.Value != null)
				targetPos = targetObject.Value.transform.position;
			else
				targetPos = targetPosition.Value;

			go.transform.position = Vector3.MoveTowards (go.transform.position, targetPos, maxSpeed.Value * Time.deltaTime);

			if (finishEvent != null)
			{
				float distance = (go.transform.position - targetPos).magnitude;
				if (distance < finishDistance.Value)
				{
					Fsm.Event (finishEvent);
					Finish ();
				}
			}
		}
	}
}


