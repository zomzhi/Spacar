using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyCompany.MyGame.Level;
using MyCompany.MyGame.NPC;

namespace MyCompany.MyGame.PathFinding
{
	public class PathManager
	{
		/// <summary>
		/// 当前是否正在寻路
		/// </summary>
		/// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
		public bool Finished
		{
			get
			{
				return (bridgeSequence.Count == 0 && currentBridgePath == null);
			}
		}

		private BridgePath currentBridgePath;

		public BridgePath CurrentBridgePath{ get { return currentBridgePath; } }

		private EnemyController controller;
		private Queue<BridgePath> bridgeSequence;
		private Vector3 destination;

		public PathManager (EnemyController _controller)
		{
			controller = _controller;
			destination = Vector3.zero;
			bridgeSequence = new Queue<BridgePath> ();
		}

		public bool SetDestination (LevelBridge destBridge, Vector3 position)
		{
			bool success = destBridge.NodeFromWorldPoint (position).IsWalkable ();
			if (success)
			{						
				destination = position;
				success = DecomposeBridgePath (controller.currentBridge, destBridge);
			}
			return success;
		}

		public void Update ()
		{
			if (Finished)
				return;

			if (currentBridgePath == null/* || currentBridgePath.Finished*/)
			{
				if (controller.CanProceedToNextDestination ())
				{
					currentBridgePath = bridgeSequence.Dequeue ();
					if (bridgeSequence.Count > 0)
						currentBridgePath.InitializeToBridge (bridgeSequence.Peek ().bridge);
					else
						currentBridgePath.InitializeToPosition (destination);

					controller.SetupBridgePath (currentBridgePath);
				}
			}

			if (currentBridgePath != null)
			{
				currentBridgePath.Update (controller.Position);
				if (currentBridgePath.Finished)
				{
					currentBridgePath.Dispose ();
					currentBridgePath = null;
				}
			}
		}

		public void OnDrawGizmos ()
		{
			if (currentBridgePath != null)
			{
				currentBridgePath.OnDrawGizmos ();
			}
		}

		#region Private Methods

		/// <summary>
		/// 分解bridge路径,生成BridgePath队列
		/// </summary>
		/// <returns><c>true</c>, if bridge path was decomposed, <c>false</c> otherwise.</returns>
		/// <param name="currentBridge">Current bridge.</param>
		/// <param name="destBridge">Destination bridge.</param>
		bool DecomposeBridgePath (LevelBridge currentBridge, LevelBridge destBridge)
		{
			if (currentBridge == null || destBridge == null)
				return false;

			ClearCurrentPath ();
			List<LevelBridge> bridgeList = new List<LevelBridge> ();
			LevelBridge p = destBridge;
			while (p.prev != null && p != currentBridge)
			{
				bridgeList.Add (p);
				p = p.prev;
			}

			if (p != currentBridge)
				return false;

			// success
			bridgeList.Add (p);
			bridgeList.Reverse ();
			foreach (LevelBridge bridge in bridgeList)
			{
				bridgeSequence.Enqueue (new BridgePath (bridge));
			}
			p = null;
			bridgeList = null;
			return true;
		}

		void ClearCurrentPath ()
		{
			foreach (BridgePath bridgePath in bridgeSequence)
			{
				bridgePath.Dispose ();
			}
			bridgeSequence.Clear ();
			currentBridgePath = null;
		}

		#endregion
	}
}
 