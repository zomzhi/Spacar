using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class UpdateVerticalSpeed : FsmStateAction
	{
		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmFloat verticalSpeed;

		public FsmFloat verticalMaxSpeed;

		[UIHint (UIHint.Layer)]
		public int groundLayer;

		[RequiredField]
		public FsmVector3 gameObjectPosition;

		[RequiredField]
		public FsmVector3 upDirection;

		[RequiredField]
		public FsmFloat gravity;

		public FsmFloat rayCheckLength;

		// use GetGroundPoint action to get point added by touchgroundheight
		[RequiredField]
		public FsmVector3 groundPoint;

		[UIHint (UIHint.Variable)]
		public FsmBool grounded;

		[UIHint (UIHint.Variable)]
		public FsmBool fall;

		float curVerticalSpeed;
		float rayLength;
		Vector3 supposePosition;

		public override void Reset ()
		{
			verticalSpeed = new FsmFloat (){ UseVariable = true };
			groundLayer = 0;
			upDirection = null;
			gravity = null;

			grounded = new FsmBool (){ UseVariable = true };
			fall = new FsmBool (){ UseVariable = true };
		}

		public override void OnEnter ()
		{
			if (rayCheckLength.IsNone)
				rayLength = float.PositiveInfinity;
			else
				rayLength = rayCheckLength.Value;
		}

		public override void OnUpdate ()
		{
			verticalSpeed.Value -= gravity.Value * Time.deltaTime;
			if (!verticalMaxSpeed.IsNone && verticalSpeed.Value < -verticalMaxSpeed.Value)
				verticalSpeed.Value = -verticalMaxSpeed.Value;

			supposePosition = gameObjectPosition.Value + verticalSpeed.Value * Time.deltaTime * upDirection.Value;
			float supposeHeight = Vector3.Dot (supposePosition, upDirection.Value);
			float groundHeight = Vector3.Dot (groundPoint.Value, upDirection.Value);
			if (Mathf.Approximately (supposeHeight, groundHeight))
			{
				verticalSpeed.Value = 0f;
				if (!grounded.IsNone)
					grounded.Value = true;
				return;
			}

			if (supposeHeight < groundHeight)
			{
				// 下一帧的位移会使物体低于平面高度，检查物体下部是否存在地面，
				// 有的话设置速度为使物体这帧刚好到平面的值，没有则跌落出地面
				RaycastHit hit;
				if (Physics.Raycast (gameObjectPosition.Value, -upDirection.Value, out hit, rayLength, 1 << groundLayer))
				{
					verticalSpeed.Value = Vector3.Dot (groundPoint.Value - gameObjectPosition.Value, upDirection.Value) / Time.deltaTime;
					if (!grounded.IsNone)
						grounded.Value = true;
				}
				else
				{
					if (!fall.IsNone)
						fall.Value = true;
				}
			}
		}
	}
}

