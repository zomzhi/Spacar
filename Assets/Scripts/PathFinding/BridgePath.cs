using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using System;
using System.Collections.Generic;

namespace MyCompany.MyGame.PathFinding
{
	public class BridgePath : IDisposable
	{
		public enum PathType
		{
			ToNextBridge,
			ToPosition,
		}

		/// <summary>
		/// 路径片段
		/// </summary>
		public class PathSegment
		{
			/// <summary>
			/// 当前目标位置
			/// </summary>
			/// <value>The current destination position.</value>
			public Vector3 CurDestPos{ get { return path [index]; } }

			/// <summary>
			/// 是否是最后的目标点
			/// </summary>
			/// <value><c>true</c> if this instance is final destination; otherwise, <c>false</c>.</value>
			public bool IsFinalDest{ get { return (index == path.Length - 1); } }

			/// <summary>
			/// 最终点的到达距离
			/// </summary>
			/// <value>The reach distance.</value>
			public float ReachDistance{ get; private set; }

			/// <summary>
			/// 路径片段是否准备完毕，即路径是否请求并计算完毕
			/// </summary>
			/// <value><c>true</c> if ready; otherwise, <c>false</c>.</value>
			public bool Ready{ get; private set; }

			/// <summary>
			/// 此片段路径是否走完
			/// </summary>
			/// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
			public bool Finished{ get; private set; }


			public PathSegment (Action<Vector3> _onStart, Action<Vector3> _onReachMiddlePoint, Action _onReachDestination, float _reachDistance = 0f)
			{
				onStart = _onStart;
				onReachMiddlePoint = _onReachMiddlePoint;
				onReachDestination = _onReachDestination;
				ReachDistance = _reachDistance;
				reachDistSqr = ReachDistance * ReachDistance;
				Finished = false;
				Ready = false;
				index = 0;
			}

			private Vector3[] path;
			private Action<Vector3> onReachMiddlePoint;
			private Action onReachDestination;
			private Action<Vector3> onStart;

			private int index;
			private float reachDistSqr;

			public void Update (Vector3 curPosition)
			{
				if (Finished)
					return;

				if (!IsFinalDest)
				{
					// TODO: distance compare
					float distSqr = (curPosition - CurDestPos).sqrMagnitude;
					if (distSqr <= 0.1f)
//					if (curPosition == CurDestPos)
					{
						MoveNext ();
						if (onReachMiddlePoint != null)
							onReachMiddlePoint (CurDestPos);
					}
				}
				else
				{
					float distSqr = (curPosition - CurDestPos).sqrMagnitude;
					if (distSqr <= reachDistSqr)
					{
						if (onReachDestination != null)
							onReachDestination ();
						Finished = true;
					}
				}
			}

			public void SetPath (Vector3[] _path)
			{
				path = _path;
				Ready = true;
			}

			public void PathBegin ()
			{
				onStart (path [0]);
			}

			public void OnDrawGizmos ()
			{
				Gizmos.color = Color.black;
				for (int i = 0; i < path.Length; i++)
				{
					Gizmos.DrawCube (path [i], Vector3.one * 0.25f);
					if (i > 0)
					{
						Gizmos.DrawLine (path [i - 1], path [i]);
					}
				}
			}

			void MoveNext ()
			{
				index++;
			}
		}

		#region Public Member

		/// <summary>
		/// 当前路径的bridge
		/// </summary>
		public LevelBridge bridge;

		/// <summary>
		/// 当前bridge路径的类型，是去往下一个bridge还是到当前bridge的目标点
		/// </summary>
		/// <value>The type.</value>
		public PathType Type{ get; private set; }


		/// <summary>
		/// 目的地
		/// </summary>
		public Vector3 destination;

		/// <summary>
		/// 所有片段路径均计算好后准备就绪
		/// </summary>
		/// <value><c>true</c> if ready; otherwise, <c>false</c>.</value>
		public bool ready;

		/// <summary>
		/// 当前bridge路径是否走完
		/// </summary>
		/// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
		public bool Finished{ get; private set; }

		/// <summary>
		/// 连接的下一个bridge
		/// </summary>
		public LevelBridge nextBridge;

		public BridgePath (LevelBridge _bridge)
		{
			bridge = _bridge;
			Finished = false;
			ready = false;
			pathSegmentList = new Queue<PathSegment> ();
		}

		#endregion


		#region Private Member

		private Queue<PathSegment> pathSegmentList;

		private PathSegment curPathSegment;

		#endregion


		/// <summary>
		/// 初始化当前bridge连接到下一个bridge
		/// </summary>
		/// <param name="bridge">下一个bridge.</param>
		public void InitializeToBridge (LevelBridge bridge)
		{
			Type = PathType.ToNextBridge;
			nextBridge = bridge;
		}

		/// <summary>
		/// 初始化当前bridge到最终点
		/// </summary>
		/// <param name="position">Position.</param>
		public void InitializeToPosition (Vector3 position)
		{
			Type = PathType.ToPosition;
			destination = position;
		}

		/// <summary>
		/// 创建路径片段，路径片段在请求的路径计算完毕后才准备就绪
		/// </summary>
		/// <returns>The path segment.</returns>
		/// <param name="onStart">On start.</param>
		/// <param name="onReachMiddlePoint">On reach middle point.</param>
		/// <param name="onReachDestination">On reach destination.</param>
		/// <param name="reachDistance">Reach distance.</param>
		public PathSegment CreatePathSegment (Action<Vector3> onStart, Action<Vector3> onReachMiddlePoint, Action onReachDestination, float reachDistance = 0.1f)
		{
			PathSegment ps = new PathSegment (onStart, onReachMiddlePoint, onReachDestination, reachDistance);
			pathSegmentList.Enqueue (ps);
			return ps;
		}

		public void Update (Vector3 curPosition)
		{
			if (Finished)
				return;

			if (!ready)
			{
				if (pathSegmentList.Count > 0)
				{
					ready = true;
					foreach (PathSegment segment in pathSegmentList)
					{
						if (!segment.Ready)
						{
							ready = false;
							break;
						}
					}
				}

				if (!ready)
					return;
			}

			if (curPathSegment == null || curPathSegment.Finished)
			{
				if (curPathSegment != null)
				{
					// TODO: dispose
				}

				if (pathSegmentList.Count > 0)
				{
					curPathSegment = pathSegmentList.Dequeue ();
					curPathSegment.PathBegin ();
				}
				else
				{
					Finished = true;
				}
			}

			curPathSegment.Update (curPosition);
		}

		public void OnDrawGizmos ()
		{
			if (ready)
			{
				if (curPathSegment != null)
					curPathSegment.OnDrawGizmos ();
				foreach (PathSegment segment in pathSegmentList)
				{
					segment.OnDrawGizmos ();
				}
			}
		}

		public void Dispose ()
		{
			// TOOD: dispose
		}
	}
}

