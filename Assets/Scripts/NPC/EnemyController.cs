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

		public Transform target;
		public LevelBridge targetBridge;

		protected FsmState idleState;
		protected FsmState moveState;

		protected PathManager pathManager;

		protected sealed override void Awake ()
		{
			base.Awake ();
			idleState = motionFsm.GetState (EnemyBaseState.Idle.ToString ());
			moveState = motionFsm.GetState (EnemyBaseState.Move.ToString ());

			touchGroundHeight = ((EnemyAttribute)characterAttribute).touchGroundHeight;

			motionFsm.GetFsmFloat ("touchGroundHeight").Value = touchGroundHeight;

			pathManager = new PathManager (this);

			InitControllerAttribute ();
			OnAwake ();
		}

		void Update ()
		{
			pathManager.Update ();

			OnUpdate ();
		}

		#region Protected Methods

		protected void MoveEvent ()
		{
			motionFsm.Event ("Move");
		}

		protected void StopMoveEvent ()
		{
			motionFsm.Event ("StopMove");
		}

		protected void SetLevelParams (LevelBridge bridge)
		{
			SetLevelParams (bridge.Forward, bridge.Up, bridge.Right);
		}

		protected void SetLevelParams (Vector3 levelForward, Vector3 levelUp, Vector3 levelRight)
		{
			motionFsm.GetFsmVector3 ("levelForward").Value = levelForward;
			motionFsm.GetFsmVector3 ("levelUp").Value = levelUp;
			motionFsm.GetFsmVector3 ("levelRight").Value = levelRight;
		}

		protected void SetlevelParamsBefore (LevelBridge bridge)
		{
			SetLevelParamsBefore (bridge.Forward, bridge.Up, bridge.Right);
		}

		protected void SetLevelParamsBefore (Vector3 levelForwardBefore, Vector3 levelUpBefore, Vector3 levelRightBefore)
		{
			motionFsm.GetFsmVector3 ("levelForwardBefore").Value = levelForwardBefore;
			motionFsm.GetFsmVector3 ("levelUpBefore").Value = levelUpBefore;
			motionFsm.GetFsmVector3 ("levelRightBefore").Value = levelRightBefore;
		}

		#endregion

		#region Util Methods

		public void Initialize (LevelBridge bridge, Vector3 position)
		{
			currentBridge = bridge;
			thisTrans.position = position;
			PlaceOnBridge ();
			SetLevelParams (currentBridge);

			// FIXME: Remove this test code
			ResetTargetTrans (currentBridge);
		}

		public void PlaceOnBridge ()
		{
			Position = currentBridge.ProjectToPlanePoint (Position, TouchGroundHeight);
		}

		public void ResetTargetTrans (LevelBridge bridge)
		{
			targetBridge = bridge;
			target.position = targetBridge.ProjectToPlanePoint (targetBridge.leftBottom, GameDefine.BLOCK_TALL);
		}

		public virtual bool CanProceedToNextDestination ()
		{
			if (motionFsm.ActiveState == idleState)
				return true;
			return false;
		}

		public void MovetoPosition (LevelBridge destBridge, Vector3 position)
		{
			pathManager.SetDestination (destBridge, position);
		}

		public void SetupBridgePath (BridgePath bridgePath)
		{
			if (bridgePath.Type == BridgePath.PathType.ToNextBridge)
			{
				if (bridgePath.bridge.Up == bridgePath.nextBridge.Up)
				{
					SetupTurnBridgePath (bridgePath);
				}
				else if (bridgePath.bridge.Forward == bridgePath.nextBridge.Up)
				{
					SetupExceedBridgePath (bridgePath);
				}
				else if (bridgePath.bridge.Up == bridgePath.nextBridge.Forward)
				{
					SetupJumpUpBridgePath (bridgePath);
				}
				else if (bridgePath.nextBridge.Up == bridgePath.bridge.Right ||
				         bridgePath.nextBridge.Up == -bridgePath.bridge.Right)
				{
					SetupTurnExceedBridgePath (bridgePath);
				}
				else
					UnityLog.LogError ("Bridge path type not implemented yet!");
			}
			else if (bridgePath.Type == BridgePath.PathType.ToPosition)
			{
				SetupToPosBridgePath (bridgePath);
			}
		}

		protected virtual void SetupToPosBridgePath (BridgePath bridgePath)
		{
			BridgePath.PathSegment segment = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, OnPathReachDestination, 0.1f);
			PathRequestManager.RequestPath (bridgePath.bridge, Position, bridgePath.destination, ((Vector3[] path, bool success) => {
				if (success)
				{
					segment.SetPath (path);
				}
				else
				{
					Debug.Break ();
					UnityLog.LogError ("Path Request failed! " + Position + " -> " + bridgePath.destination);
				}
			}), touchGroundHeight);
		}

		protected virtual void SetupTurnBridgePath (BridgePath bridgePath)
		{
		}

		protected virtual void SetupExceedBridgePath (BridgePath bridgePath)
		{
		}

		protected virtual void SetupJumpUpBridgePath (BridgePath bridgePath)
		{
		}

		protected virtual void SetupTurnExceedBridgePath (BridgePath bridgePath)
		{
		}

		protected virtual void OnPathStart (Vector3 firstPoint)
		{
			UnityLog.Log ("On path start.");
		}

		protected virtual void OnPathReachMiddlePoint (Vector3 nextPoint)
		{
			UnityLog.Log ("On path reach middle point");
		}

		protected virtual void OnPathReachDestination ()
		{
			UnityLog.Log ("On path reach destination");
		}

		protected virtual void InitControllerAttribute ()
		{
			
		}

		protected virtual void OnAwake ()
		{
		}

		protected virtual void OnUpdate ()
		{
		}

		#region Private Methods

		void OnDrawGizmosSelected ()
		{
			if (pathManager != null && !pathManager.Finished)
			{
				pathManager.OnDrawGizmos ();
			}
		}

		#endregion

		#endregion
	}
}

