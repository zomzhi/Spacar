using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Data.Character;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class GetAttributeVariables : FsmStateAction
	{
		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmObject attribute;

		[UIHint (UIHint.Variable)]
		public FsmFloat horizonSpeedMax;

		[UIHint (UIHint.Variable)]
		public FsmFloat horizonAcceleration;

		[UIHint (UIHint.Variable)]
		public FsmFloat forwardSpeedMax;

		[UIHint (UIHint.Variable)]
		public FsmFloat forwardAcceleration;

		[UIHint (UIHint.Variable)]
		public FsmFloat turnAngleSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat forwardTurnAcce;

		[UIHint (UIHint.Variable)]
		public FsmFloat turnDeceleration;

		[UIHint (UIHint.Variable)]
		public FsmFloat turnDecelerationTime;

		[UIHint (UIHint.Variable)]
		public FsmFloat turnOffsetAngle;

		[UIHint (UIHint.Variable)]
		public FsmFloat adjustTurnSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat jumpSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat fallMaxSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat touchGroundHeight;

		[UIHint (UIHint.Variable)]
		public FsmFloat gravity;

		[UIHint (UIHint.Variable)]
		public FsmFloat raiseAngleSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat spinAngleSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat toNormalAngleSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat jumpWallMinSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat exceedMaxHeight;

		[UIHint (UIHint.Variable)]
		public FsmFloat lowHeadAngleSpeed;

		[UIHint (UIHint.Variable)]
		public FsmFloat maxHeightHeadAngle;

		[UIHint (UIHint.Variable)]
		public FsmFloat minFallHeadAngle;

		[UIHint (UIHint.Variable)]
		public FsmFloat exceedForwardSpeedMultiplier;

		[UIHint (UIHint.Variable)]
		public FsmFloat rollOneCircleTime;

		CharacterAttribute playerAttribute;

		public override void Reset ()
		{
			attribute = null;
			horizonSpeedMax = new FsmFloat (){ UseVariable = true };
			horizonAcceleration = new FsmFloat (){ UseVariable = true };
			forwardSpeedMax = new FsmFloat (){ UseVariable = true };
			forwardAcceleration = new FsmFloat (){ UseVariable = true };
			turnAngleSpeed = new FsmFloat (){ UseVariable = true };
			forwardTurnAcce = new FsmFloat (){ UseVariable = true };
			turnDeceleration = new FsmFloat (){ UseVariable = true };
			turnDecelerationTime = new FsmFloat (){ UseVariable = true };
			turnOffsetAngle = new FsmFloat (){ UseVariable = true };
			adjustTurnSpeed = new FsmFloat (){ UseVariable = true };
			jumpSpeed = new FsmFloat (){ UseVariable = true };
			gravity = new FsmFloat (){ UseVariable = true };
			fallMaxSpeed = new FsmFloat (){ UseVariable = true };
			touchGroundHeight = new FsmFloat (){ UseVariable = true };
			spinAngleSpeed = new FsmFloat (){ UseVariable = true };
			raiseAngleSpeed = new FsmFloat (){ UseVariable = true }; 
			toNormalAngleSpeed = new FsmFloat (){ UseVariable = true }; 
			jumpWallMinSpeed = new FsmFloat (){ UseVariable = true }; 
			exceedMaxHeight = new FsmFloat (){ UseVariable = true }; 
			lowHeadAngleSpeed = new FsmFloat (){ UseVariable = true }; 
			maxHeightHeadAngle = new FsmFloat (){ UseVariable = true }; 
			minFallHeadAngle = new FsmFloat (){ UseVariable = true }; 
			exceedForwardSpeedMultiplier = new  FsmFloat (){ UseVariable = true }; 
			rollOneCircleTime = new  FsmFloat (){ UseVariable = true }; 
		}

		public override void OnEnter ()
		{
			if (attribute.IsNone)
			{
				Finish ();
				return;
			}

			playerAttribute = attribute.Value as CharacterAttribute;
			Debug.Assert (playerAttribute != null);

			if (!horizonSpeedMax.IsNone)
				horizonSpeedMax.Value = playerAttribute.horizonSpeedMax;

			if (!horizonAcceleration.IsNone)
				horizonAcceleration.Value = playerAttribute.horizonAcceleration;

			if (!forwardSpeedMax.IsNone)
				forwardSpeedMax.Value = playerAttribute.forwardSpeedMax;

			if (!forwardAcceleration.IsNone)
				forwardAcceleration.Value = playerAttribute.forwardAcceleration;

			if (!turnAngleSpeed.IsNone)
				turnAngleSpeed.Value = playerAttribute.turnAngleSpeed;

			if (!forwardTurnAcce.IsNone)
				forwardTurnAcce.Value = playerAttribute.forwardTurnAcce;

			if (!turnDeceleration.IsNone)
				turnDeceleration.Value = playerAttribute.turnDeceleration;

			if (!turnDecelerationTime.IsNone)
				turnDecelerationTime.Value = playerAttribute.turnDecelerationTime;

			if (!turnOffsetAngle.IsNone)
				turnOffsetAngle.Value = playerAttribute.turnOffsetAngle;

			if (!adjustTurnSpeed.IsNone)
				adjustTurnSpeed.Value = playerAttribute.adjustTurnSpeed;

			if (!jumpSpeed.IsNone)
				jumpSpeed.Value = playerAttribute.jumpSpeed;

			if (!gravity.IsNone)
				gravity.Value = playerAttribute.gravity;

			if (!fallMaxSpeed.IsNone)
				fallMaxSpeed.Value = playerAttribute.fallMaxSpeed;

			if (!touchGroundHeight.IsNone)
				touchGroundHeight.Value = playerAttribute.touchGroundHeight;

			if (!spinAngleSpeed.IsNone)
				spinAngleSpeed.Value = playerAttribute.spinAngleSpeed;

			if (!raiseAngleSpeed.IsNone)
				raiseAngleSpeed.Value = playerAttribute.raiseAngleSpeed;

			if (!toNormalAngleSpeed.IsNone)
				toNormalAngleSpeed.Value = playerAttribute.toNormalAngleSpeed;

			if (!jumpWallMinSpeed.IsNone)
				jumpWallMinSpeed.Value = playerAttribute.jumpWallMinSpeed;

			if (!exceedMaxHeight.IsNone)
				exceedMaxHeight.Value = playerAttribute.exceedMaxHeight;

			if (!lowHeadAngleSpeed.IsNone)
				lowHeadAngleSpeed.Value = playerAttribute.lowHeadAngleSpeed;

			if (!maxHeightHeadAngle.IsNone)
				maxHeightHeadAngle.Value = playerAttribute.maxHeightHeadAngle;

			if (!minFallHeadAngle.IsNone)
				minFallHeadAngle.Value = playerAttribute.minFallHeadAngle;

			if (!exceedForwardSpeedMultiplier.IsNone)
				exceedForwardSpeedMultiplier.Value = playerAttribute.exceedForwardSpeedMultiplier;

			if (!rollOneCircleTime.IsNone)
				rollOneCircleTime.Value = playerAttribute.rollOneCircleTime;

			Finish ();
		}
	}
}

