using UnityEngine;
using System.Collections;
using MyCompany.MyGame.PathFinding;
using MyCompany.MyGame.Data.Character;
using HutongGames.PlayMaker;
using MyCompany.MyGame.Util;
using System;

namespace MyCompany.MyGame.NPC
{
	public class ChaserAlphaOne : EnemyController
	{
		FsmVector3 curDestinationVar;

		protected override void OnAwake ()
		{
			curDestinationVar = motionFsm.Variables.FindFsmVector3 ("curDestination");
			UnityLog.Assert (curDestinationVar != null, "curDestination variable not found! pls check.", gameObject);
		}

		protected override void InitControllerAttribute ()
		{
			EnemyAttribute enemyAttribute = characterAttribute as EnemyAttribute;
			motionFsm.GetFsmFloat ("moveSpeed").Value = enemyAttribute.moveSpeed;
			motionFsm.GetFsmFloat ("rotateSpeed").Value = enemyAttribute.rotateSpeed;
		}

		protected override void OnPathStart (Vector3 firstPoint)
		{
			base.OnPathStart (firstPoint);
			curDestinationVar.Value = firstPoint;
			MoveEvent ();
		}

		protected override void OnPathReachMiddlePoint (Vector3 nextPoint)
		{
			base.OnPathReachMiddlePoint (nextPoint);
			curDestinationVar.Value = nextPoint;
		}

		protected override void OnPathReachDestination ()
		{
			base.OnPathReachDestination ();
			StopMoveEvent ();
		}

		protected override void SetupToPosBridgePath (BridgePath bridgePath)
		{
			BridgePath.PathSegment segment = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, OnPathReachDestination, 0.1f);
			PathRequestManager.RequestPath (bridgePath.bridge, Position, bridgePath.destination, ((Vector3[] path, bool success) => {
				if (success)
				{
					UnityLog.Log ("SetupToPosBridgePath Find path");
					segment.SetPath (path);
					FsmArray pathVar = motionFsm.Variables.FindFsmArray ("path");
					pathVar.Reset ();
					pathVar.ElementType = VariableType.Vector3;
					pathVar.Resize (path.Length);
					for (int i = 0; i < path.Length; i++)
					{
						pathVar.Set (i, path [i]);
					}
				}
				else
				{
					Debug.Break ();
					UnityLog.LogError ("Path Request failed! " + Position + " -> " + bridgePath.destination);
				}
			}), touchGroundHeight);
		}

		protected override void SetupTurnBridgePath (BridgePath bridgePath)
		{
//			Vector3 middlePos = bridgePath.bridge.WorldPointFromNode (bridgePath.bridge.GetConnectMiddleNode (bridgePath.nextBridge), touchGroundHeight);
			Vector3 middlePos = GameUtils.GetConnectBridgeMiddlePos (bridgePath.bridge, bridgePath.nextBridge, this.touchGroundHeight);
			BridgePath.PathSegment firstSegment = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, OnPathReachDestination, 0.1f);
			PathRequestManager.RequestPath (bridgePath.bridge, Position, middlePos, (path, success) => {
				if (success)
				{
					firstSegment.SetPath (path);
				}
			}, touchGroundHeight);

			Vector3 endPos = bridgePath.nextBridge.ClampPoint (middlePos);
			endPos = bridgePath.nextBridge.WorldPointFromNode (bridgePath.nextBridge.NodeFromWorldPoint (endPos), touchGroundHeight);
			BridgePath.PathSegment secondSegment = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, () => {
				StopMoveEvent ();
				SetLevelParams (bridgePath.nextBridge);
				currentBridge = bridgePath.nextBridge;
			}, 0.1f);
			Vector3[] secondPath = new Vector3[2];
			secondPath [0] = middlePos;
			secondPath [1] = endPos;
			secondSegment.SetPath (secondPath);
		}

		protected override void SetupJumpUpBridgePath (BridgePath bridgePath)
		{
			Vector3 middlePos = GameUtils.GetConnectBridgeMiddlePos (bridgePath.bridge, bridgePath.nextBridge, this.touchGroundHeight);
			Vector3 endPos = bridgePath.nextBridge.ProjectToPlanePoint (middlePos, this.touchGroundHeight);
			BridgePath.PathSegment segment = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, () => {
				StopMoveEvent ();
				SetLevelParams (bridgePath.nextBridge);
				currentBridge = bridgePath.nextBridge;
			});
			PathRequestManager.RequestPath (bridgePath.bridge, Position, endPos, (path, success) => {
				if (success)
				{
					segment.SetPath (path);
				}
				else
				{
					UnityLog.LogError ("SetupJumpUpBridgePath request path failed! " + Position + " -> " + endPos);
				}
			}, this.touchGroundHeight);
		}

		protected override void SetupExceedBridgePath (BridgePath bridgePath)
		{
			Vector3 middlePos = GameUtils.GetConnectBridgeMiddlePos (bridgePath.bridge, bridgePath.nextBridge, this.touchGroundHeight);
			Vector3 endPos = bridgePath.nextBridge.ClampPoint (middlePos);
			endPos = bridgePath.nextBridge.WorldPointFromNode (bridgePath.nextBridge.NodeFromWorldPoint (endPos), this.touchGroundHeight);
			Vector3 edgePoint = middlePos + Vector3.Dot (endPos - middlePos, bridgePath.nextBridge.Up) * bridgePath.nextBridge.Up;

			BridgePath.PathSegment firstSeg = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, () => {
				StopMoveEvent ();
				SetLevelParams (bridgePath.nextBridge);
				currentBridge = bridgePath.nextBridge;
			});
			PathRequestManager.RequestPath (bridgePath.bridge, Position, middlePos, (path, success) => {
				if (success)
				{
					Vector3[] newPath = new Vector3[path.Length + 1];
					Array.Copy (path, newPath, path.Length);
					newPath [newPath.Length - 1] = edgePoint;
					firstSeg.SetPath (newPath);
				}
				else
				{
					UnityLog.LogError ("SetupExceedBridgePath request path failed! " + Position + " -> " + middlePos);
				}
			}, this.touchGroundHeight);

			BridgePath.PathSegment secondSeg = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, OnPathReachDestination);
			Vector3[] secondPath = new Vector3[2];
			secondPath [0] = edgePoint;
			secondPath [1] = endPos;
			secondSeg.SetPath (secondPath);
		}

		protected override void SetupTurnExceedBridgePath (BridgePath bridgePath)
		{
			Vector3 middlePos = GameUtils.GetConnectBridgeMiddlePos (bridgePath.bridge, bridgePath.nextBridge, this.touchGroundHeight);
			Vector3 endPos = bridgePath.nextBridge.ClampPoint (middlePos);
			endPos = bridgePath.nextBridge.WorldPointFromNode (bridgePath.nextBridge.NodeFromWorldPoint (endPos), this.touchGroundHeight);
			Vector3 edgePoint = middlePos + Vector3.Dot (endPos - middlePos, bridgePath.nextBridge.Up) * bridgePath.nextBridge.Up;

			BridgePath.PathSegment firstSeg = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, () => {
				StopMoveEvent ();
				SetLevelParams (bridgePath.nextBridge);
				currentBridge = bridgePath.nextBridge;
			});
			PathRequestManager.RequestPath (bridgePath.bridge, this.Position, middlePos, (path, success) => {
				if (success)
				{
					Vector3[] newPath = new Vector3[path.Length + 1];
					Array.Copy (path, newPath, path.Length);
					newPath [newPath.Length - 1] = edgePoint;
					firstSeg.SetPath (newPath);
				}
				else
				{
					UnityLog.LogError ("SetupTurnExceedBridgePath request path failed! " + Position + " -> " + middlePos);
				}
			}, this.touchGroundHeight);

			BridgePath.PathSegment secondSeg = bridgePath.CreatePathSegment (OnPathStart, OnPathReachMiddlePoint, OnPathReachDestination);
			Vector3[] secondPath = new Vector3[2];
			secondPath [0] = edgePoint;
			secondPath [1] = endPos;
			secondSeg.SetPath (secondPath);
		}
	}
}

