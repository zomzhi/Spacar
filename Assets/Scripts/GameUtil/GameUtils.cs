using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using PathologicalGames;
using MyCompany.Common;
using MyCompany.Common.Util;
using MyCompany.Common.Interface;

namespace MyCompany.MyGame.Util
{
	public static class GameUtils
	{
		public static ELevelType ChooseRandomLevelType (ELevelType type1, ELevelType type2)
		{
			float rand = Random.value;
			return rand < 0.5f ? type1 : type2;
		}

		public static IEnumerator PreloadInstances (PrefabPool prefabPool, IPreloadable preloadObj)
		{
			Transform inst;

			int numPerFrame = preloadObj.PreloadAmount / preloadObj.PreloadFrames;
			int remainder = preloadObj.PreloadAmount % preloadObj.PreloadFrames;
			for (int j = 0; j < preloadObj.PreloadFrames; j++)
			{
				if (j == preloadObj.PreloadFrames - 1)
					numPerFrame += remainder;

				for (int k = 0; k < numPerFrame; k++)
				{
					inst = prefabPool.SpawnNew ();
					if (inst != null)
						prefabPool.DespawnInstance (inst, false);
				}

				yield return null;
			}
		}

		public static ELevelType GetConnectedRandomType (ELevelType type)
		{
			ELevelType[] avaliableTypes = GameDefine.CONNECTABLE_TYPE [(int)type];
			int connectIndex = MathUtils.RandomBool () ? 0 : 1;
			return avaliableTypes [connectIndex];
		}
	}
}