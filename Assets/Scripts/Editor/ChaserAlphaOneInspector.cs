using UnityEngine;
using UnityEditor;
using System.Collections;
using MyCompany.MyGame.NPC;

[CustomEditor (typeof(ChaserAlphaOne))]
public class ChaserAlphaOneInspector : Editor
{

	ChaserAlphaOne chaserAlphaOne;

	void OnEnable ()
	{
		chaserAlphaOne = target as ChaserAlphaOne;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EnemyControllerInspector.DrawControllerPathFindingUtil (chaserAlphaOne);
	}
}

