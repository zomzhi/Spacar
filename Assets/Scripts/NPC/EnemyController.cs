using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using MyCompany.MyGame.Data.Character;
using HutongGames.PlayMaker;
using MyCompany.MyGame.PathFinding;

namespace MyCompany.MyGame.NPC
{
	public class EnemyController : NPCController
	{
		public enum EnemyBaseState
		{
			Idle = 0,
			Move,
		}


		#region Attribute

		protected float touchGroundHeight;
		public float TouchGroundHeight{ get { return touchGroundHeight; } }

		#endregion


		public LevelBridge currentBridge;

		protected FsmState idleState;
		protected FsmState moveState;

		protected override void Awake ()
		{
			base.Awake ();
			idleState = motionFsm.GetState (EnemyBaseState.Idle.ToString ());
			moveState = motionFsm.GetState (EnemyBaseState.Move.ToString ());

			touchGroundHeight = ((EnemyAttribute)characterAttribute).touchGroundHeight;

			motionFsm.GetFsmFloat ("touchGroundHeight").Value = touchGroundHeight;
		}

		#region Protected Methods


		#endregion

		#region Util Methods

		public void Initialize (LevelBridge bridge, Vector3 position)
		{
			currentBridge = bridge;
			thisTrans.position = position;
			PlaceOnBridge ();
		}

		public void PlaceOnBridge ()
		{
			Position = currentBridge.ProjectToPlanePoint (Position, TouchGroundHeight);
		}

		public virtual bool CanProceedToNextDestination ()
		{
			if (motionFsm.ActiveState == idleState)
				return true;
			return false;
		}

		public void SetupBridgePath (ref BridgePath bridgePath)
		{
			if (bridgePath.Type == BridgePath.PathType.ToNextBridge)
			{
				if (bridgePath.bridge.Up == bridgePath.nextBridge.Up)
				{
					SetupTurnBridgePath (ref bridgePath);
				}
				else if (bridgePath.bridge.Forward == bridgePath.nextBridge.Up)
				{
					SetupExceedBridgePath (ref bridgePath);
				}
				else if (bridgePath.bridge.Up == bridgePath.nextBridge.Forward)
				{
					SetupJumpUpBridgePath (ref bridgePath);
				}
				else if (bridgePath.nextBridge.Up == bridgePath.bridge.Right ||
				         bridgePath.nextBridge.Up == -bridgePath.bridge.Right)
				{
					SetupTurnExceedBridgePath (ref bridgePath);
				}
				else
					UnityLog.LogError ("Bridge path type not implemented yet!");
			}
			else if (bridgePath.Type == BridgePath.PathType.ToPosition)
			{
				SetupToPosBridgePath (ref bridgePath);
			}
		}

		protected virtual void SetupToPosBridgePath (ref BridgePath bridgePath)
		{
		}

		protected virtual void SetupTurnBridgePath (ref BridgePath bridgePath)
		{
		}

		protected virtual void SetupExceedBridgePath (ref BridgePath bridgePath)
		{
		}

		protected virtual void SetupJumpUpBridgePath (ref BridgePath bridgePath)
		{
		}

		protected virtual void SetupTurnExceedBridgePath (ref BridgePath bridgePath)
		{
		}

		#endregion
	}
}

