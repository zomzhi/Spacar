using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(PathFindingTest))]
public class PathFindingTestInspector : Editor
{
	PathFindingTest pathfinding;

	void OnEnable ()
	{
		pathfinding = target as PathFindingTest;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.BeginVertical ();
		if (GUILayout.Button ("Find path"))
		{
			pathfinding.FindPath ();
		}

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Prev bridge"))
		{
			if (pathfinding.bridge.prev != null)
				pathfinding.Reset (pathfinding.bridge.prev);
		}

		if (GUILayout.Button ("Next bridge"))
		{
			if (pathfinding.bridge.next != null)
				pathfinding.Reset (pathfinding.bridge.next);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical ();
	}
}

