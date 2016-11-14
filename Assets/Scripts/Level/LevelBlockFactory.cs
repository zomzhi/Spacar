using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using PathologicalGames;
using MyCompany.MyGame.Util;

namespace MyCompany.MyGame.Level
{
	public class LevelBlockFactory
	{
		#region public member

		#endregion

		#region private member

		private bool initialized = false;

		/// <summary>
		/// BlockPrefab信息的封装
		/// </summary>
		class BlockPrefab
		{
			public GameObject prefab;
			public LevelBlock levelBlock;
		}

		/// <summary>
		/// 相同宽度BlockPrefab列表的封装
		/// </summary>
		class BlockPrefabList
		{
			public List<BlockPrefab> sameWidthBlockPrefabs;
			public int blockWidth;
		}

		private List<BlockPrefabList> blockPrefabInfoList = new List<BlockPrefabList> ();

		private SpawnPool levelBlockPool;

		#endregion


		/// <summary>
		/// ctor
		/// </summary>
		public LevelBlockFactory ()
		{
//			initialized = Initialize ();
		}

		/// <summary>
		/// 得到指定宽度BLock列表中随机一个Block
		/// </summary>
		/// <returns>The specific width random block.</returns>
		/// <param name="width">Width.</param>
		public GameObject GetSpecificWidthRandomBlock (int width)
		{
			BlockPrefabList prefabList = GetPrefabListByWidth (width);
			if (prefabList == null)
			{
				UnityLog.LogError ("Block width " + width + " is not exist in factory. Check!");
				return null;
			}
			if (prefabList.sameWidthBlockPrefabs != null && prefabList.sameWidthBlockPrefabs.Count > 0)
			{
				int count = prefabList.sameWidthBlockPrefabs.Count;
				return prefabList.sameWidthBlockPrefabs [UnityEngine.Random.Range (0, count)].prefab;
			}
			else
			{
				UnityLog.LogError ("prefabList.sameWidthBlockPrefabs == null ? WTF");
				return null;
			}
		}

		public Transform GetBlockByWidth (int width)
		{
			GameObject prefab;
			BlockPrefabList prefabList = GetPrefabListByWidth (width);
			if (prefabList == null)
			{
				UnityLog.LogError ("Block width " + width + " is not exist in factory. Check!");
				return null;
			}
			if (prefabList.sameWidthBlockPrefabs != null && prefabList.sameWidthBlockPrefabs.Count > 0)
			{
				int count = prefabList.sameWidthBlockPrefabs.Count;
				prefab = prefabList.sameWidthBlockPrefabs [UnityEngine.Random.Range (0, count)].prefab;
			}
			else
			{
				UnityLog.LogError ("prefabList.sameWidthBlockPrefabs == null ? WTF");
				return null;
			}

			return levelBlockPool.Spawn (prefab.transform);
		}

		/// <summary>
		/// 加载所有Block prefab预制体并预加载放入对象池
		/// </summary>
		/// <param name="pool">Pool.</param>
		/// <param name="callback">Callback.</param>
		public IEnumerator Init (SpawnPool pool, Action<float> callback)
		{
			if (initialized)
				yield break;

			levelBlockPool = pool;
			float progress = 0f;

			string blockPrefabPath = GameDefine.RESOURCE_FOLDER + GameDefine.BLOCK_PREFAB_RESOURCE_PATH;
			string[] prefabsPath = Directory.GetFiles (blockPrefabPath, "*" + GameDefine.PREFAB_EXTENSION);

			for (int i = 0; i < prefabsPath.Length; i++)
			{
				string path = prefabsPath [i];
				string prefabPath = path.Substring (path.LastIndexOf ('/') + 1).Replace (GameDefine.PREFAB_EXTENSION, "");
				prefabPath = GameDefine.BLOCK_PREFAB_RESOURCE_PATH + prefabPath;

				// load prefab
				ResourceRequest request = Resources.LoadAsync<GameObject> (prefabPath);
				while (!request.isDone)
					yield return null;

				progress = (float)(i + 1) / prefabsPath.Length;
				if (callback != null)
					callback (progress);

				GameObject prefab = request.asset as GameObject;
				if (prefab == null)
				{
					UnityLog.LogError ("Missing block prefab : " + prefabPath);
					continue;
				}

				LevelBlock lb = prefab.GetComponent<LevelBlock> ();
				if (lb == null)
				{
					UnityLog.LogError ("Block prefab missing component LevelBlock : " + prefabPath);
					continue;
				}

				// add to list by width
				int blockWidth = lb.width;
				BlockPrefabList prefabList = GetPrefabListByWidth (blockWidth);
				if (prefabList == null)
				{
					prefabList = new BlockPrefabList ();
					prefabList.blockWidth = blockWidth;
					prefabList.sameWidthBlockPrefabs = new List<BlockPrefab> ();
					blockPrefabInfoList.Add (prefabList);
				}
				BlockPrefab blockPrefab = new BlockPrefab ();
				blockPrefab.levelBlock = lb;
				blockPrefab.prefab = prefab;
				prefabList.sameWidthBlockPrefabs.Add (blockPrefab);

				if (lb.IsPreload)
				{
					// preload some instances
					// SpawnPool的预加载有问题，这里使用自己的预加载
					PrefabPool newPrefabPool = new PrefabPool (prefab.transform);
					newPrefabPool.preloadTime = false;
					newPrefabPool.preloadAmount = 0;
					pool.CreatePrefabPool (newPrefabPool);

					yield return GameUtils.PreloadInstances (newPrefabPool, lb);
				}
			}

			blockPrefabInfoList.Sort (((x, y) => {
				return x.blockWidth.CompareTo (y.blockWidth);
			}));
			initialized = true;
			yield return null;
		}

		#region private methods

		/// <summary>
		/// 初始化，加载Resource下BlockPrefab目录下的所有Prefab
		/// </summary>
		[Obsolete ("已改为异步加载初始化方法Init")]
		public bool Initialize ()
		{
			string blockPrefabPath = GameDefine.RESOURCE_FOLDER + GameDefine.BLOCK_PREFAB_RESOURCE_PATH;
			string[] prefabsPath = Directory.GetFiles (blockPrefabPath, "*" + GameDefine.PREFAB_EXTENSION);
			foreach (string path in prefabsPath)
			{
				string prefabPath = path.Substring (path.LastIndexOf ('/') + 1).Replace (GameDefine.PREFAB_EXTENSION, "");
				prefabPath = GameDefine.BLOCK_PREFAB_RESOURCE_PATH + prefabPath;
				GameObject prefab = Resources.Load<GameObject> (prefabPath);
				if (prefab == null)
				{
					UnityLog.LogError ("Missing block prefab : " + prefabPath);
					continue;
				}

				LevelBlock lb = prefab.GetComponent<LevelBlock> ();
				if (lb == null)
				{
					UnityLog.LogError ("Block prefab missing component LevelBlock : " + prefabPath);
					continue;
				}

				int blockWidth = lb.width;
				BlockPrefabList prefabList = GetPrefabListByWidth (blockWidth);
				if (prefabList == null)
				{
					prefabList = new BlockPrefabList ();
					prefabList.blockWidth = blockWidth;
					prefabList.sameWidthBlockPrefabs = new List<BlockPrefab> ();
					blockPrefabInfoList.Add (prefabList);
				}
				BlockPrefab blockPrefab = new BlockPrefab ();
				blockPrefab.levelBlock = lb;
				blockPrefab.prefab = prefab;
				prefabList.sameWidthBlockPrefabs.Add (blockPrefab);
			}

			blockPrefabInfoList.Sort (((x, y) => {
				return x.blockWidth.CompareTo (y.blockWidth);
			}));

			return true;
		}

		/// <summary>
		/// 获得指定宽度的BlockPrefab列表
		/// </summary>
		/// <returns>The prefab list by width.</returns>
		/// <param name="width">Width.</param>
		private BlockPrefabList GetPrefabListByWidth (int width)
		{
			foreach (BlockPrefabList prefabList in blockPrefabInfoList)
			{
				if (prefabList.blockWidth == width)
					return prefabList;
			}
			return null;
		}

		#endregion
	}
}
