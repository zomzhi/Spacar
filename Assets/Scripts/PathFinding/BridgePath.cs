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
			public bool IsFinalDest{ get { return (index == path.Length); } }

			/// <summary>
			/// 最终点的到达距离
			/// </summary>
			/// <value>The reach distance.</value>
			public float ReachDistance{ get; private set; }

			/// <summary>
			/// 此片段路径是否走完
			/// </summary>
			/// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
			public bool Finished{ get; private set; }

			public PathSegment (Vector3[] _path, Action<Vector3> _onReachMiddlePoint, Action _onReachDestination, float _reachDistance = 0f)
			{
				path = _path;
				onReachMiddlePoint = _onReachMiddlePoint;
				onReachDestination = _onReachDestination;
				ReachDistance = _reachDistance;
				reachDistSqr = ReachDistance * ReachDistance;
				Finished = false;
				index = 0;
			}

			private Vector3[] path;
			private Action<Vector3> onReachMiddlePoint;
			private Action onReachDestination;

			private int index;
			private float reachDistSqr;

			public void Update (Vector3 curPosition)
			{
				if (Finished)
					return;

				if (!IsFinalDest)
				{
					if (curPosition == CurDestPos)
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
		}

		public void AddPathSegment (Vector3[] path, Action<Vector3> onReachMiddlePoint, Action onReachDestination, float reachDistance = 0f)
		{
			pathSegmentList.Enqueue (new PathSegment (path, onReachMiddlePoint, onReachDestination, reachDistance));
		}

		public void Update (Vector3 curPosition)
		{
			if (Finished)
				return;

			if (curPathSegment == null || curPathSegment.Finished)
			{
				if (pathSegmentList.Count > 0)
				{
					curPathSegment = pathSegmentList.Dequeue ();
				}
				else
				{
					Finished = true;
				}
			}

			curPathSegment.Update (curPosition);
		}

		public void Dispose ()
		{
			
		}
	}
}

