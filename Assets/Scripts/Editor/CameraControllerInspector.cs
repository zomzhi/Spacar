using UnityEngine;
using UnityEditor;
using System.Collections;
using MyCompany.MyGame.CameraControl;
using MyCompany.MyGame.Level;
using UnityEditor.AnimatedValues;

[CustomEditor (typeof(CameraController))]
public class CameraControllerInspector : Editor
{
	CameraController cameraController;

	AnimBool[] showExtraFields = new AnimBool[6];

	void OnEnable ()
	{
		cameraController = target as CameraController;
		for (int i = 0; i < showExtraFields.Length; i++)
		{
			showExtraFields [i] = new AnimBool (false);
			showExtraFields [i].valueChanged.AddListener (Repaint);
		}
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Previous bridge"))
		{
			cameraController.ToPrevBridge ();
		}
		if (GUILayout.Button ("Next bridge"))
		{
			cameraController.ToNextBridge ();
		}
		EditorGUILayout.EndHorizontal ();


		EditorGUILayout.BeginVertical ();
		GUILayout.Space (10f);
		for (int i = 0; i < cameraController.parameters.Length; i++)
		{
			cameraController.parameters [i].name = ((ELevelType)i).ToString ();
			showExtraFields [i].target = EditorGUILayout.ToggleLeft (cameraController.parameters [i].name, showExtraFields [i].target);
			if (EditorGUILayout.BeginFadeGroup (showExtraFields [i].faded))
			{

				EditorGUI.BeginChangeCheck ();
				GUILayout.Space (10f);
				EditorGUILayout.LabelField ("X angle coefficent: ");
				EditorGUI.indentLevel++;
//				EditorGUILayout.BeginHorizontal ();
				cameraController.parameters [i].xAngleCoef.horizonCoef = EditorGUILayout.FloatField ("horizonCoef: ", cameraController.parameters [i].xAngleCoef.horizonCoef);
				cameraController.parameters [i].xAngleCoef.verticalCoef = EditorGUILayout.FloatField ("verticalCoef: ", cameraController.parameters [i].xAngleCoef.verticalCoef);
				cameraController.parameters [i].xAngleCoef.constCoef = EditorGUILayout.FloatField ("constCoef: ", cameraController.parameters [i].xAngleCoef.constCoef);
//				EditorGUILayout.EndHorizontal ();
				EditorGUI.indentLevel--;

				GUILayout.Space (10f);
				EditorGUILayout.LabelField ("Y angle coefficent: ");
				EditorGUI.indentLevel++;
//				EditorGUILayout.BeginHorizontal ();
				cameraController.parameters [i].yAngleCoef.horizonCoef = EditorGUILayout.FloatField ("horizonCoef: ", cameraController.parameters [i].yAngleCoef.horizonCoef);
				cameraController.parameters [i].yAngleCoef.verticalCoef = EditorGUILayout.FloatField ("verticalCoef: ", cameraController.parameters [i].yAngleCoef.verticalCoef);
				cameraController.parameters [i].yAngleCoef.constCoef = EditorGUILayout.FloatField ("constnCoef: ", cameraController.parameters [i].yAngleCoef.constCoef);
//				EditorGUILayout.EndHorizontal ();
				EditorGUI.indentLevel--;

				GUILayout.Space (10f);
				EditorGUILayout.LabelField ("Zoom in coefficent: ");
				EditorGUI.indentLevel++;
//				EditorGUILayout.BeginHorizontal ();
				cameraController.parameters [i].zoomCoef.horizonCoef = EditorGUILayout.FloatField ("horizonCoef: ", cameraController.parameters [i].zoomCoef.horizonCoef);
				cameraController.parameters [i].zoomCoef.verticalCoef = EditorGUILayout.FloatField ("verticalCoef: ", cameraController.parameters [i].zoomCoef.verticalCoef);
				cameraController.parameters [i].zoomCoef.constCoef = EditorGUILayout.FloatField ("constCoef: ", cameraController.parameters [i].zoomCoef.constCoef);
//				EditorGUILayout.EndHorizontal ();
				EditorGUI.indentLevel--;

				GUILayout.Space (10f);
				EditorGUILayout.LabelField ("Trace distance: ");
				EditorGUI.indentLevel++;
//				EditorGUILayout.BeginHorizontal ();
//				cameraController.parameters [i].headTraceDist = EditorGUILayout.FloatField ("head", cameraController.parameters [i].headTraceDist);
//				cameraController.parameters [i].endTraceDist = EditorGUILayout.FloatField ("end", cameraController.parameters [i].endTraceDist);
				cameraController.parameters [i].verticalTraceDistCurve = EditorGUILayout.CurveField (cameraController.parameters [i].verticalTraceDistCurve);
				cameraController.parameters [i].horizonTraceRatio = EditorGUILayout.Slider ("horizonTraceRatio: ", cameraController.parameters [i].horizonTraceRatio, 0f, 1f);
//				EditorGUILayout.EndHorizontal ();
				EditorGUI.indentLevel--;


				if (EditorGUI.EndChangeCheck ())
				{
					Debug.Log ("Changed");
					Undo.RecordObject (target, "Cam Param Changed");
				}
			}
			EditorGUILayout.EndFadeGroup ();
		}
		EditorGUILayout.EndVertical ();

	}
}

