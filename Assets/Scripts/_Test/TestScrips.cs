using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using MyCompany.MyGame;
using MyCompany.Common.Util;
using MyCompany.MyGame.Player;
using MyCompany;
using Newtonsoft.Json;
using MyCompany.MyGame.Obstacle;

public class TestScrips : MonoBehaviour
{
	public static TestScrips Instance;

	public TextAsset levelAsset;


	const string PLAYER_PREFAB = "Player/Player";

	PlayerController playerController;

	DichotomyList<LevelBridge> path;

	public LevelBridge curBridge;
	LevelBridge nextBridge;

	ObstacleFactory obstacleFactory;

	void Awake ()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start ()
	{
//		LevelBlockFactory.Instance.Test ();
//		ObstacleFactory.Instance.Initialize ();
		obstacleFactory = new ObstacleFactory ();
		obstacleFactory.Initialize ();
//		Test ();

//		LevelGenerator.Instance.GenerateMainPath ();
		if (levelAsset != null)
		{
			string content = levelAsset.text;
			SerializedBridgeList seriBridgeList = JsonConvert.DeserializeObject<SerializedBridgeList> (content);
			if (seriBridgeList != null)
			{
				path = seriBridgeList.DeserializeToPath (null);
			}
			else
			{
				UnityLog.LogError ("Deserialize level information failed! use default path.");
//				path = LevelGenerator.Instance.GeneratePath ();
			}
		}
//		else
//			path = LevelGenerator.Instance.GeneratePath ();
		curBridge = path.head;
		LevelBlock secondBlock = curBridge.GetBlockByIndex (1);

//		Vector3 centerPos = secondBlock.leftBottom + curBridge.Right * secondBlock.width * 0.5f +
//		                    curBridge.Forward * secondBlock.height * 0.5f + curBridge.Up * (GameDefine.BLOCK_TALL);

		GameObject playerPrefab = Resources.Load<GameObject> (PLAYER_PREFAB);
		playerController = GameObject.Instantiate (playerPrefab).GetComponent<PlayerController> ();
//		playerController.transform.position = centerPos;

//		playerController.InitPlayAttribute ();
//		playerController.PlaceOnBlock (secondBlock);
//		playerController.InitLevelParam (curBridge);

//		playerController.StartRun ();
	}
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.L))
		{
			// turn left
//			if (curBridge.WithinLastBlock (playerController.transform.position))
			{
				LevelBridge bridge1 = curBridge.next0;
				LevelBridge bridge2 = curBridge.next1;

				if (bridge1 != null && bridge1.Forward == -curBridge.Right)
				{
					nextBridge = bridge1;
					playerController.StartTurn (curBridge, nextBridge);
					curBridge = nextBridge;
				}

				if (bridge2 != null && bridge2.Forward == -curBridge.Right)
				{
					nextBridge = bridge2;
					playerController.StartTurn (curBridge, nextBridge);
					curBridge = nextBridge;
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.R))
		{
			// turn right
//			if (curBridge.WithinLastBlock (playerController.transform.position))
			{
				LevelBridge bridge1 = curBridge.next0;
				LevelBridge bridge2 = curBridge.next1;

				if (bridge1 != null && bridge1.Forward == curBridge.Right)
				{
					nextBridge = bridge1;
					playerController.StartTurn (curBridge, nextBridge);
					curBridge = nextBridge;
				}

				if (bridge2 != null && bridge2.Forward == curBridge.Right)
				{
					nextBridge = bridge2;
					playerController.StartTurn (curBridge, nextBridge);
					curBridge = nextBridge;
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Space))
		{
			playerController.JumpToWall ();
		}
	}

	void LateUpdate ()
	{
		// TODO: 看是用脚底的position还是pivot的position
		if (curBridge.BelowBridge (playerController.Position))
		{
			// 比当前bridge平面低，失败
			return;
		}

		// 检测跳起状态何时碰触下一个bridge
		if (playerController.IsInJumpToWallState ())
		{
			LevelBridge bridge1 = curBridge.next0;
			LevelBridge bridge2 = curBridge.next1;
			RaycastHit hit;
			if (Physics.Raycast (playerController.Position, curBridge.Forward, out hit, 10, 1 << LayerMask.NameToLayer (GameDefine.GROUND_LAYER_NAME)))
			{
				LevelBlock hitBlock = hit.transform.GetComponent<LevelBlock> ();
				UnityLog.Assert (hitBlock != null, "ground missing LevelBlock component. wtf", hit.transform.gameObject);
				if (bridge1 != null && hitBlock.bridgeBelong == bridge1 && bridge1.BelowBridge (playerController.Position, playerController.TouchGroundHeight))
				{
					// Jump to bridge1
					playerController.ToStraightMove (curBridge, bridge1);
					playerController.Position = bridge1.ProjectToPlanePoint (playerController.Position, playerController.TouchGroundHeight);
					curBridge = bridge1;
				}
				else if (bridge2 != null && hitBlock.bridgeBelong == bridge2 && bridge2.BelowBridge (playerController.Position, playerController.TouchGroundHeight))
				{
					// Jump to bridge2
					playerController.ToStraightMove (curBridge, bridge2);
					playerController.Position = bridge2.ProjectToPlanePoint (playerController.Position, playerController.TouchGroundHeight);
					curBridge = bridge2;
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
				curBridge = bridge1;
			}
			else if (bridge2 != null && bridge2.Up == curBridge.Forward)
			{
				playerController.ExceedToNextBridge (curBridge, bridge2);
				curBridge = bridge2;
			}
		}
	}

	void Test ()
	{
		/*
		int offset = 0;
		for (int i = 0; i <= (int)ELevelType.ALONG_Z_FACE_Y; i++)
		{
			LevelBridge head = new LevelBridge ((ELevelType)i, 2, (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
			head.Build ();

			head.bridgeGo.transform.position = new Vector3 (offset, 0f, offset);

			ELevelType[] connectedTypes = GameDefine.CONNECTABLE_TYPE [i];
			LevelBridge leftBridge = new LevelBridge (connectedTypes [0], 3, (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
			leftBridge.prev = head;
			leftBridge.Build ();
			LevelBridge rightBridge = new LevelBridge (connectedTypes [1], 4, (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
			rightBridge.prev = head;
			rightBridge.Build ();

			offset += 50;
		}
		*/

		/*
		LevelBridge head = new LevelBridge (ELevelType.ALONG_X_FACE_Y, 
			                   GameDefine.DEFAULT_FIRST_BRIDGE_BLOCK_COUNT, 
			                   (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
		head.Build ();
		LevelBridge p = head;

		for (int i = 0; i < 20; i++)
		{
			ELevelType[] avaliableTypes = GameDefine.CONNECTABLE_TYPE [(int)p.levelType];
			int index = MathUtils.RandomBool () ? 0 : 1;
			LevelBridge bridge = new LevelBridge (avaliableTypes [index], Random.Range (3, 10), (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
			bridge.prev = p;
			bridge.Build ();
			if (index == 0)
			{
				p.next0 = bridge;
				p.NextLeft ();
			}
			else
			{
				p.next1 = bridge;
				p.NextRight ();
			}

			p = p.next;
		}
		*/
	}
}

