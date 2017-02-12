using UnityEditor;
using UnityEngine;
using MyCompany.MyGame.Level;

[CustomEditor (typeof(LevelBlock))]
public class LevelBlockInspector : Editor
{
	LevelBlock levelBlock;

	SerializedProperty specificWidthProp;

	void OnEnable ()
	{
		levelBlock = target as LevelBlock;
	}

	public override void OnInspectorGUI ()
	{
//		DrawDefaultInspector ();
		serializedObject.Update ();
		specificWidthProp = serializedObject.FindProperty ("specificWidth");
		EditorGUILayout.PropertyField (specificWidthProp);

		specificWidthProp = serializedObject.FindProperty ("isPreload");
		EditorGUILayout.PropertyField (specificWidthProp);
		specificWidthProp = serializedObject.FindProperty ("preloadAmount");
		EditorGUILayout.PropertyField (specificWidthProp);
		specificWidthProp = serializedObject.FindProperty ("preloadFrames");
		EditorGUILayout.PropertyField (specificWidthProp);
		specificWidthProp = serializedObject.FindProperty ("connectType");
		EditorGUILayout.PropertyField (specificWidthProp);

		serializedObject.ApplyModifiedProperties ();

		levelBlock.normalStartIndex = EditorGUILayout.IntSlider ("Normal start index: ", levelBlock.normalStartIndex, 0, levelBlock.width - 1);
		levelBlock.normalEndIndex = EditorGUILayout.IntSlider ("Normal end index: ", levelBlock.normalEndIndex, levelBlock.normalStartIndex, levelBlock.width - 1);
		levelBlock.placeable = EditorGUILayout.Toggle ("Placeable: ", levelBlock.placeable);

		GUILayout.Space (10f);

	}

	void OnSceneGUI ()
	{
		if (!levelBlock.viewMapGrid)
			return;

		for (int x = 0; x < levelBlock.width; x++)
		{
			for (int y = 0; y < levelBlock.height; y++)
			{
				Vector3 pos = levelBlock.leftBottom - levelBlock.transform.forward * (x + 0.5f) + levelBlock.transform.right * (y + 0.5f) + levelBlock.transform.up * 2f;
				if (x >= levelBlock.normalStartIndex && x <= levelBlock.normalEndIndex)
					Handles.color = Color.white;
				else
					Handles.color = levelBlock.placeable ? Color.yellow : Color.red;
//				Handles.RectangleCap (0, pos, levelBlock.transform.rotation * Quaternion.Euler (Vector3.right * -90f), 0.5f);

				float halfRectWidth = 0.45f;
				Vector3[] verts = new Vector3[] {
					pos + levelBlock.transform.forward * halfRectWidth + levelBlock.transform.right * halfRectWidth,
					pos - levelBlock.transform.forward * halfRectWidth + levelBlock.transform.right * halfRectWidth,
					pos - levelBlock.transform.forward * halfRectWidth - levelBlock.transform.right * halfRectWidth,
					pos + levelBlock.transform.forward * halfRectWidth - levelBlock.transform.right * halfRectWidth,
				};
				Handles.DrawSolidRectangleWithOutline (verts, Handles.color, Color.black);
			}
		}
	}
}

