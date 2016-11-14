using UnityEngine;
using UnityEditor;
using System.Collections;
using MyCompany;
using MyCompany.MyGame.Level;
using MyCompany.MyGame;
using Newtonsoft.Json;
using System.IO;

public class GenerateLevelEditor : EditorWindow
{

	[MenuItem ("MyGame/Level Generator")]
	public static void OpenWindow ()
	{
		GenerateLevelEditor window = EditorWindow.GetWindow<GenerateLevelEditor> ();
		window.Show ();
	}


	string fileName = "";
	string warningInfo = "";
	DichotomyList<LevelBridge> levelPath;
	LevelBridge head;
	int blockCount = 3;
	MyCompany.MyGame.GameDefine.BLOCK_SPECIFICATION bridgeWidth = MyCompany.MyGame.GameDefine.BLOCK_SPECIFICATION.METER_20;
	Vector2 scrollPos = Vector2.zero;
	LevelBridge deleteBridge;

	void OnGUI ()
	{
		if (head == null)
			head = LevelGenerator.FirstDefaultBridge ();

		if (levelPath == null)
			levelPath = new DichotomyList<LevelBridge> (head);

		EditorGUILayout.BeginVertical ();

		fileName = EditorGUILayout.TextField ("File name: ", fileName);
		ELevelType[] avaliableType = GameDefine.CONNECTABLE_TYPE [(int)levelPath.tail.LevelType];

		EditorGUILayout.BeginHorizontal ();
		blockCount = EditorGUILayout.IntField ("Block count: ", blockCount);
		bridgeWidth = (MyCompany.MyGame.GameDefine.BLOCK_SPECIFICATION)EditorGUILayout.EnumPopup ("Bridge width: ", bridgeWidth);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button (avaliableType [0].ToString ()))
		{
			levelPath.AppendLeft (new LevelBridge (avaliableType [0], blockCount, (int)bridgeWidth), true);
		}
		if (GUILayout.Button (avaliableType [1].ToString ()))
		{
			levelPath.AppendRight (new LevelBridge (avaliableType [1], blockCount, (int)bridgeWidth), true);
		}
		EditorGUILayout.EndHorizontal ();

		GUILayout.Space (20f);

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
		foreach (LevelBridge bridge in levelPath)
		{
			EditorGUILayout.BeginHorizontal (GUI.skin.box);

			EditorGUILayout.LabelField (bridge.LevelType.ToString ());
			EditorGUILayout.LabelField (bridge.BlockCount.ToString (), GUILayout.Width (50f));
			if (GUILayout.Button ("-", GUILayout.Width (20f)))
			{
				deleteBridge = bridge;
			}

			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.EndScrollView ();

		if (deleteBridge != null)
		{
			deleteBridge.prev.CutNext ();
			levelPath.MarkDirty ();
			deleteBridge = null;
		}

		if (GUILayout.Button ("Save Level"))
		{
			if (string.IsNullOrEmpty (fileName))
			{
				warningInfo = "Please give a file name to store the file.";
			}
			else
			{
				SerializedBridgeList seriPath = new SerializedBridgeList ();
				seriPath.SerializeSingleBridgePath (levelPath);
				string content = JsonConvert.SerializeObject (seriPath);

				string filePath = GameDefine.SAVED_LEVEL_FULL_PATH + fileName + GameDefine.FILE_EXTENSION_JSON;
				using (StreamWriter sw = new StreamWriter (File.Open (filePath, FileMode.Create)))
				{
					sw.Write (content);
				}
				AssetDatabase.ImportAsset (filePath);
			}
		}
		if (!string.IsNullOrEmpty (warningInfo))
		{
			EditorGUILayout.HelpBox (warningInfo, MessageType.Warning);
		}
		if (!string.IsNullOrEmpty (fileName))
			warningInfo = "";

//		ScriptableObject.c

		EditorGUILayout.EndVertical ();
	}
}

