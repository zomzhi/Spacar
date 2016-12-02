using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using MyCompany.MyGame.PathFinding;
using MyCompany.MyGame;

public class PathFindingTest : MonoBehaviour
{
	#if UNITY_EDITOR
	public LevelBridge bridge;
	public Transform seeker;
	public Transform target;

	Vector3[] waypoints;

	bool initialized = false;

	public void Initialize (LevelBridge _bridge)
	{
		bridge = _bridge;
		seeker = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		target = GameObject.CreatePrimitive (PrimitiveType.Sphere).transform;
		seeker.SetParent (transform);
		target.SetParent (transform);

		seeker.GetComponent<Renderer> ().material.color = Color.green;
		target.GetComponent<Renderer> ().material.color = Color.red;

		Reset (bridge);
		initialized = true;
	}

	public void FindPath ()
	{
		if (initialized)
			PathRequestManager.RequestPath (bridge, seeker.position, target.position, OnPathFound, 0f);
	}

	public void Reset (LevelBridge _bridge)
	{
		bridge = _bridge;
		seeker.position = bridge.leftBottom + bridge.Up * GameDefine.BLOCK_TALL;
		target.position = bridge.leftBottom + bridge.Right * bridge.width + bridge.Up * GameDefine.BLOCK_TALL;
	}

	void OnPathFound (Vector3[] path, bool success)
	{
		if (success)
		{
			waypoints = path;
		}
	}

	void OnDrawGizmos ()
	{
		if (waypoints != null)
		{
			Gizmos.color = Color.black;
			for (int i = 0; i < waypoints.Length; i++)
			{
				Gizmos.DrawCube (waypoints [i], Vector3.one * 0.25f);
				if (i + 1 < waypoints.Length)
					Gizmos.DrawLine (waypoints [i], waypoints [i + 1]);
			}
		}
	}
	#endif
}

