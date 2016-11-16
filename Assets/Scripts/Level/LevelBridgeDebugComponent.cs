using UnityEngine;
using System.Collections;

namespace MyCompany.MyGame.Level
{
	public class LevelBridgeDebugComponent : MonoBehaviour
	{
		public LevelBridge levelBridge;
		public bool viewMapGrid = true;

		[Header ("View value")]
		public bool viewMapValue = false;

		[System.NonSerialized]
		public bool viewSignValue = true;
		[System.NonSerialized]
		public bool viewPenaltyValue = false;
	}
}

