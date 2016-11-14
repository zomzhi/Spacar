using System;
using System.Collections.Generic;
using UnityEngine;
using MyCompany.Common.Interface;
using MyCompany.MyGame.Obstacle;

namespace MyCompany.MyGame.Level
{
	public class LevelBridge : DichotomyListNode<LevelBridge>, IRectangleInt
	{


		public LevelBridge (ELevelType levelType, int blockCount, int bridgeWidth)
		{
			m_levelType = levelType;
			_blockCount = blockCount;
			m_width = bridgeWidth;
			dummy = false;
		}

		public LevelBridge (ELevelType levelType, int blockCount, int bridgeWidth, bool _dummy)
		{
			m_levelType = levelType;
			_blockCount = blockCount;
			m_width = bridgeWidth;
			this.dummy = _dummy;
		}

		/// <summary>
		/// Bridge对应的Map，Map用于标示路径、障碍等
		/// </summary>
		/// <value>The map.</value>
		public BridgeMap Map{ get; private set; }

		/// <summary>
		/// Bridge的第一块block
		/// </summary>
		/// <value>The first block.</value>
		public LevelBlock FirstBlock
		{ 
			get
			{ 
				if (blockList.Count > 0)
					return blockList [0];
				else
					return null;
			} 
		}

		/// <summary>
		/// Bridge的最后一块block
		/// </summary>
		/// <value>The last block.</value>
		public LevelBlock LastBlock
		{
			get
			{
				if (blockList.Count > 0)
					return blockList [blockList.Count - 1];
				else
					return null;
			}
		}

		// 障碍填充的比例
		public int fillPercent;

		// 小面积障碍过滤几率
		public int filterPercent;


		public bool dummy;

		#region Private member


		List<LevelBlock> blockList = new List<LevelBlock> ();

		private List<ObstacleBase> obstacleList = new List<ObstacleBase> ();

		private GameObject blockHolder;
		private GameObject obstaclesHolder;

		#endregion

		#region Public member


		#endregion

		#region Attribute

		private GameObject m_bridgeGo;
		public GameObject BridgeGo{ get { return m_bridgeGo; } set { m_bridgeGo = value; } }

		private ELevelType m_levelType;
		public ELevelType LevelType{ get { return m_levelType; } }

		private int m_width;
		public int width{ get { return m_width; } }

		//		public int height{ get { return BlockCount * (int)GameDefine.BLOCK_HEIGHT_SPEC; } }
		public int height{ get; private set; }

		private int _blockCount;
		public int BlockCount{ get { return _blockCount; } }

		public Vector3 leftBottom
		{ 
			get
			{ 
				Vector3 pos = Vector3.zero;
				if (m_bridgeGo != null)
					pos = m_bridgeGo.transform.position;
				return pos;
			} 
		}

		public Vector3 Forward{ get { return GameDefine.LEVEL_DIRECTION [(int)m_levelType] [0]; } }
		public Vector3 Right{ get { return GameDefine.LEVEL_DIRECTION [(int)m_levelType] [1]; } }
		public Vector3 Up{ get { return GameDefine.LEVEL_DIRECTION [(int)m_levelType] [2]; } }
		public Vector3 Down{ get { return -Up; } }

		#endregion


		#region Public Methods

		public void Build (LevelGenerator generator)
		{
			if (dummy)
			{
				height = BlockCount * (int)GameDefine.BLOCK_HEIGHT_SPEC;
				return;
			}

			if (m_bridgeGo == null)
			{
				// TODO: add to a parent holder
				m_bridgeGo = new GameObject ("LevelBridge");
				m_bridgeGo.transform.SetParent (generator.transform);
				if (GameDefine.debugMode)
				{
					LevelBridgeDebugComponent comp = m_bridgeGo.AddComponent<LevelBridgeDebugComponent> ();
					comp.levelBridge = this;
				}
				blockHolder = new GameObject ("BlockHolder");
				blockHolder.transform.SetParent (m_bridgeGo.transform);
				blockHolder.transform.localRotation = Quaternion.identity;
				blockHolder.transform.localPosition = Vector3.zero;

				obstaclesHolder = new GameObject ("ObstaclesHolder");
				obstaclesHolder.transform.SetParent (m_bridgeGo.transform);
				obstaclesHolder.transform.localRotation = Quaternion.identity;
				obstaclesHolder.transform.localPosition = Vector3.zero;

				if (prev == null)
				{
					m_bridgeGo.transform.position = GameDefine.DEFAULT_ORIGIN;
				}
				else
				{
					Vector3 offset = LevelBridge.GetConnectPositionOffset (prev, this);
					m_bridgeGo.transform.position = prev.leftBottom + offset;
				}
				BuildBlocks ();
				AdjustRotation ();
			}
		}

		public void SetupBridge ()
		{
			if (dummy || next == null)
				return;

			GeneratePath ();
			PlaceObstacles ();
		}

		/// <summary>
		/// 清除障碍
		/// </summary>
		public void ClearObstacles ()
		{
			foreach (ObstacleBase obstacle in obstacleList)
			{
				GameSystem.Instance.ObstaclesFactory.CollectObstacle (obstacle);
			}
			obstacleList.Clear ();
		}

		/// <summary>
		/// 放置障碍物
		/// </summary>
		public void PlaceObstacles ()
		{
			ClearObstacles ();
			ObstacleFactory obstacleFactory = GameSystem.Instance.ObstaclesFactory;

			List<Coordinate> obstacleCoords = Map.GenerateObstacleCoords (fillPercent);
			obstacleCoords.Sort ();

			foreach (Coordinate coord in obstacleCoords)
			{
				// 前面放置的物体可能会将后来的坐标覆盖，若该坐标已放置了物体则跳过
				if (!Map.IsPlaceable (coord))
					continue;

				Dictionary<int, int> area = Map.GetCoordinateArea (coord, obstacleFactory.ObstacleMaxWidth, obstacleFactory.ObstacleMaxHeight);
				ObstacleBase obstacle = obstacleFactory.GetRandomObstacle (area);
				if (obstacle == null)
				{
					string errStr = "Cannot find obstalce for coord: " + coord + " area: ";
					foreach (KeyValuePair<int, int> pair in area)
						errStr += "[" + pair.Key + ", " + pair.Value + "], ";
					UnityLog.LogError (errStr);
					continue;
				}

				obstacleList.Add (obstacle);
				obstacle.SetCoordinate (coord);

				Map.MarkObstacleArea (coord, obstacle.width, obstacle.height);
			}

			// 过滤掉面积较小且孤立的障碍
			List<ObstacleBase> removeObstacles = new List<ObstacleBase> ();
			foreach (ObstacleBase obstacle in obstacleList)
			{
				if (obstacle.AreaSize < GameDefine.FILTER_AREA_SIZE && Map.IsIsolateArea (obstacle.CoordX, obstacle.CoordY, obstacle.width, obstacle.height))
				{
					int rand = UnityEngine.Random.Range (0, 100);
					if (rand < filterPercent)
					{
						Map.MarkNotObstacleArea (obstacle.CoordX, obstacle.CoordY, obstacle.width, obstacle.height);
						removeObstacles.Add (obstacle);
					}
				}
			}
			foreach (ObstacleBase obstacle in removeObstacles)
			{
				obstacleList.Remove (obstacle);
				GameSystem.Instance.ObstaclesFactory.CollectObstacle (obstacle);
			}

			// 放置
			foreach (ObstacleBase obstacle in obstacleList)
			{
				obstacle.Trans.SetParent (obstaclesHolder.transform);
				obstacle.Trans.localRotation = Quaternion.identity;
				obstacle.Trans.position = leftBottom + Right * obstacle.CoordX + Forward * obstacle.CoordY + Up * GameDefine.BLOCK_TALL;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// 选取Block连在一起形成Bridge
		/// </summary>
		private void BuildBlocks ()
		{
			//TODO: 从Pool中索取Block而不是BlockFactory，根据游戏当前难度选取不等宽的Block

//			GameObject blockPrefab = GameSystem.Instance.BlockFactory.GetSpecificWidthRandomBlock (width);
//			if (blockPrefab == null)
//			{
//				UnityLog.LogError ("Can not get block prefab by width " + width);
//				return;
//			}


			int buildHeight = 0;
			for (int i = 0; i < BlockCount; i++)
			{
				Transform blockTrans = GameSystem.Instance.BlockFactory.GetBlockByWidth (width);
				if (blockTrans == null)
				{
					UnityLog.LogError ("Can not get block prefab by width " + width);
					return;
				}
				LevelBlock lb = blockTrans.GetComponent<LevelBlock> ();
				if (lb != null)
				{
					lb.bridgeBelong = this;
					blockTrans.SetParent (blockHolder.transform);
					blockTrans.localPosition = new Vector3 (buildHeight, 0f, 0f);
					blockTrans.localRotation = Quaternion.identity;
					blockTrans.localScale = Vector3.one;
					buildHeight += lb.height;
				}
				blockList.Add (lb);
			}
			height = buildHeight;

			GenerateBridgeMap ();
		}

		/// <summary>
		/// 生成Bridge的Map,Map可标示所有1x1地图点用于放置障碍和寻路
		/// </summary>
		private void GenerateBridgeMap ()
		{
			Map = new BridgeMap (width, height);
			int startY = 0;
			foreach (LevelBlock block in blockList)
			{
				for (int x = 0; x < width; x++)
				{
					for (int y = startY; y < startY + block.height; y++)
					{
						if (x < block.normalStartIndex || x > block.normalEndIndex)
						{
							Map.MarkBreakable (x, y);
							if (block.placeable)
								Map.MarkPlaceable (x, y);
						}
						else
							Map.MarkPlaceable (x, y);
					}
				}
				startY += block.height;
			}
		}

		private void GeneratePath ()
		{
			// 从bridge高度开始寻找path
			Coordinate startCoord = new Coordinate (FirstBlock.StartX, GameDefine.BLOCK_TALL);
			Map.PathToBottom (startCoord);
			int endX = -1, endY = -1;
			bool turnLeft = false, turnRight = false, straightUp = false;

			UnityLog.Log (string.Format ("======================{0} start generating path =======================", BridgeGo.name));

			// 确定终点坐标
			// 若下一个bridge与当前bridge处于同一平面，即左转或右转，则终点y坐标由
			// 下一bridge的起始x坐标决定，若当前bridge还与其他种类的桥相连则那种桥
			// 的起始x坐标决定当前终点x坐标，若无则可随机。
			if (next0 != null)
			{
				if (next0.Forward == -Right || next0.Up == -Right)
				{
					// 左转弯
					turnLeft = true;
					int nextStartX = next0.FirstBlock.StartX;
					endY = height - (next0.FirstBlock.width - nextStartX);
					UnityLog.Log ("next0 turn left");
				}
				else if (next0.Forward == Right || next0.Up == Right)
				{
					// 右转弯
					turnRight = true;
					int nextStartX = next0.FirstBlock.StartX;
					endY = height - 1 - nextStartX;
					UnityLog.Log ("next0 turn right");
				}
				else
				{
					straightUp = true;
					endX = next0.FirstBlock.StartX;
					UnityLog.Log ("next0 straight");
				}
			}

			if (next1 != null)
			{
				if (next1.Forward == -Right || next1.Up == -Right)
				{
					// 左转弯
					turnLeft = true;
					int nextStartX = next1.FirstBlock.StartX;
					endY = height - (next1.FirstBlock.width - nextStartX);
					UnityLog.Log ("next1 turn left");
				}
				else if (next1.Forward == Right || next1.Up == Right)
				{
					// 右转弯
					turnRight = true;
					int nextStartX = next1.FirstBlock.StartX;
					endY = height - 1 - nextStartX;
					UnityLog.Log ("next1 turn right");
				}
				else
				{
					straightUp = true;
					endX = next1.FirstBlock.StartX;
					UnityLog.Log ("next1 straight up");
				}
			}

			if (endX < 0)
//				endX = LastBlock.GetPathValidCoordX ();
				// TODO: random point near valid middle point
				endX = LastBlock.GetValidCenterCoordX ();

			if (endY < 0)
				endY = height - (LastBlock.height - LastBlock.CenterY);

			Coordinate endCoord = new Coordinate (endX, endY);
			UnityLog.Log ("Generate path between " + startCoord + " to " + endCoord);
			Map.GeneratePathBetweenPoints (startCoord, endCoord);

			if (turnLeft)
			{
				Map.PathToLeft (endCoord);
				UnityLog.Log ("Path to left");
			}
			else if (turnRight)
			{
				Map.PathToRight (endCoord);
				UnityLog.Log ("Path to right");
			}

			if (straightUp)
			{
				Map.PathStraightUp (endCoord);
				UnityLog.Log ("Path straightUp");
			}

			Map.MarkPath ();

			UnityLog.Log (string.Format ("======================{0} end generating path =======================", BridgeGo.name));
		}

		private void AdjustRotation ()
		{
			m_bridgeGo.transform.rotation = GameDefine.BLOCK_ROTATION [(int)LevelType];
		}

		#endregion

		#region Util Methods

		/// <summary>
		/// 根据到原点的高度得到对应的Block
		/// </summary>
		/// <returns>The block by height.</returns>
		/// <param name="heightFromOrigin">Height from origin.</param>
		public LevelBlock GetBlockByHeight (float heightFromOrigin)
		{
			if (dummy)
			{
				return null;
			}

			float totalHeight = 0f;
			heightFromOrigin = Mathf.Clamp (heightFromOrigin, 0f, height);
			for (int i = 0; i < blockList.Count; i++)
			{
				totalHeight += blockList [i].height;
				if (heightFromOrigin <= totalHeight)
					return blockList [i];
			}
			return null;
		}


		/// <summary>
		/// 检测某个点是否处于Bridge平面下方
		/// </summary>
		/// <returns><c>true</c>, if bridge was belowed, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		/// <param name="bridge">Bridge.</param>
		/// <param name="groundHeightOffset">bridge平面高度偏移值.</param>
		public bool BelowBridge (Vector3 point, float groundHeightOffset = 0f)
		{
			Vector3 bridgePlanePoint = this.leftBottom + this.Up * GameDefine.BLOCK_TALL;
			bridgePlanePoint += this.Up * groundHeightOffset;
			return Vector3.Dot (point, this.Up) < Vector3.Dot (bridgePlanePoint, this.Up);
		}

		/// <summary>
		/// 将坐标点投射在bridge平面
		/// </summary>
		/// <returns>The to plane point.</returns>
		/// <param name="point">Point.</param>
		/// <param name="groundHeightOffset">bridge平面高度偏移值</param>
		public Vector3 ProjectToPlanePoint (Vector3 point, float groundHeightOffset = 0f)
		{
			Vector3 bridgePlanePoint = this.leftBottom + this.Up * GameDefine.BLOCK_TALL;
			bridgePlanePoint += this.Up * groundHeightOffset;
			float offsetHeight = Vector3.Dot (bridgePlanePoint, this.Up) - Vector3.Dot (point, this.Up);
			return (point + this.Up * offsetHeight);
		}

		/// <summary>
		/// 坐标垂直方向是否超出了bridge
		/// </summary>
		/// <returns><c>true</c>, if bridge height was exceeded, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		public bool ExceedBridgeHeight (Vector3 point)
		{
			float bridgeOriginDot = Vector3.Dot (this.leftBottom, this.Forward);
			float pointDot = Vector3.Dot (point, this.Forward);
			return (pointDot - bridgeOriginDot) > this.height;
		}

		/// <summary>
		/// 坐标水平方向是否超出bridge
		/// </summary>
		/// <returns><c>true</c>, if bridge width was exceeded, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		public bool ExceedBridgeWidth (Vector3 point)
		{
			float hDistToOrigin = Vector3.Dot (point - leftBottom, Right);
			return (hDistToOrigin < 0 || hDistToOrigin > width);
		}

		/// <summary>
		/// 获得当前bridge原点距上一个bridge原点的偏移
		/// </summary>
		/// <returns>The connect position offset.</returns>
		/// <param name="prevBridge">Previous bridge.</param>
		/// <param name="curBridge">Current bridge.</param>
		public static Vector3 GetConnectPositionOffset (LevelBridge prevBridge, LevelBridge curBridge)
		{
			Vector3 offset = Vector3.zero;
			switch (prevBridge.LevelType)
			{
				case ELevelType.ALONG_X_FACE_Y:
					if (curBridge.LevelType == ELevelType.ALONG_Z_FACE_Y)
						offset.x = prevBridge.height - curBridge.width;
					else if (curBridge.LevelType == ELevelType.ALONG_Y_FACE_X)
						offset.x = prevBridge.height + GameDefine.BLOCK_TALL;
					else if (curBridge.LevelType == ELevelType.ALONG_X_FACE_Y)			// special case: first default bridge
						offset.x = prevBridge.height;
					else
						UnityLog.LogError ("Bridge type " + curBridge.LevelType + " is  not connected to " + prevBridge.LevelType);
				break;

				case ELevelType.ALONG_X_FACE_Z:
					if (curBridge.LevelType == ELevelType.ALONG_Y_FACE_Z)
						offset.x = prevBridge.height - curBridge.width;
					else if (curBridge.LevelType == ELevelType.ALONG_Z_FACE_Y)
					{
						offset.x = prevBridge.height - curBridge.width;
						offset.y = -GameDefine.BLOCK_TALL;
					}
					else
						UnityLog.LogError ("Bridge type " + curBridge.LevelType + " is  not connected to " + prevBridge.LevelType);
				break;

				case ELevelType.ALONG_Y_FACE_X:
					if (curBridge.LevelType == ELevelType.ALONG_X_FACE_Y)
						offset.y = prevBridge.height - GameDefine.BLOCK_TALL;
					else if (curBridge.LevelType == ELevelType.ALONG_Z_FACE_X)
						offset.y = prevBridge.height - curBridge.width;
					else
						UnityLog.LogError ("Bridge type " + curBridge.LevelType + " is  not connected to " + prevBridge.LevelType);
				break;

				case ELevelType.ALONG_Y_FACE_Z:
					if (curBridge.LevelType == ELevelType.ALONG_Z_FACE_Y)
						offset.y = prevBridge.height - GameDefine.BLOCK_TALL;
					else if (curBridge.LevelType == ELevelType.ALONG_X_FACE_Z)
					{
						offset.y = prevBridge.height;
						offset.x = prevBridge.width;
					}
					else
						UnityLog.LogError ("Bridge type " + curBridge.LevelType + " is  not connected to " + prevBridge.LevelType);
				break;

				case ELevelType.ALONG_Z_FACE_X:
					if (curBridge.LevelType == ELevelType.ALONG_Y_FACE_X)
					{
						offset.z = prevBridge.height;
						offset.y = prevBridge.width;
					}
					else if (curBridge.LevelType == ELevelType.ALONG_X_FACE_Y)
					{
						offset.z = prevBridge.height;
						offset.y = prevBridge.width - GameDefine.BLOCK_TALL;
					}
					else
						UnityLog.LogError ("Bridge type " + curBridge.LevelType + " is  not connected to " + prevBridge.LevelType);
				break;

				case ELevelType.ALONG_Z_FACE_Y:
					if (curBridge.LevelType == ELevelType.ALONG_X_FACE_Y)
					{
						offset.z = prevBridge.height;
						offset.x = prevBridge.width;
					}
					else if (curBridge.LevelType == ELevelType.ALONG_Y_FACE_Z)
						offset.z = prevBridge.height + GameDefine.BLOCK_TALL;
					else
						UnityLog.LogError ("Bridge type " + curBridge.LevelType + " is  not connected to " + prevBridge.LevelType);
				break;

				default:
					throw new ArgumentException ("Prev bridge type " + prevBridge.LevelType + " is not implemented. wtf?");
				break;
			}
			return offset;
		}

		public LevelBlock GetBlockByIndex (int index)
		{
			if (index < 0 || index >= blockList.Count)
				return null;

			return blockList [index];
		}

		public bool WithinLastBlock (Vector3 pos)
		{
			if (blockList == null || blockList.Count == 0)
				return false;

			LevelBlock lastBlock = blockList [blockList.Count - 1];
			Vector3 dir = pos - lastBlock.leftBottom;

			float h = Vector3.Dot (dir, Right);
			float v = Vector3.Dot (dir, Forward);

			if (h >= 0 && h <= lastBlock.width && v >= 0 && v <= lastBlock.height)
				return true;

			return false;
		}

		#endregion
	}
}

