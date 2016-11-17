using UnityEngine;
using UnityEditor;
using System.Collections;
using MyCompany.MyGame.Level;

[CanEditMultipleObjects]
[CustomEditor (typeof(LevelBridgeDebugComponent))]
public class LevelBridgeInspector : Editor
{
	LevelBridgeDebugComponent levelBridgeDebugComp;
	LevelBridge levelBridge;

	BridgeMap bridgeMap;

	GUIStyle mapValueStyle;

	int fillPercent;

	void OnEnable ()
	{
		levelBridgeDebugComp = target as LevelBridgeDebugComponent;
		levelBridge = levelBridgeDebugComp.levelBridge;
		bridgeMap = levelBridge.Map;

		mapValueStyle = new GUIStyle ();
		mapValueStyle.normal.textColor = Color.black;
		mapValueStyle.alignment = TextAnchor.MiddleCenter;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		if (EditorGUILayout.Toggle ("view sign value: ", levelBridgeDebugComp.viewSignValue))
		{
			levelBridgeDebugComp.viewSignValue = true;
			levelBridgeDebugComp.viewPenaltyValue = false;
		}

		if (EditorGUILayout.Toggle ("view penalty value: ", levelBridgeDebugComp.viewPenaltyValue))
		{
			levelBridgeDebugComp.viewSignValue = false;
			levelBridgeDebugComp.viewPenaltyValue = true;
		}

		GUILayout.Space (20f);

		EditorGUILayout.ColorField ("BreakableArea: ", Color.red);
		EditorGUILayout.ColorField ("MainPathArea: ", Color.green);
		EditorGUILayout.ColorField ("ExtendPathArea: ", Color.yellow);
		EditorGUILayout.ColorField ("ObstacleArea: ", Color.black);

		fillPercent = EditorGUILayout.IntSlider ("Fill percent: ", levelBridge.fillPercent, 0, 100);
		if (levelBridge.fillPercent != fillPercent)
		{
			levelBridge.fillPercent = fillPercent;
			levelBridge.PlaceObstacles ();
		}

		levelBridge.filterPercent = EditorGUILayout.IntSlider ("Filter percent: ", levelBridge.filterPercent, 0, 100);
	}

	void OnSceneGUI ()
	{
		if (!Application.isPlaying)
			return;


		for (int x = 0; x < bridgeMap.Width; x++)
		{
			for (int y = 0; y < bridgeMap.Height; y++)
			{
				Vector3 centerPos = levelBridge.leftBottom + levelBridge.Right * (x + 0.5f) + levelBridge.Forward * (y + 0.5f) + levelBridge.Up * 2f;

				if (bridgeMap.IsObstacle (x, y))
					Handles.color = Color.black;
				else if (bridgeMap.IsBreakable (x, y))
					Handles.color = Color.red;
				else if (bridgeMap.IsMainPath (x, y))
					Handles.color = Color.green;
				else if (bridgeMap.IsExtendPath (x, y))
					Handles.color = Color.yellow;
				else
					Handles.color = Color.white;
//				Handles.RectangleCap (0, centerPos, levelBridge.BridgeGo.transform.rotation * Quaternion.Euler (Vector3.right * -90f), 0.45f);
				float halfRectWidth = 0.45f;
				Vector3[] verts = new Vector3[] {
					centerPos - levelBridge.Right * halfRectWidth + levelBridge.Forward * halfRectWidth,
					centerPos + levelBridge.Right * halfRectWidth + levelBridge.Forward * halfRectWidth,
					centerPos + levelBridge.Right * halfRectWidth - levelBridge.Forward * halfRectWidth,
					centerPos - levelBridge.Right * halfRectWidth - levelBridge.Forward * halfRectWidth,
				};
				if (levelBridgeDebugComp.viewMapGrid)
					Handles.DrawSolidRectangleWithOutline (verts, Handles.color, Color.black);

				if (levelBridgeDebugComp.viewMapValue)
				{
					if (levelBridgeDebugComp.viewSignValue)
						Handles.Label (centerPos, bridgeMap.GetValue (x, y).ToString (), mapValueStyle);
					else if (levelBridgeDebugComp.viewPenaltyValue)
						Handles.Label (centerPos, bridgeMap.GetUnmodifiedPenalty (x, y).ToString (), mapValueStyle);
				}
			}
		}
	}
}

