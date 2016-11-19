using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using MyCompany.MyGame.Data.Character;
using MyCompany.MyGame.Level;
using MyCompany.Common.Signal;

namespace MyCompany.MyGame.Player
{
	public enum PlayerMotionState
	{
		// the name has to be the same as play maker state machine state name.
		StraightMove = 0,
		JumpToWall,
		ExceedUpBridge,
		TurnExceedBridge,

		Max
	}

	public class PlayerController : MonoBehaviour
	{
		#region Public Member

		public PlayerAttribute playerAttribute;

		[HideInInspector]
		public LevelBridge currentBridge;

		#endregion

		#region Attribute

		public Vector3 Position
		{
			get{ return thisTrans.position; }
			set{ thisTrans.position = value; }
		}

		/// <summary>
		/// 悬空高度
		/// </summary>
		/// <value>The height of the touch ground.</value>
		public float TouchGroundHeight
		{
			get{ return playerAttribute.touchGroundHeight; }
		}

		/// <summary>
		/// 当前运行的方向即Bridge的方向，注意区分transform.forward
		/// </summary>
		/// <value>The current forward.</value>
		public Vector3 LevelForward{ get { return levelForwardVar.Value; } }

		public Transform Trans{ get { return thisTrans; } }

		#endregion

		#region Private Member

		const string FORWARD_ACCE_FLOAT = "forwardAcceleration";
		const string FORWARD_MAX_SPEED_FLOAT = "forwardMaxSpeed";
		const string HORIZON_ACCE_FLOAT = "horizonAcceleration";
		const string HORIZON_MAX_SPEED_FLOAT = "horizonMaxSpeed";

		const string LEVEL_FORWARD_BEFORE = "levelForwardBefore";
		const string LEVEL_RIGHT_BEFORE = "levelRightBefore";
		const string LEVEL_UP_BEFORE = "levelUpBefore";

		const string PLAYER_ATTRIBUTE_OBJECT = "playerAttribute";

		const string CURVERTICAL_SPEED_FLOAT = "curVerticalSpeed";

		PlayMakerFSM motionPMFsm;
		Fsm motionFsm;

		// PlayMaker variables
		FsmVector3 levelForwardVar;
		FsmVector3 levelRightVar;
		FsmVector3 levelUpVar;
		FsmVector3 bridgeOriginVar;

		FsmFloat verticalSpeedVar;

		private Transform thisTrans;

		bool grounded;

		FsmState[] motionStates;

		/// <summary>
		/// 是否已经转向，在每个bridge内只能进行一次转向动作
		/// </summary>
		bool turnUsed = false;

		#endregion


		#region internal methods

		void Awake ()
		{
			motionPMFsm = GetComponent<PlayMakerFSM> ();
			motionFsm = motionPMFsm.Fsm;

			thisTrans = transform;

			SignalMgr.instance.Subscribe (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, (funsig<LevelBridge, LevelBridge>)OnSwitchBridge);
		}

		void Start ()
		{
			if (playerAttribute == null)
			{
				UnityLog.LogError ("Assign character attribute for the player before start play!", gameObject);
				return;
			}

			motionStates = new FsmState[(int)PlayerMotionState.Max];
			for (int i = 0; i < (int)PlayerMotionState.Max; i++)
			{
				motionStates [i] = motionFsm.GetState (((PlayerMotionState)i).ToString ());
				if (motionStates [i] == null)
				{
					UnityLog.LogError (((PlayerMotionState)i).ToString () + " is not exist, check enum state name.");
				}
			}

			// init variables
			verticalSpeedVar = motionFsm.GetFsmFloat (CURVERTICAL_SPEED_FLOAT);
			levelForwardVar = motionFsm.GetFsmVector3 ("levelForward");
			levelRightVar = motionFsm.GetFsmVector3 ("levelRight");
			levelUpVar = motionFsm.GetFsmVector3 ("levelUp");
			bridgeOriginVar = motionFsm.GetFsmVector3 ("bridgeOrigin");
		}


		void Update ()
		{

		}
			
		void OnDestroy ()
		{
			SignalMgr.instance.Unsubscribe (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, (funsig<LevelBridge, LevelBridge>)OnSwitchBridge);
		}


		#endregion


		#region Private Methods

		void GestureSwipe (SwipeGesture gesture)
		{
			Debug.Log ("SwipeGesture  swipe happened " + gesture.swipeDirection + " went from " + gesture.startPosition + " to " + gesture.endPosition);
		}

		void OnSwitchBridge (LevelBridge prevBridge, LevelBridge newBridge)
		{
			currentBridge = newBridge;
			turnUsed = false;
			InitLevelParam (currentBridge);
		}

		private void InitPlayAttribute ()
		{
			motionFsm.GetFsmObject (PLAYER_ATTRIBUTE_OBJECT).Value = playerAttribute;
			motionFsm.Event ("InitParam");
		}

		private void InitLevelParam (LevelBridge bridge)
		{
			currentBridge = bridge;
			levelForwardVar.Value = bridge.Forward;
			levelRightVar.Value = bridge.Right;
			levelUpVar.Value = bridge.Up;
			bridgeOriginVar.Value = bridge.leftBottom;
		}

		#endregion

		#region Public Methods



		public void PlaceOnBlock (LevelBlock block)
		{
			Vector3 bridgeCenter = block.leftBottom + block.bridgeBelong.Right * block.width * 0.5f +
			                       block.bridgeBelong.Forward * block.height * 0.5f +
			                       block.bridgeBelong.Up * GameDefine.BLOCK_TALL;
			bridgeCenter += block.bridgeBelong.Up * playerAttribute.touchGroundHeight;
			transform.position = bridgeCenter;
			transform.rotation = Quaternion.LookRotation (block.bridgeBelong.Forward, block.bridgeBelong.Up);
		}

		public void StartRun (LevelBridge firstBridge)
		{
			InitPlayAttribute ();
			InitLevelParam (firstBridge);

			motionFsm.Event ("GroundMove");
		}



		// obselete
		public void StartTurn (LevelBridge curBridge, LevelBridge newBridge)
		{
			motionFsm.GetFsmVector3 (LEVEL_FORWARD_BEFORE).Value = curBridge.Forward;
			motionFsm.GetFsmVector3 (LEVEL_RIGHT_BEFORE).Value = curBridge.Right;
			motionFsm.GetFsmVector3 (LEVEL_UP_BEFORE).Value = curBridge.Up;
			bridgeOriginVar.Value = curBridge.leftBottom;

			levelForwardVar.Value = newBridge.Forward;
			levelRightVar.Value = newBridge.Right;
			levelUpVar.Value = newBridge.Up;

			motionFsm.Event ("Turn");
		}

		public void JumpToWall ()
		{
			motionFsm.Event ("JumpToWall");
		}

		public void ToStraightMove (LevelBridge curBridge, LevelBridge newBridge)
		{
			motionFsm.GetFsmVector3 (LEVEL_FORWARD_BEFORE).Value = curBridge.Forward;
			motionFsm.GetFsmVector3 (LEVEL_RIGHT_BEFORE).Value = curBridge.Right;
			motionFsm.GetFsmVector3 (LEVEL_UP_BEFORE).Value = curBridge.Up;

			levelForwardVar.Value = newBridge.Forward;
			levelRightVar.Value = newBridge.Right;
			levelUpVar.Value = newBridge.Up;
			bridgeOriginVar.Value = newBridge.leftBottom;

			motionFsm.Event ("GroundMove");
		}

		public void ExceedToNextBridge (LevelBridge curBridge, LevelBridge newBridge)
		{
			motionFsm.GetFsmVector3 (LEVEL_FORWARD_BEFORE).Value = curBridge.Forward;
			motionFsm.GetFsmVector3 (LEVEL_RIGHT_BEFORE).Value = curBridge.Right;
			motionFsm.GetFsmVector3 (LEVEL_UP_BEFORE).Value = curBridge.Up;

			levelForwardVar.Value = newBridge.Forward;
			levelRightVar.Value = newBridge.Right;
			levelUpVar.Value = newBridge.Up;
			bridgeOriginVar.Value = newBridge.leftBottom;

			motionFsm.Event ("ExceedBridge");
		}

		public void TurnExceedToNextBridge (LevelBridge curBridge, LevelBridge newBridge)
		{
			motionFsm.GetFsmVector3 (LEVEL_FORWARD_BEFORE).Value = curBridge.Forward;
			motionFsm.GetFsmVector3 (LEVEL_RIGHT_BEFORE).Value = curBridge.Right;
			motionFsm.GetFsmVector3 (LEVEL_UP_BEFORE).Value = curBridge.Up;

			levelForwardVar.Value = newBridge.Forward;
			levelRightVar.Value = newBridge.Right;
			levelUpVar.Value = newBridge.Up;
			bridgeOriginVar.Value = newBridge.leftBottom;
			motionFsm.Event ("TurnExceedBridge");
		}

		public bool IsInJumpToWallState ()
		{
			return motionFsm.ActiveState == motionStates [(int)PlayerMotionState.JumpToWall];
		}

		public bool CanGoToExceedBridge ()
		{
			return motionFsm.ActiveState == motionStates [(int)PlayerMotionState.StraightMove];
		}

		public bool IsOnGround ()
		{
			return motionFsm.ActiveState == motionStates [(int)PlayerMotionState.StraightMove];
		}

		/// <summary>
		/// 玩家是否没有处于上升的状态,即直线运动和跳起过程的下落阶段
		/// </summary>
		/// <returns><c>true</c> if this instance is not jump up; otherwise, <c>false</c>.</returns>
		public bool IsNotJumpUp ()
		{
			if (motionFsm.ActiveState == motionStates [(int)PlayerMotionState.StraightMove])
				return true;
			else if (motionFsm.ActiveState == motionStates [(int)PlayerMotionState.JumpToWall] ||
			         motionFsm.ActiveState == motionStates [(int)PlayerMotionState.ExceedUpBridge] ||
			         motionFsm.ActiveState == motionStates [(int)PlayerMotionState.TurnExceedBridge])
			{
				return verticalSpeedVar.Value < 0;
			}
			return true;
		}

		public void SwipeLeft ()
		{
			if (!turnUsed)
			{
				turnUsed = true;
				motionFsm.GetFsmVector3 (LEVEL_FORWARD_BEFORE).Value = currentBridge.Forward;
				motionFsm.GetFsmVector3 (LEVEL_RIGHT_BEFORE).Value = currentBridge.Right;
				motionFsm.GetFsmVector3 (LEVEL_UP_BEFORE).Value = currentBridge.Up;
				bridgeOriginVar.Value = currentBridge.leftBottom;

				levelForwardVar.Value = -currentBridge.Right;
				levelRightVar.Value = currentBridge.Forward;
				levelUpVar.Value = currentBridge.Up;

				motionFsm.Event ("Turn");
			}
		}

		public void SwipRight ()
		{
			if (!turnUsed)
			{
				turnUsed = true;
				motionFsm.GetFsmVector3 (LEVEL_FORWARD_BEFORE).Value = currentBridge.Forward;
				motionFsm.GetFsmVector3 (LEVEL_RIGHT_BEFORE).Value = currentBridge.Right;
				motionFsm.GetFsmVector3 (LEVEL_UP_BEFORE).Value = currentBridge.Up;
				bridgeOriginVar.Value = currentBridge.leftBottom;

				levelForwardVar.Value = currentBridge.Right;
				levelRightVar.Value = -currentBridge.Forward;
				levelUpVar.Value = currentBridge.Up;

				motionFsm.Event ("Turn");
			}
		}

		#endregion
	}
}

