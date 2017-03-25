using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PathologicalGames;
using System;
using MyCompany.MyGame.Util;

namespace MyCompany.MyGame.Obstacle
{
	public class ObstacleFactory
	{
		#region Public Member

		public ObstacleFactory ()
		{

		}

		#endregion


		#region Attribute

		public int ObstacleMaxWidth{ get; private set; }
		public int ObstacleMaxHeight{ get; private set; }

		#endregion


		#region Private Member

		private bool initialized = false;

		private Dictionary<int, List<ObstacleBase>> obstacleDict = new Dictionary<int, List<ObstacleBase>> ();


		private SpawnPool obstaclePool;

		#endregion


		#region Public Methods

		public IEnumerator Init (SpawnPool pool, Action<float> callback)
		{
			if (initialized)
				yield break;

			obstaclePool = pool;
			float progress = 0f;
			if (callback != null)
				callback (progress);

			//string obstalcePrefabPath = GameDefine.RESOURCE_FOLDER + GameDefine.OBSTACLE_PREFAB_RESOURCE_PATH;
			//string[] prefabsPath = Directory.GetFiles (obstalcePrefabPath, "*" + GameDefine.PREFAB_EXTENSION);
			string[] prefabsPath = new string[12];
			prefabsPath[0] = "Assets/Resources/Obstacles/ArchedEntrance 5x1x4.prefab";
			prefabsPath[1] = "Assets/Resources/Obstacles/building1 2x4x6.prefab";
			prefabsPath[2] = "Assets/Resources/Obstacles/building2 2x4x6.prefab";
			prefabsPath[3] = "Assets/Resources/Obstacles/Light1 1x1x4.prefab";
			prefabsPath[4] = "Assets/Resources/Obstacles/Light2 2x2x4.prefab";
			prefabsPath[5] = "Assets/Resources/Obstacles/Light3 1x2x4.prefab";
			prefabsPath[6] = "Assets/Resources/Obstacles/tile1 1x1x3.prefab";
			prefabsPath[7] = "Assets/Resources/Obstacles/tile2 1x1x4.prefab";
			prefabsPath[8] = "Assets/Resources/Obstacles/tile3 3x3x5.prefab";
			prefabsPath[9] = "Assets/Resources/Obstacles/tile4 3x3x4.prefab";
			prefabsPath[10] = "Assets/Resources/Obstacles/tile5 2x2x2.prefab";
			prefabsPath[11] = "Assets/Resources/Obstacles/tile6 3x3x3.prefab";

			for (int i = 0; i < prefabsPath.Length; i++)
			{
				string path = prefabsPath [i];
				string prefabPath = path.Substring (path.LastIndexOf ('/') + 1).Replace (GameDefine.PREFAB_EXTENSION, "");
				prefabPath = GameDefine.OBSTACLE_PREFAB_RESOURCE_PATH + prefabPath;

				ResourceRequest request = Resources.LoadAsync<GameObject> (prefabPath);
				while (!request.isDone)
					yield return null;
				
				progress = (float)(i + 1) / prefabsPath.Length;
				if (callback != null)
					callback (progress);

				GameObject prefab = request.asset as GameObject;
				if (prefab == null)
				{
					UnityLog.LogError ("Missing obstalce prefab : " + prefabPath);
					continue;
				}

				ObstacleBase ob = prefab.GetComponent<ObstacleBase> ();
				if (ob == null)
				{
					UnityLog.LogError ("Obstacle prefab missing component ObstacleBase : " + prefabPath);
					continue;
				}

				List<ObstacleBase> obstacleList = null;
				if (obstacleDict.TryGetValue (ob.width, out obstacleList))
				{
					obstacleList.Add (ob);
				}
				else
				{
					obstacleList = new List<ObstacleBase> ();
					obstacleList.Add (ob);
					obstacleDict [ob.width] = obstacleList;
				}

				if (ob.width > ObstacleMaxWidth)
					ObstacleMaxWidth = ob.width;
				if (ob.height > ObstacleMaxHeight)
					ObstacleMaxHeight = ob.height;

				if (ob.IsPreload)
				{
					// preload some instances
					PrefabPool newPrefabPool = new PrefabPool (prefab.transform);
					newPrefabPool.preloadTime = false;
					newPrefabPool.preloadAmount = 0;
					pool.CreatePrefabPool (newPrefabPool);

					yield return GameUtils.PreloadInstances (newPrefabPool, ob);
				}
			}
			initialized = true;
			yield return null;
		}

		public void Initialize ()
		{
			if (initialized)
				return;

			string obstalcePrefabPath = GameDefine.RESOURCE_FOLDER + GameDefine.OBSTACLE_PREFAB_RESOURCE_PATH;
			string[] prefabsPath = Directory.GetFiles (obstalcePrefabPath, "*" + GameDefine.PREFAB_EXTENSION);
			foreach (string path in prefabsPath)
			{
				string prefabPath = path.Substring (path.LastIndexOf ('/') + 1).Replace (GameDefine.PREFAB_EXTENSION, "");
				prefabPath = GameDefine.OBSTACLE_PREFAB_RESOURCE_PATH + prefabPath;
				GameObject prefab = Resources.Load<GameObject> (prefabPath);
				if (prefab == null)
				{
					UnityLog.LogError ("Missing obstalce prefab : " + prefabPath);
					continue;
				}

				ObstacleBase ob = prefab.GetComponent<ObstacleBase> ();
				if (ob == null)
				{
					UnityLog.LogError ("Obstacle prefab missing component ObstacleBase : " + prefabPath);
					continue;
				}

				List<ObstacleBase> obstacleList = null;
				if (obstacleDict.TryGetValue (ob.width, out obstacleList))
				{
					obstacleList.Add (ob);
				}
				else
				{
					obstacleList = new List<ObstacleBase> ();
					obstacleList.Add (ob);
					obstacleDict [ob.width] = obstacleList;
				}

				if (ob.width > ObstacleMaxWidth)
					ObstacleMaxWidth = ob.width;
				if (ob.height > ObstacleMaxHeight)
					ObstacleMaxHeight = ob.height;
			}
			initialized = true;
		}

		public ObstacleBase GetRandomObstacle (Dictionary<int, int> fitableArea)
		{
			ObstacleBase result = null;
			List<ObstacleBase> availableObstacles = new List<ObstacleBase> ();
			foreach (KeyValuePair<int, int> pair in fitableArea)
			{
				int width = pair.Key;
				int maxHeight = pair.Value;
				List<ObstacleBase> obstacles = null;
				if (obstacleDict.TryGetValue (width, out obstacles))
				{
					availableObstacles.AddRange (obstacles.FindAll (t => t.height <= maxHeight));
				}
			}

			if (availableObstacles.Count > 0)
				result = availableObstacles [UnityEngine.Random.Range (0, availableObstacles.Count)];

			if (result != null)
			{
//				result = GameObject.Instantiate (result) as ObstacleBase;
				Transform inst = obstaclePool.Spawn (result.transform);
				result = inst.GetComponent<ObstacleBase> ();
			}
			return result;
		}

		public void CollectObstacle (ObstacleBase obstacle)
		{
			obstaclePool.Despawn (obstacle.transform, obstaclePool.group);
		}

		#endregion


		#region Private Methods

		private List<ObstacleBase> GetObstaclesByWidth (int width)
		{
			if (obstacleDict.ContainsKey (width))
				return obstacleDict [width];
			return null;
		}

		#endregion
	}
}