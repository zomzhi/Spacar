using System.Collections;
using UnityEngine;
using MyCompany.MyGame.Util;
using MyCompany.Common.Util;
using Newtonsoft.Json;
using MyCompany.Common.Signal;
using MyCompany.MyGame.PathFinding;

namespace MyCompany.MyGame.Level
{
	public class LevelGenerator : MonoBehaviour
	{

		#region Public Member

		#endregion

		#region Private Member;

		DichotomyList<LevelBridge> mainPath;
		DichotomyList<LevelBridge> newMainPath;
		DichotomyList<LevelBridge> branchPath;

		DichotomyList<LevelBridge> deletePath;

		bool readyToDelete = false;

		#endregion


		#region Internal Methods

		void Awake ()
		{
			SignalMgr.instance.Subscribe (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, (funsig<LevelBridge, LevelBridge>)OnPlayerSwitchBridge);
			gameObject.AddComponent<PathRequestManager> ();
		}

		void OnDestroy ()
		{
			SignalMgr.instance.Unsubscribe (GAME_EVT.ON_PLAYER_SWITCH_BRIDGE, (funsig<LevelBridge, LevelBridge>)OnPlayerSwitchBridge);
		}

		#endregion

		#region Private Methods

		void OnPlayerSwitchBridge (LevelBridge prevBridge, LevelBridge newBridge)
		{
			if (newMainPath != null && newBridge == newMainPath.head)
			{
				// 进入了新的主路径，准备删除旧主路径
				UnityLog.Log ("Entered new main path, ready to delete main path");
				mainPath = newMainPath;
			}
			else if (branchPath != null && newBridge == branchPath.head)
			{
				// 进入了分支路径，准备删除主路径，标记分支路径为主路径
			}
			else if (newBridge.next == mainPath.tail)
			{
				// 进入主路径的尾部了，需要生成新的主路径,几率在主路径中间生成分支路径
				UnityLog.Log ("Near the tail of the main path, generate new main path.");
				int connectIndex = 0;
				newMainPath = GenerateRandomPath (ref connectIndex, mainPath.tail);
				if (connectIndex == 0)
					mainPath.ConnectListLeft (newMainPath);
				else
					mainPath.ConnectListRight (newMainPath);
				BuildLevelPath (newMainPath);
			}
			else if (branchPath != null && (prevBridge.next0 == branchPath.head || prevBridge.next1 == branchPath.head))
			{
				// 错过了分支路径，准备删除分支路径
			}
		}

		void AppendSomeRandomBridge (DichotomyList<LevelBridge> path)
		{
			for (int i = 0; i < 3; i++)
			{
				ELevelType[] avaliableTypes = GameDefine.CONNECTABLE_TYPE [(int)path.tail.LevelType];
				int connectIndex = MathUtils.RandomBool () ? 0 : 1;

				LevelBridge newBridge = new LevelBridge (avaliableTypes [connectIndex], Random.Range (5, 10), (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
				if (connectIndex == 0)
					path.AppendLeft (newBridge, true);
				else
					path.AppendRight (newBridge, true);
			}
		}

		#endregion

		#region Public Methods

		public void Reset ()
		{
			
		}

		public void GenerateStartPath (TextAsset levelAsset, LevelBridge firstBridge)
		{
//			mainPath = LevelGenerator.GetDeserializedBridge (levelAsset, firstBridge);
//			if (mainPath == null)
//			{
//				mainPath = GeneratePath (firstBridge);
//			}
//			BuildLevelPath (mainPath);

			mainPath = LevelGenerator.GetDeserializedBridge (levelAsset, firstBridge);
			if (mainPath == null)
			{
				int dummy = 0;
				mainPath = new DichotomyList<LevelBridge> (firstBridge);
				mainPath.AppendListLeft (GenerateRandomPath (ref dummy));
			}
			BuildLevelPath (mainPath);
		}

		public DichotomyList<LevelBridge> GenerateRandomPath (ref int connectIndex, LevelBridge connectBridge = null)
		{
			DichotomyList<LevelBridge> path;
			LevelBridge firstBridge;
			if (connectBridge != null)
			{
				ELevelType[] avaliableTypes = GameDefine.CONNECTABLE_TYPE [(int)connectBridge.LevelType];
				connectIndex = MathUtils.RandomBool () ? 0 : 1;

				ELevelType firstType = avaliableTypes [connectIndex];
				firstBridge = new LevelBridge (firstType, Random.Range (5, 10), (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
				firstBridge.prev = connectBridge;
			}
			else
				firstBridge = FirstDefaultBridge ();

			path = new DichotomyList<LevelBridge> (firstBridge);

			AppendSomeRandomBridge (path);
			return path;
		}

		public DichotomyList<LevelBridge> GeneratePath (LevelBridge firstBridge = null)
		{
			DichotomyList<LevelBridge> path;
			if (firstBridge == null)
			{
				firstBridge = FirstDefaultBridge ();
				path = new DichotomyList<LevelBridge> (firstBridge);
			}
			else
			{
				path = new DichotomyList<LevelBridge> (firstBridge);
				path.AppendLeft (FirstDefaultBridge (), true);
			}

			for (int i = 0; i < 10; i++)
			{
				ELevelType[] avaliableTypes = GameDefine.CONNECTABLE_TYPE [(int)path.tail.LevelType];
				int connectIndex = MathUtils.RandomBool () ? 0 : 1;

				LevelBridge newBridge = new LevelBridge (avaliableTypes [connectIndex], Random.Range (5, 10), (int)GameDefine.BLOCK_SPECIFICATION.METER_20);
				if (connectIndex == 0)
					path.AppendLeft (newBridge, true);
				else
					path.AppendRight (newBridge, true);
			}

			return path;
		}

		/// <summary>
		/// 生成实体bridge
		/// </summary>
		/// <param name="path">Path.</param>
		public void BuildLevelPath (DichotomyList<LevelBridge> path)
		{
			foreach (LevelBridge bridge in path)
			{
				UnityLog.Log (bridge.LevelType + " " + bridge.BlockCount + ", ");
				bridge.Build (this);
			}

			foreach (LevelBridge bridge in path)
			{
				bridge.SetupBridge ();
			}
		}

		#endregion


		#region Util methods

		public static LevelBridge FirstDefaultBridge ()
		{
			LevelBridge firstBridge = new LevelBridge (ELevelType.ALONG_X_FACE_Y, 
				                          GameDefine.DEFAULT_FIRST_BRIDGE_BLOCK_COUNT, 
				                          (int)GameDefine.BLOCK_SPECIFICATION.METER_20);	
			return firstBridge;
		}

		/// <summary>
		/// 得到反序列化的bridge列表，需要BuildLevelPath才会生成实体
		/// </summary>
		/// <returns>The deserialized bridge.</returns>
		/// <param name="levelAsset">Level asset.</param>
		/// <param name="firstBridge">First bridge.</param>
		public static DichotomyList<LevelBridge> GetDeserializedBridge (TextAsset levelAsset, LevelBridge firstBridge)
		{
			DichotomyList<LevelBridge> path = null;
			if (levelAsset != null)
			{
				string content = levelAsset.text;
				SerializedBridgeList seriBridgeList = JsonConvert.DeserializeObject<SerializedBridgeList> (content);
				if (seriBridgeList != null)
				{
					path = seriBridgeList.DeserializeToPath (firstBridge);
				}
				else
					UnityLog.LogError ("Deserialize level information failed! use default path.");
			}
			return path;
		}

		#endregion
	}
}

