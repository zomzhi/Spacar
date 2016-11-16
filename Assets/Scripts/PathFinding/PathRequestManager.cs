using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MyCompany.Common;
using MyCompany.MyGame.Level;

namespace MyCompany.MyGame.PathFinding
{
	struct PathRequest
	{
		public LevelBridge bridge;
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest (LevelBridge _bridge, Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
		{
			bridge = _bridge;
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}

	public class PathRequestManager : MonoSingleton<PathRequestManager>
	{
		Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
		PathRequest currentRequest;

		bool isProcessingPath;

		public static void RequestPath (LevelBridge bridge, Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
		{
			PathRequest newRequest = new PathRequest (bridge, pathStart, pathEnd, callback);
			Instance.pathRequestQueue.Enqueue (newRequest);
			Instance.TryProcessNext ();
		}

		void TryProcessNext ()
		{
			if (!isProcessingPath && pathRequestQueue.Count > 0)
			{
				currentRequest = pathRequestQueue.Dequeue ();
				isProcessingPath = true;
				StartCoroutine (StartFindPath ());
			}
		}

		IEnumerator StartFindPath ()
		{
			// A* path finding
			Vector3[] waypoints = new Vector3[0];
			bool success = false;

			LevelBridge bridge = currentRequest.bridge;
			BridgeMap.Node startNode = bridge.NodeFromWorldPoint (currentRequest.pathStart);
			BridgeMap.Node targetNode = bridge.NodeFromWorldPoint (currentRequest.pathEnd);

			if (startNode.IsWalkable () && targetNode.IsWalkable ())
			{
				Heap<BridgeMap.Node> openSet = new Heap<BridgeMap.Node> (bridge.Map.GridCount);
				HashSet<BridgeMap.Node> closedSet = new HashSet<BridgeMap.Node> ();
				openSet.Add (startNode);

				while (openSet.Count > 0)
				{
					BridgeMap.Node currentNode = openSet.RemoveFirst ();
					closedSet.Add (currentNode);

					if (currentNode == targetNode)
					{
						success = true;
						break;
					}

					List<BridgeMap.Node> neighbours = bridge.Map.GetNeighbours (currentNode);
					foreach (BridgeMap.Node neighbour in neighbours)
					{
						if (!neighbour.IsWalkable () || closedSet.Contains (neighbour) || !bridge.Map.DiagonalWalkable (currentNode, neighbour))
							continue;

						int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode.coord, neighbour.coord) + bridge.Map.GetPenalty (neighbour);
						if (!openSet.Contains (neighbour) || newMovementCostToNeighbour < neighbour.gCost)
						{
							neighbour.gCost = newMovementCostToNeighbour;
							neighbour.hCost = GetDistance (neighbour.coord, targetNode.coord);
							neighbour.parent = currentNode;
							if (!openSet.Contains (neighbour))
								openSet.Add (neighbour);
							else
								openSet.UpdateItem (neighbour);
						}
					}
				}
			}
			yield return null;
			if (success)
			{
				waypoints = RetracePath (currentRequest, startNode, targetNode);
			}

			// Finish path finding
			currentRequest.callback (waypoints, success);
			isProcessingPath = false;
			TryProcessNext ();
		}

		/// <summary>
		/// 根据头尾节点返回路径
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="request">Request.</param>
		/// <param name="startNode">Start node.</param>
		/// <param name="endNode">End node.</param>
		Vector3[] RetracePath (PathRequest request, BridgeMap.Node startNode, BridgeMap.Node endNode)
		{
			List<BridgeMap.Node> path = new List<BridgeMap.Node> ();
			BridgeMap.Node currentNode = endNode;

			// 路径将不包含开始坐标
			while (currentNode != startNode)
			{
				path.Add (currentNode);
				currentNode = currentNode.parent;
			}
			// 路径是逆序的，需要reverse
			path.Reverse ();
			Vector3[] waypoints = SimplifyPath (request, path);

//			Array.Reverse (waypoints);
			return waypoints;
		}

		/// <summary>
		/// 将路径按方向进行简化
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="request">Request.</param>
		/// <param name="path">Path.</param>
		Vector3[] SimplifyPath (PathRequest request, List<BridgeMap.Node> path)
		{
			List<Vector3> waypoints = new List<Vector3> ();
			Vector2 directionOld = Vector2.zero;
			LevelBridge bridge = request.bridge;


			for (int i = 1; i < path.Count; i++)
			{
//				Vector2 directionNew = new Vector2 (path [i - 1].coord.x - path [i].coord.x, path [i - 1].coord.y - path [i].coord.y);
//				if (directionNew != directionOld)
				{
					waypoints.Add (bridge.WorldPointFromNode (path [i - 1], 0f));
				}
//				directionOld = directionNew;
			}
			waypoints.Add (request.pathEnd);

			// 选用真正的路径终点而不是终点所处格子的坐标
//			waypoints.Add (request.pathEnd);
//
//			for (int i = 1; i < path.Count; i++)
//			{
//				if (i == path.Count - 1)
//				{
//					waypoints.Add (bridge.WorldPointFromNode (path [i], 0f));
//					break;
//				}
//
//				Vector2 directionNew = new Vector2 (path [i - 1].coord.x - path [i].coord.x, path [i - 1].coord.y - path [i].coord.y);
//				if (directionNew != directionOld)
//				{
//					waypoints.Add (bridge.WorldPointFromNode (path [i], 0f));
//				}
//				directionOld = directionNew;
//			}

			return waypoints.ToArray ();
		}

		/// <summary>
		/// 格子之间的距离，横向移动为1，对角线移动为1.4，乘10取整
		/// </summary>
		/// <returns>The distance.</returns>
		/// <param name="coordA">Coordinate a.</param>
		/// <param name="coordB">Coordinate b.</param>
		int GetDistance (Coordinate coordA, Coordinate coordB)
		{
			int dstX = Mathf.Abs (coordA.x - coordB.x);
			int dstY = Mathf.Abs (coordA.y - coordB.y);

			if (dstX > dstY)
				return 14 * dstY + 10 * (dstX - dstY);
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
}

