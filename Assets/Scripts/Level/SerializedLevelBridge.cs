using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyCompany.MyGame.Level
{
	public class SerializedLevelBridge
	{
		public ELevelType type;
		public int blockCount;
		public int bridgeWidth;

		public SerializedLevelBridge (ELevelType type, int blockCount, int bridgeWidth)
		{
			this.type = type;
			this.blockCount = blockCount;
			this.bridgeWidth = bridgeWidth;
		}

		public int prevIndex;
		public int next0Index;
		public int next1Index;
		public bool nextLeft;
	}

	public class SerializedBridgeList
	{
		public SerializedBridgeList ()
		{
		}

		public List<SerializedLevelBridge> bridgeList = new List<SerializedLevelBridge> ();

		public void SerializeSingleBridgePath (DichotomyList<LevelBridge> path)
		{
			bridgeList.Clear ();
			int count = 0;
			foreach (LevelBridge bridge in path)
			{
				SerializedLevelBridge serializedBridge = new SerializedLevelBridge (bridge.LevelType, bridge.BlockCount, bridge.width);
				serializedBridge.prevIndex = count - 1;
				serializedBridge.next0Index = bridge.next0 != null ? count + 1 : -1;
				serializedBridge.next1Index = bridge.next1 != null ? count + 1 : -1;
				serializedBridge.nextLeft = bridge.NextIsLeft ();
				bridgeList.Add (serializedBridge);
				count++;
			}
		}

		public DichotomyList<LevelBridge> DeserializeToPath (LevelBridge firstBridge)
		{
			List<LevelBridge> levelBridgeList = new List<LevelBridge> ();
			foreach (SerializedLevelBridge seriBridge in bridgeList)
			{
				LevelBridge levelBridge = new LevelBridge (seriBridge.type, seriBridge.blockCount, seriBridge.bridgeWidth);
				levelBridgeList.Add (levelBridge);
			}
			for (int i = 0; i < levelBridgeList.Count; i++)
			{
				if (bridgeList [i].prevIndex >= 0)
					levelBridgeList [i].prev = levelBridgeList [bridgeList [i].prevIndex];

				if (bridgeList [i].next0Index >= 0)
					levelBridgeList [i].next0 = levelBridgeList [bridgeList [i].next0Index];

				if (bridgeList [i].next1Index >= 0)
					levelBridgeList [i].next1 = levelBridgeList [bridgeList [i].next1Index];

				if (bridgeList [i].nextLeft)
					levelBridgeList [i].NextLeft ();
				else
					levelBridgeList [i].NextRight ();
			}

			if (firstBridge != null)
			{
				firstBridge.next0 = levelBridgeList [0];
				firstBridge.NextLeft ();
				levelBridgeList [0].prev = firstBridge;
			}
			else
				firstBridge = levelBridgeList [0];

			DichotomyList<LevelBridge> path = new DichotomyList<LevelBridge> (firstBridge);
			path.Refresh ();
			return path;
		}
	}
}

