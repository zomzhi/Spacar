using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using Newtonsoft.Json;
using MyCompany;
using MyCompany.MyGame.Obstacle;
using MyCompany.MyGame.CameraControl;

public class LevelTest : MonoBehaviour
{
	public TextAsset levelAsset;

	DichotomyList<LevelBridge> path;
	LevelBlockFactory blockFactory;
	ObstacleFactory obstacleFactory;

	void Start ()
	{
		blockFactory = new LevelBlockFactory ();
//		blockFactory.Initialize ();
		obstacleFactory = new ObstacleFactory ();
		obstacleFactory.Initialize ();
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

		FindObjectOfType<CameraController> ().StartFollow (path.head);
	}

}

