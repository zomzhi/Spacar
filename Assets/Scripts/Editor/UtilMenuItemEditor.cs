using UnityEditor;
using MyCompany.MyGame;
using System.IO;
using MyCompany.MyGame.Obstacle;
using UnityEngine;

public class UtilMenuItemEditor
{
	[MenuItem ("MyGame/Set obstacle width height")]
	static void SetObstacleWidthHeight ()
	{
		Debug.Log ("WTF?");
		string obstalcePrefabPath = GameDefine.RESOURCE_FOLDER + GameDefine.OBSTACLE_PREFAB_RESOURCE_PATH;
		string[] prefabsPath = Directory.GetFiles (obstalcePrefabPath, "*" + GameDefine.PREFAB_EXTENSION);
		foreach (string path in prefabsPath)
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject> (path);
			if (prefab != null)
			{
				ObstacleBase ob = prefab.GetComponent<ObstacleBase> ();
				if (ob == null)
				{
					Debug.LogError ("Missing Obstacle component for " + path, prefab);
					continue;
				}

				string prefabPath = path.Substring (path.LastIndexOf ('/') + 1).Replace (GameDefine.PREFAB_EXTENSION, "");
				prefabPath = prefabPath.Split (' ') [1];

				string[] specific = prefabPath.Split ('x');
				int width, height;
				if (specific.Length == 3 && int.TryParse (specific [0], out width) && int.TryParse (specific [1], out height))
				{
					SerializedObject serializedObject = new SerializedObject (ob);
					SerializedProperty serializeWidth = serializedObject.FindProperty ("specificWidth");
					SerializedProperty serializeHeight = serializedObject.FindProperty ("specificHeight");
					serializeWidth.intValue = width;
					serializeHeight.intValue = height;
					serializedObject.ApplyModifiedProperties ();
				}
				else
					Debug.LogError ("Naming format is incorrect! Example: Obstacle 1x1x1", prefab);
			}
		}
	}

}