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

		public virtual void SetupBridgePath (ref BridgePath bridgePath)
		{
			
		}

		#endregion
	}
}

