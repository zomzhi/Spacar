using UnityEngine;
using UnityEditor;
using System.Collections;
using MyCompany.MyGame.Level;
using MyCompany.MyGame;

public class EditorTestWindow : EditorWindow
{
	[MenuItem ("MyGame/TestWindow")]
	public static void OpenWindow ()
	{
		EditorTestWindow window = EditorWindow.GetWindow<EditorTestWindow> ();
		window.Show ();
	}

	ELevelType type = ELevelType.ALONG_X_FACE_Y;

	void OnGUI ()
	{
		type = (ELevelType)EditorGUILayout.EnumPopup (type);

		if (GUILayout.Button ("Rotate"))
		{
			GameObject go = Selection.activeGameObject;
			if (go != null)
			{
				go.transform.rotation = GameDefine.BLOCK_ROTATION [(int)type];
			}
		} 

		if (GUILayout.Button ("Test"))
		{
			
		}
	}
}
