using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using MyCompany.MyGame.Player;
using MyCompany.MyGame.CameraControl;
using Newtonsoft.Json;
using MyCompany.Common.Signal;
using MyCompany.MyGame.NPC;

namespace MyCompany.MyGame
{
	public class GamePlay : MonoBehaviour
	{
		#region Public Member

		public TextAsset firstLevelAsset;
		public AudioClip mainTheme;
		public CameraController camController;
		public Transform playerStartPoint;

		public GameObject firstBridgeGo;
		public ELevelType firstBridgeType;
		public GameDefine.BLOCK_SPECIFICATION firstBridgeWidth;
		public int firstBridgeBlockCount;

		[HideInInspector]
		public bool startPlay;

		#endregion

		#region Private Member

		private LevelGenerator levelGenerator;

		private LevelBridge dummyBridge;
		private PlayerController playerController;

		//		private DichotomyList<LevelBridge> path;
		private LevelBridge curBridge;
		//		private LevelBridge nextBridge;

		int groundLayerMask;
		bool initialized = false;

		#endregion


		void Awake ()
		{
			levelGenerator = GetComponentInChildren<LevelGenerator> ();
			UnityLog.Assert ((levelGenerator != null), "Missing level generator component");

			UnityLog.Assert ((playerStartPoint != null), "Missing player start transform");
			dummyBridge = new LevelBridge (firstBridgeType, firstBridgeBlockCount, (int)firstBridgeWidth, true);
			dummyBridge.BridgeGo = firstBridgeGo;
		}

		void Start ()
		{
			startPlay = false;
			groundLayerMask = 1 << LayerMask.NameToLayer (GameDefine.GROUND_LAYER_NAME);

			// Place player
			GameObject playerPrefab = Resources.Load<GameObject> ("Player/Player");
			playerController = GameObject.Instantiate (playerPrefab).GetComponent<PlayerController> ();
			playerController.transform.position = playerStartPoint.position;
			playerController.transform.rotation = playerStartPoint.rotation;
		}

		void Update ()
		{
			if (!GameSystem.Instance.Paused)
				UpdateInput ();
		}

		void LateUpdate ()
		{
			if (GameSystem.Instance.Paused || !startPlay)
				return;

			curBridge = playerController.currentBridge;

			// TODO: 看是用脚底的position还是pivot的position
			if (curBridge.BelowBridge (playerController.Position))
			{
				// 比当前bridge平面低，失败
				return;
			}

			// 更新摄像机参数
			UpdateCamera ();

			// 检测是否跨越了新的bridge
			if (playerController.IsNotJumpUp ())
			{
				RaycastHit groundHit;
				if (Physics.Raycast (playerController.Position, -curBridge.Up, out groundHit, playerController.TouchGroundHeight + 5, groundLayerMask))
				{
					LevelBlock hitBlock = groundHit.transform.GetComponent<LevelBlock> ();
					if (hitBlock != null && hitBlock.bridgeBelong != null && hitBlock.bridgeBelong != curBridge && hitBlock.bridgeBelong != curBridge.prev)
					{
						SignalMgr.instance.Raise (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, curBridge, hitBlock.bridgeBelong);
						UnityLog.Log ("Enter new bridge -> down check");
					}
				}
			}

			// 检测跳起状态何时碰触下一个bridge
			if (playerController.IsInJumpToWallState ())
			{
				LevelBridge bridge1 = curBridge.next0;
				LevelBridge bridge2 = curBridge.next1;
				RaycastHit hit;
				if (Physics.Raycast (playerController.Position, curBridge.Forward, out hit, 10, groundLayerMask))
				{
					LevelBlock hitBlock = hit.transform.GetComponent<LevelBlock> ();
					UnityLog.Assert (hitBlock != null, "ground missing LevelBlock component. wtf", hit.transform.gameObject);
					if (bridge1 != null && hitBlock.bridgeBelong == bridge1 && bridge1.BelowBridge (playerController.Position, playerController.TouchGroundHeight))
					{
						// Jump to bridge1
						playerController.ToStraightMove (curBridge, bridge1);
						playerController.Position = bridge1.ProjectToPlanePoint (playerController.Position, playerController.TouchGroundHeight);
						SignalMgr.instance.Raise (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, curBridge, bridge1);
						UnityLog.Log ("Enter new bridge -> jump to left child bridge");
//						curBridge = bridge1;
					}
					else if (bridge2 != null && hitBlock.bridgeBelong == bridge2 && bridge2.BelowBridge (playerController.Position, playerController.TouchGroundHeight))
					{
						// Jump to bridge2
						playerController.ToStraightMove (curBridge, bridge2);
						playerController.Position = bridge2.ProjectToPlanePoint (playerController.Position, playerController.TouchGroundHeight);
						SignalMgr.instance.Raise (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, curBridge, bridge2);
						UnityLog.Log ("Enter new bridge -> jump to right child bridge");
//						curBridge = bridge2;
					}
				}
			}

			// 检测向上行驶时何时进入下一个bridge
			if (curBridge.Forward == Vector3.up && playerController.CanGoToExceedBridge () && curBridge.ExceedBridgeHeight (playerController.Position))
			{
				LevelBridge bridge1 = curBridge.next0;
				LevelBridge bridge2 = curBridge.next1;
				if (bridge1 != null && bridge1.Up == curBridge.Forward)
				{
					playerController.ExceedToNextBridge (curBridge, bridge1);
					SignalMgr.instance.Raise (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, curBridge, bridge1);
					UnityLog.Log ("Enter new bridge -> exceed to left child bridge");
//					curBridge = bridge1;
				}
				else if (bridge2 != null && bridge2.Up == curBridge.Forward)
				{
					playerController.ExceedToNextBridge (curBridge, bridge2);
					SignalMgr.instance.Raise (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, curBridge, bridge2);
					UnityLog.Log ("Enter new bridge -> exceed to right child bridge");
//					curBridge = bridge2;
				}
			}

			// 检测转向后翻越bridge的情况
			if (curBridge.Forward != Vector3.up && playerController.LevelForward == Vector3.up && curBridge.ExceedBridgeWidth (playerController.Position))
			{
				LevelBridge bridge1 = curBridge.next0;
				LevelBridge bridge2 = curBridge.next1;
				if (bridge1 != null && bridge1.Up == Vector3.up && !bridge1.ExceedBridgeWidth (playerController.Position))
				{
					playerController.TurnExceedToNextBridge (curBridge, bridge1);
					SignalMgr.instance.Raise (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, curBridge, bridge1);
					UnityLog.Log ("Enter new bridge -> turn exceed to left child bridge");
				}
				else if (bridge2 != null && bridge2.Up == Vector3.up && !bridge2.ExceedBridgeWidth (playerController.Position))
				{
					playerController.TurnExceedToNextBridge (curBridge, bridge2);
					SignalMgr.instance.Raise (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, curBridge, bridge2);
					UnityLog.Log ("Enter new bridge -> turn exceed to right child bridge");
				}
			}
		}

		private void UpdateInput ()
		{
			if (Input.GetKeyDown (KeyCode.A))
			{
//				{
//					LevelBridge bridge1 = curBridge.next0;
//					LevelBridge bridge2 = curBridge.next1;
//
//					if (bridge1 != null && bridge1.Forward == -curBridge.Right)
//					{
//						nextBridge = bridge1;
//						playerController.StartTurn (curBridge, nextBridge);
//						curBridge = nextBridge;
//					}
//
//					if (bridge2 != null && bridge2.Forward == -curBridge.Right)
//					{
//						nextBridge = bridge2;
//						playerController.StartTurn (curBridge, nextBridge);
//						curBridge = nextBridge;
//					}
//				}
				playerController.SwipeLeft ();
			}

			if (Input.GetKeyDown (KeyCode.D))
			{
//				{
//					LevelBridge bridge1 = curBridge.next0;
//					LevelBridge bridge2 = curBridge.next1;
//
//					if (bridge1 != null && bridge1.Forward == curBridge.Right)
//					{
//						nextBridge = bridge1;
//						playerController.StartTurn (curBridge, nextBridge);
//						curBridge = nextBridge;
//					}
//
//					if (bridge2 != null && bridge2.Forward == curBridge.Right)
//					{
//						nextBridge = bridge2;
//						playerController.StartTurn (curBridge, nextBridge);
//						curBridge = nextBridge;
//					}
//				}
				playerController.SwipRight ();
			}

			if (Input.GetKeyDown (KeyCode.Space))
			{
				playerController.JumpToWall ();
			}
		}

		private void UpdateCamera ()
		{
			Vector3 bridgeOrigin = playerController.currentBridge.leftBottom;
			float heightFromOrigin = Vector3.Dot (playerController.Position - bridgeOrigin, playerController.currentBridge.Forward);
			float vRatio = heightFromOrigin / playerController.currentBridge.height; 
			vRatio = Mathf.Clamp01 (vRatio);
			float hRatio;
			if (playerController.currentBridge.dummy)
			{
				hRatio = Vector3.Dot (playerController.Position - playerController.currentBridge.leftBottom, playerController.currentBridge.Right) / playerController.currentBridge.width;
				hRatio = Mathf.Clamp01 (hRatio);
			}
			else
			{
				hRatio = playerController.currentBridge.GetBlockByHeight (heightFromOrigin).GetHorizonRatioByPos (playerController.Position);
			}
			camController.UpdateCamera (playerController.currentBridge, hRatio, vRatio);
		}

		#region Util Methods

		public void StartGenerate ()
		{
			levelGenerator.GenerateStartPath (firstLevelAsset, dummyBridge);
			camController.playerController = playerController;
			camController.StartFollow (dummyBridge);
			if (GameSystem.Instance.debugMode)
			{
				PathFindingTest pathfindTest = new GameObject ("PathfindingTest").AddComponent<PathFindingTest> ();
				if (pathfindTest != null)
				{
					pathfindTest.Initialize (dummyBridge.next);
				}

				GameObject alphaOnePrefab = Resources.Load<GameObject> ("Enemy/AlphaOne");
				GameObject alphaGo = Instantiate (alphaOnePrefab) as GameObject;
				EnemyController enemyController = alphaGo.GetComponent<EnemyController> ();

				GameObject targetGo = GameObject.CreatePrimitive (PrimitiveType.Cube);
				targetGo.name = "Target";
				enemyController.target = targetGo.transform;

				enemyController.Initialize (dummyBridge.next, dummyBridge.next.leftBottom);
			}
			initialized = true;

			GameSystem.Instance.AudioMgr.PlayMusic (mainTheme, 2f);
		}

		public void StartPlay ()
		{
			if (startPlay)
				return;

			if (!initialized)
				StartGenerate ();
			startPlay = true;
			playerController.StartRun (dummyBridge);
//			GameSystem.Instance.AudioMgr.PlayMusic (mainTheme, 2f);
		}

		#endregion
	}


}