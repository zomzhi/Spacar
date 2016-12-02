using UnityEngine;
using UnityEditor;
using System.Collections;
using MyCompany.MyGame.NPC;

[CustomEditor (typeof(EnemyController))]
public class EnemyControllerInspector : Editor
{
	EnemyController enemyController;

	void OnEnable ()
	{
		enemyController = target as EnemyController;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		EnemyControllerInspector.DrawControllerPathFindingUtil (enemyController);
	}

	public static void DrawControllerPathFindingUtil (EnemyController controller)
	{
		GUILayout.Space (20f);

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Prev bridge"))
		{
			if (controller.targetBridge.prev != null)
			{
				controller.targetBridge = controller.targetBridge.prev;
				controller.ResetTargetTrans (controller.targetBridge);
			}
		}
		if (GUILayout.Button ("Next bridge"))
		{
			if (controller.targetBridge.next != null)
			{
				controller.targetBridge = controller.targetBridge.next;
				controller.ResetTargetTrans (controller.targetBridge);
			}
		}
		EditorGUILayout.EndHorizontal ();

		if (GUILayout.Button ("Search target"))
		{
			controller.MovetoPosition (controller.targetBridge, controller.target.position);
		}
	}
}

