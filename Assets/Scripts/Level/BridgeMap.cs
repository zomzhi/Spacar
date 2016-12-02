using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MyCompany.Common;

namespace MyCompany.MyGame.Level
{
	public enum EMapSign
	{
		// 是否可放置物体
		Placeable = 1 << 0,
		// 是否放置了物体
		Obstacle = 1 << 1,
		// 是否会断裂
		Breakable = 1 << 2,
		// 主路径点
		MainPath = 1 << 3,
		// 延伸路径点
		ExtendPath = 1 << 4,
	}


	public struct Coordinate : IComparable
	{
		public int x;
		public int y;

		public Coordinate (int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public static bool operator == (Coordinate lhs, Coordinate rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}

		public static bool operator != (Coordinate lhs, Coordinate rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString ()
		{
			return string.Format ("[{0}, {1}]", x, y);
		}

		public int CompareTo (object other)
		{
			int result = 0;
			Coordinate coord = (Coordinate)other;
			if (y == coord.y)
				result = x - coord.x;
			else
				result = y - coord.y;
			return result;
		}
	}

	public class BridgeMap
	{
		public class Node : IHeapItem<Node>
		{
			/// <summary>
			/// 节点坐标
			/// </summary>
			public Coordinate coord;

			/// <summary>
			/// 寻路中到起始节点的代价
			/// </summary>
			public int gCost;

			/// <summary>
			/// 寻路中到终点节点的代价
			/// </summary>
			public int hCost;

			/// <summary>
			/// 父节点（寻路中使用）
			/// </summary>
			public Node parent;

			/// <summary>
			/// 地图标志
			/// </summary>
			public sbyte sign;

			/// <summary>
			/// 节点代价
			/// </summary>
			/// <value>The f cost.</value>
			public int fCost{ get { return gCost + hCost; } }

			/// <summary>
			/// 节点在堆中的索引
			/// </summary>
			/// <value>The index of the heap.</value>
			public int HeapIndex{ get; set; }

			/// <summary>
			/// 地形代价
			/// </summary>
			/// <value>The penalty.</value>
			public int Penalty{ get; set; }

			public Node (int x, int y)
			{
				coord = new Coordinate (x, y);
				Penalty = -1;
			}

			public Node (Coordinate _coord)
			{
				coord = _coord;
				Penalty = -1;
			}

			#region Sign Op

			public bool IsPlaceable ()
			{
				return ((sign & (sbyte)EMapSign.Placeable) > 0 && !IsObstacle ());
			}

			public bool IsBreakable ()
			{
				return (sign & (sbyte)EMapSign.Breakable) > 0;
			}

			public bool IsMainPath ()
			{
				return (sign & (sbyte)EMapSign.MainPath) > 0;
			}

			public bool IsExtendPath ()
			{
				return (sign & (sbyte)EMapSign.ExtendPath) > 0;
			}

			public bool IsObstacle ()
			{
				return (sign & (sbyte)EMapSign.Obstacle) > 0;
			}

			public bool IsWalkable ()
			{
				return !IsObstacle () && !IsBreakable ();
			}

			#endregion


			public int CompareTo (Node nodeToCompare)
			{
				int compare = fCost.CompareTo (nodeToCompare.fCost);
				if (compare == 0)
				{
					compare = hCost.CompareTo (nodeToCompare.hCost);
				}
				return -compare;
			}
		}

		public int Width{ get; private set; }

		public int Height{ get; private set; }

		public int GridCount{ get { return Width * Height; } }

		//		sbyte[,] map;
		Node[,] mapGrid;
		List<Coordinate> bridgePath;

		public BridgeMap (int _width, int _height)
		{
			Width = _width;
			Height = _height;

//			map = new sbyte[Width, Height];
			mapGrid = new Node[Width, Height];
			for (int i = 0; i < Width; i++)
				for (int j = 0; j < Height; j++)
					mapGrid [i, j] = new Node (i, j);
			bridgePath = new List<Coordinate> ();
		}

		#region Sign operation

		public sbyte GetValue (Coordinate coord)
		{
			return GetValue (coord.x, coord.y);
		}

		public sbyte GetValue (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
//				return map [x, y];
				return mapGrid [x, y].sign;
			return sbyte.MinValue;
		}

		public void MarkBreakable (Coordinate coord)
		{
			MarkBreakable (coord.x, coord.y);
		}

		public void MarkBreakable (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
//				map [x, y] |= (sbyte)EMapSign.Breakable;
				mapGrid [x, y].sign |= (sbyte)EMapSign.Breakable;
		}

		public void MarkPlaceable (Coordinate coord)
		{
			MarkPlaceable (coord.x, coord.y);
		}

		public void MarkPlaceable (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
//				map [x, y] |= (sbyte)EMapSign.Placeable;
				mapGrid [x, y].sign |= (sbyte)EMapSign.Placeable;
		}

		public void MarkObstacle (Coordinate coord)
		{
			MarkObstacle (coord.x, coord.y);
		}

		public void MarkObstacle (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
			{
//				map [x, y] |= (sbyte)EMapSign.Obstacle;
				mapGrid [x, y].sign |= (sbyte)EMapSign.Obstacle;
			}
		}

		public void MarkNotObstacle (Coordinate coord)
		{
			MarkNotObstacle (coord.x, coord.y);
		}

		public void MarkNotObstacle (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
			{
//				map [x, y] &= ~((sbyte)EMapSign.Obstacle);
				mapGrid [x, y].sign &= ~((sbyte)EMapSign.Obstacle);
			}
		}

		public void MarkMainPath (Coordinate coord)
		{
			MarkMainPath (coord.x, coord.y);
		}

		public void MarkMainPath (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
			{
//				map [x, y] |= (sbyte)EMapSign.MainPath;
//				map [x, y] &= ~((sbyte)EMapSign.Placeable);
				mapGrid [x, y].sign |= (sbyte)EMapSign.MainPath;
				mapGrid [x, y].sign &= ~((sbyte)EMapSign.Placeable);
			}
		}

		public void MarkExtendPath (Coordinate coord)
		{
			MarkExtendPath (coord.x, coord.y);
		}

		public void MarkExtendPath (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
			{
//				map [x, y] |= (sbyte)EMapSign.ExtendPath;
//				map [x, y] &= ~((sbyte)EMapSign.MainPath);
//				map [x, y] &= ~((sbyte)EMapSign.Placeable);
				mapGrid [x, y].sign |= (sbyte)EMapSign.ExtendPath;
				mapGrid [x, y].sign &= ~((sbyte)EMapSign.MainPath);
				mapGrid [x, y].sign &= ~((sbyte)EMapSign.Placeable);
			}
		}

		public bool ValidX (int x)
		{
			return x >= 0 && x < Width;
		}

		public bool ValidY (int y)
		{
			return y >= 0 && y < Height;
		}

		public bool ValidCoordinate (Coordinate coord)
		{
			return ValidX (coord.x) && ValidY (coord.y);
		}

		public bool IsPlaceable (Coordinate coord)
		{
			return IsPlaceable (coord.x, coord.y);
		}

		public bool IsPlaceable (int x, int y)
		{
			if (!ValidX (x) || !ValidY (y))
				return false;
//			return ((map [x, y] & (sbyte)EMapSign.Placeable) > 0 && !IsObstacle (x, y));
//			return ((mapGrid [x, y].sign & (sbyte)EMapSign.Placeable) > 0 && !IsObstacle (x, y));
			return mapGrid [x, y].IsPlaceable ();
		}

		public bool IsBreakable (Coordinate coord)
		{
			return IsBreakable (coord.x, coord.y);
		}

		public bool IsBreakable (int x, int y)
		{
			if (!ValidX (x) || !ValidY (y))
				return false;
//			return (map [x, y] & (sbyte)EMapSign.Breakable) > 0;
//			return (mapGrid [x, y].sign & (sbyte)EMapSign.Breakable) > 0;
			return mapGrid [x, y].IsBreakable ();
		}

		public bool IsMainPath (Coordinate coord)
		{
			return IsMainPath (coord.x, coord.y);
		}

		public bool IsMainPath (int x, int y)
		{
			if (!ValidX (x) || !ValidY (y))
				return false;
//			return (map [x, y] & (sbyte)EMapSign.MainPath) > 0;
//			return (mapGrid [x, y].sign & (sbyte)EMapSign.MainPath) > 0;
			return mapGrid [x, y].IsMainPath ();
		}

		public bool IsExtendPath (Coordinate coord)
		{
			return IsExtendPath (coord.x, coord.y);
		}

		public bool IsExtendPath (int x, int y)
		{
			if (!ValidX (x) || !ValidY (y))
				return false;
//			return (map [x, y] & (sbyte)EMapSign.ExtendPath) > 0;
//			return (mapGrid [x, y].sign & (sbyte)EMapSign.ExtendPath) > 0;
			return mapGrid [x, y].IsExtendPath ();
		}

		public bool IsObstacle (Coordinate coord)
		{
			return IsObstacle (coord.x, coord.y);
		}

		public bool IsObstacle (int x, int y)
		{
			if (!ValidX (x) || !ValidY (y))
				return false;
//			return (map [x, y] & (sbyte)EMapSign.Obstacle) > 0;
//			return (mapGrid [x, y].sign & (sbyte)EMapSign.Obstacle) > 0;
			return mapGrid [x, y].IsObstacle ();
		}

		public bool IsWalkable (Coordinate coord)
		{
			return IsWalkable (coord.x, coord.y);
		}

		public bool IsWalkable (int x, int y)
		{
			if (!ValidX (x) || !ValidY (y))
				return false;
			return mapGrid [x, y].IsWalkable ();
		}

		#endregion

		#region Path Utils

		public void GeneratePathBetweenPoints (Coordinate start, Coordinate end)
		{
			if (!ValidCoordinate (start) || !ValidCoordinate (end))
			{
				UnityLog.LogError ("Path generation failed! coordinate invalid: " + start + ", " + end);
				return;
			}

			List<Coordinate> path = GetPathBetweenPoints (start, end);
			bridgePath.AddRange (path);

			string pathString = "Path: ";
			foreach (Coordinate coord in bridgePath)
			{
				pathString += coord + ", ";
			}
			UnityLog.Log (pathString);
		}

		public void PathToLeft (Coordinate coord)
		{
			if (ValidCoordinate (coord) && !IsBreakable (coord))
			{
				int x = coord.x;
				int y = coord.y;
				while (ValidX (x - 1) && !IsBreakable (x - 1, y))
				{
					x--;
					bridgePath.Add (new Coordinate (x, y));
				}
			}
		}

		public void PathToRight (Coordinate coord)
		{
			if (ValidCoordinate (coord) && !IsBreakable (coord))
			{
				int x = coord.x;
				int y = coord.y;
				while (ValidX (x + 1) && !IsBreakable (x + 1, y))
				{
					x++;
					bridgePath.Add (new Coordinate (x, y));
				}
			}
		}

		public void PathStraightUp (Coordinate coord)
		{
			if (ValidCoordinate (coord) && !IsBreakable (coord))
			{
				int x = coord.x;
				int y = coord.y;
				while (ValidY (y + 1))
				{
					y++;
					bridgePath.Add (new Coordinate (x, y));
				}
			}
		}

		public void PathToBottom (Coordinate coord)
		{
			if (ValidCoordinate (coord) && !IsBreakable (coord))
			{
				int x = coord.x;
				int y = coord.y;
				while (ValidY (y - 1))
				{
					y--;
					bridgePath.Add (new Coordinate (x, y));
				}
			}
		}

		public void MarkPath ()
		{
			foreach (Coordinate coord in bridgePath)
			{
				MarkMainPath (coord);
			}

			// Extend path
			foreach (Coordinate coord in bridgePath)
			{
				for (int i = -GameDefine.PATH_WIDTH; i <= GameDefine.PATH_WIDTH; i++)
				{
					for (int j = -GameDefine.PATH_WIDTH; j <= GameDefine.PATH_WIDTH; j++)
					{
						int x = coord.x + i;
						int y = coord.y + j;

						if (!(x == coord.x && y == coord.y) && !IsBreakable (x, y) && !IsMainPath (x, y))
						{
							MarkExtendPath (x, y);
						}
					}
				}
			}
		}

		public void FindConnectedPath (ref List<Coordinate> path, Coordinate start, Coordinate end)
		{
			if (end.y <= start.y + 1)
				return;

			int deltaY = end.y - start.y;
			int startXMaxReach = start.x + deltaY;

			if (startXMaxReach < end.x)
				return;

			int midX = -1, midXMin, midXMax;
			int midY = (end.y + start.y) / 2;
			int startXMin = Mathf.Max (0, start.x - (midY - start.y));
			int startXMax = Mathf.Min (Width - 1, start.x + (midY - start.y));
			int endXMin = Mathf.Max (0, end.x - (end.y - midY));
			int endXMax = Mathf.Min (Width - 1, end.x + (end.y - midY));

			if (startXMin <= endXMin && startXMax >= endXMin)
			{
				midXMin = endXMin;
				midXMax = Mathf.Min (startXMax, endXMax);

				// 找到了公共区域，再去掉不可通行部分
				while (IsBreakable (midXMin, midY) && midXMin < midXMax)
				{
					midXMin++;
				}
				while (IsBreakable (midXMax, midY) && midXMax > midXMin)
				{
					midXMax--;
				}

				midX = UnityEngine.Random.Range (midXMin, midXMax + 1);
				//				print ("Range1: " + startXMin + ", " + startXMax + ", " + endXMin + ", " + endXMax);
			}
			else if (startXMin > endXMin && startXMin <= endXMax)
			{
				midXMin = startXMin;
				midXMax = Mathf.Min (startXMax, endXMax);

				// 找到了公共区域，再去掉不可通行部分
				while (IsBreakable (midXMin, midY) && midXMin < midXMax)
				{
					midXMin++;
				}
				while (IsBreakable (midXMax, midY) && midXMax > midXMin)
				{
					midXMax--;
				}

				midX = UnityEngine.Random.Range (midXMin, midXMax + 1);
				//				print ("Range2: " + startXMin + ", " + startXMax + ", " + endXMin + ", " + endXMax);
			}

			if (midX >= 0)
			{
				// 找到了中点
				Coordinate midCoord = new Coordinate (midX, midY);
				path.Add (midCoord);
				//				print ("Get mid point:" + start + ", " + end + " => " + midCoord);

				if (Mathf.Abs (midY - start.y) == Mathf.Abs (midX - start.x))
				{
					// 中点与起点连成一条斜线
					int xStep = midX > start.x ? -1 : 1;
					for (int i = 1; i < midY - start.y; i++)
					{
						Coordinate lineCoord = new Coordinate (midX + xStep * i, midY - i);
						path.Add (lineCoord);
						//						print ("Line coord: " + lineCoord);
					}
				}
				else
					FindConnectedPath (ref path, start, midCoord);

				if (Mathf.Abs (end.y - midY) == Mathf.Abs (midX - end.x))
				{
					// 中点与终点连成一条斜线
					int xStep = midX > end.x ? -1 : 1;
					for (int i = 1; i < end.y - midY; i++)
					{
						Coordinate lineCoord = new Coordinate (midX + xStep * i, midY + i);
						path.Add (lineCoord);
						//						print ("Line coord: " + lineCoord);
					}
				}
				else
					FindConnectedPath (ref path, midCoord, end);
			}
		}

		public Node GetCorrespondNode (float h, float v)
		{
			Node result = null;
			if (h >= 0 && h <= Width && v >= 0 && v <= Height)
			{
				int x = Mathf.FloorToInt (h);
				int y = Mathf.FloorToInt (v);
				result = mapGrid [x, y];
			}
			return result;
		}

		/// <summary>
		/// 返回节点的周围节点列表
		/// </summary>
		/// <returns>The neighbours.</returns>
		/// <param name="node">Node.</param>
		public List<Node> GetNeighbours (Node node)
		{
			List<Node> neighbours = new List<Node> ();
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i == 0 && j == 0)
						continue;
					int x = node.coord.x + i;
					int y = node.coord.y + j;
					if (ValidX (x) && ValidY (y))
						neighbours.Add (mapGrid [x, y]);
				}
			}
			return neighbours;
		}

		/// <summary>
		/// 得到节点的地形代价
		/// </summary>
		/// <returns>The penalty.</returns>
		/// <param name="node">Node.</param>
		public int GetPenalty (Node node)
		{
			if (node.Penalty >= 0)
				return node.Penalty;

			return UpdatePenalty (node);
		}

		public int GetUnmodifiedPenalty (int x, int y)
		{
			return mapGrid [x, y].Penalty;
		}

		/// <summary>
		/// 更新节点地形代价
		/// </summary>
		/// <returns>The penalty.</returns>
		/// <param name="node">Node.</param>
		public int UpdatePenalty (Node node)
		{

			// four corner version
//			Coordinate leftCoord = new Coordinate (node.coord.x - 1, node.coord.y);
//			Coordinate upCoord = new Coordinate (node.coord.x, node.coord.y + 1);
//			Coordinate rightCoord = new Coordinate (node.coord.x + 1, node.coord.y);
//			Coordinate bottomCoord = new Coordinate (node.coord.x, node.coord.y - 1);
//			if (ValidCoordinate (leftCoord) && mapGrid [leftCoord.x, leftCoord.y].IsWalkable () &&
//			    ValidCoordinate (upCoord) && mapGrid [upCoord.x, upCoord.y].IsWalkable () &&
//			    ValidCoordinate (rightCoord) && mapGrid [rightCoord.x, rightCoord.y].IsWalkable () &&
//			    ValidCoordinate (bottomCoord) && mapGrid [bottomCoord.x, bottomCoord.y].IsWalkable ())
//			{
//				node.Penalty = 0;
//			}
//			else
//				node.Penalty = GameDefine.NOT_WALKABLE_PENALTY;

			// eight corner version
			bool ret = true;
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i == 0 && j == 0)
						continue;
					int checkX = node.coord.x + i;
					int checkY = node.coord.y + j;
					ret &= (ValidX (checkX) && ValidY (checkY) && mapGrid [checkX, checkY].IsWalkable ());
					if (!ret)
						break;
				}
			}
			node.Penalty = ret ? 0 : GameDefine.NOT_WALKABLE_PENALTY;

			return node.Penalty;
		}

		/// <summary>
		/// 对角线节点是否可通行
		/// </summary>
		/// <returns><c>true</c>, if walkable was diagonaled, <c>false</c> otherwise.</returns>
		/// <param name="nodeA">Node a.</param>
		/// <param name="nodeB">Node b.</param>
		public bool DiagonalWalkable (Node nodeA, Node nodeB)
		{
			if (Mathf.Abs (nodeA.coord.x - nodeB.coord.x) == 1 && Mathf.Abs (nodeA.coord.y - nodeB.coord.y) == 1)
			{
				// 是对角线节点
				if (!mapGrid [nodeA.coord.x, nodeB.coord.y].IsWalkable () || !mapGrid [nodeB.coord.x, nodeA.coord.y].IsWalkable ())
					return false;
			}
			return true;
		}

		private List<Node> connectableNodes = null;

		/// <summary>
		/// 返回Bridge连接处能作为路径点的节点
		/// </summary>
		/// <returns>The connectable nodes.</returns>
		public List<Node> GetConnectableNodes (LevelBlock firstBlock)
		{
			// caculate only once
			if (connectableNodes == null)
			{
				Coordinate firstCoord = bridgePath [0];
				if (firstBlock.ValidNormalCoordinate (firstCoord.x, firstCoord.y))
				{
					connectableNodes = new List<Node> ();

					// 找到路径点可达的区域
					List<Node> reachableArea = new List<Node> ();
					List<Node> checkNodes = new List<Node> ();
					checkNodes.Add (mapGrid [firstCoord.x, firstCoord.y]);
					while (checkNodes.Count > 0)
					{
						Node node = checkNodes [0];
						checkNodes.Remove (node);
						reachableArea.Add (node);

						// check neighbour
						for (int x = -1; x <= 1; x++)
						{
							for (int y = -1; y <= 1; y++)
							{
								int newX = node.coord.x + x;
								int newY = node.coord.y + y;
								if (firstBlock.ValidNormalCoordinate (newX, newY) && !checkNodes.Contains (mapGrid [newX, newY]) && !reachableArea.Contains (mapGrid [newX, newY]) && mapGrid [newX, newY].IsWalkable ())
								{
									checkNodes.Add (mapGrid [newX, newY]);
								}
							}
						}
					}
					// 设置固定y的可达节点列表
					int startY = firstBlock.bridgeBelong.Forward == Vector3.up ? GameDefine.BLOCK_TALL : 0;
					for (int startX = firstBlock.normalStartIndex; startX <= firstBlock.normalEndIndex; startX++)
					{
						if (reachableArea.Contains (mapGrid [startX, startY]))
						{
							connectableNodes.Add (mapGrid [startX, startY]);
						}
					}
				}
			}

			return connectableNodes;
		}

		private List<Node> lastBlockReachableNodes;

		/// <summary>
		/// 检测某个末尾Block的node节点是否是可达的
		/// </summary>
		/// <returns><c>true</c> if this instance is reachable node the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		public bool IsReachableCoord (int coordx, int coordy, LevelBlock lastBlock)
		{
			if (lastBlockReachableNodes == null)
			{
				Coordinate lastCoord = bridgePath [bridgePath.Count - 1];
				int checkStartX = lastBlock.normalStartIndex;
				int checkEndX = lastBlock.normalEndIndex;
				int checkY = lastBlock.bridgeBelong.height - lastBlock.height - 1;

				if (!lastBlock.ValidNormalCoordinate (lastCoord.x, lastCoord.y - (lastBlock.bridgeBelong.height - lastBlock.height)))
				{
					UnityLog.LogError ("Last path coordinate is not belong last block. pls check. " + lastCoord.ToString ());
					return false;
				}

				// 找到路径点最后一个节点的所有在lastBlock范围内的可达节点
				lastBlockReachableNodes = new List<Node> ();
				List<Node> checkNodes = new List<Node> ();
				checkNodes.Add (mapGrid [lastCoord.x, lastCoord.y]);
				while (checkNodes.Count > 0)
				{
					Node checkNode = checkNodes [0];
					lastBlockReachableNodes.Add (checkNode);
					checkNodes.Remove (checkNode);

					for (int x = -1; x <= 1; x++)
					{
						for (int y = -1; y <= 1; y++)
						{
							int newX = checkNode.coord.x + x;
							int newY = checkNode.coord.y + y;
							if (newX >= checkStartX && newX <= checkEndX && newY > checkY && newY < Height)
							{
								if (!checkNodes.Contains (mapGrid [newX, newY]) && !lastBlockReachableNodes.Contains (mapGrid [newX, newY]) && mapGrid [newX, newY].IsWalkable ())
									checkNodes.Add (mapGrid [newX, newY]);
							}
						}
					}
				}
			}

			return lastBlockReachableNodes.Contains (mapGrid [coordx, coordy]);
		}

		#endregion

		#region Obstacle Utils

		/// <summary>
		/// 根据填充率生成障碍
		/// </summary>
		/// <returns>The obstacle coords.</returns>
		/// <param name="fillPercent">Fill percent.</param>
		public List<Coordinate> GenerateObstacleCoords (int fillPercent, bool isUpBridge = false)
		{
			List<Coordinate> obstacleCoords = new List<Coordinate> ();
			fillPercent = Mathf.Clamp (fillPercent, 0, 100);
			int startY = isUpBridge ? 10 : 0;		// TODO: magic number
			// TODO: Should we fill placeable coordinates instead of the whole bridge?
			for (int x = 0; x < Width; x++)
			{
				for (int y = startY; y < Height; y++)
				{
					ResetGridNode (x, y);
					if (IsPlaceable (x, y))
					{
						int rand = UnityEngine.Random.Range (0, 100);
						if (rand < fillPercent)
						{
							obstacleCoords.Add (new Coordinate (x, y));
						}
					}
				}
			}
			return obstacleCoords;
		}

		/// <summary>
		/// 检查区域是否孤立，即周围是否与障碍相连
		/// </summary>
		/// <returns><c>true</c> if this instance is isolate area the specified coord width height; otherwise, <c>false</c>.</returns>
		/// <param name="coord">Coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public bool IsIsolateArea (Coordinate coord, int width, int height)
		{
			return IsIsolateArea (coord.x, coord.y, width, height);
		}

		/// <summary>
		/// 检查区域是否孤立，即周围是否与障碍相连
		/// </summary>
		/// <returns><c>true</c> if this instance is isolate area the specified coordX coordY width height; otherwise, <c>false</c>.</returns>
		/// <param name="coordX">Coordinate x.</param>
		/// <param name="coordY">Coordinate y.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public bool IsIsolateArea (int coordX, int coordY, int width, int height)
		{
			for (int i = 0; i < width; i++)
			{
				int x = coordX + i;
				int upY = coordY + height;
				int downY = coordY - 1;
				if (ValidX (x) && ValidY (upY) && IsObstacle (x, upY))
					return false;
				if (ValidX (x) && ValidY (downY) && IsObstacle (x, downY))
					return false;
			}

			for (int j = 0; j < height; j++)
			{
				int leftX = coordX - 1;
				int rightX = coordX + width;
				int y = coordY + j;
				if (ValidX (leftX) && ValidY (y) && IsObstacle (leftX, y))
					return false;
				if (ValidX (rightX) && ValidY (y) && IsObstacle (rightX, y))
					return false;
			}
			return true;
		}


		/// <summary>
		/// 根据坐标点和
		/// </summary>
		/// <returns>The coordinate area.</returns>
		/// <param name="coord">Coordinate.</param>
		/// <param name="maxWidth">Max width.</param>
		/// <param name="maxHeight">Max height.</param>
		public Dictionary<int, int> GetCoordinateArea (Coordinate coord, int maxWidth, int maxHeight)
		{
			Dictionary<int, int > area = new Dictionary<int, int> ();
			if (ValidCoordinate (coord))
			{
				int x = coord.x;
				int y = coord.y;
				int xStep = 0, yStep;
				int yMax = coord.y + maxHeight - 1;

				while (IsPlaceable (x + xStep, y) && xStep < maxWidth)
				{
					yStep = 1;
					while (ValidY (y + yStep) && IsPlaceable (x + xStep, y + yStep) && ((y + yStep) <= yMax))
						yStep++;
					xStep++;
					yMax = y + yStep - 1;
					area.Add (xStep, yStep);
				}
			}
			return area;
		}

		public void MarkObstacleArea (Coordinate coord, int width, int height)
		{
			MarkObstacleArea (coord.x, coord.y, width, height);
		}

		public void MarkObstacleArea (int x, int y, int width, int height)
		{
			for (int i = x; i <= x + width - 1; i++)
			{
				for (int j = y; j <= y + height - 1; j++)
				{
					MarkObstacle (i, j);
				}
			}
		}

		public void MarkNotObstacleArea (Coordinate coord, int width, int height)
		{
			MarkNotObstacleArea (coord.x, coord.y, width, height);
		}

		public void MarkNotObstacleArea (int x, int y, int width, int height)
		{
			for (int i = x; i <= x + width - 1; i++)
			{
				for (int j = y; j <= y + height - 1; j++)
				{
					MarkNotObstacle (i, j);
				}
			}
		}

		public Coordinate GetFirstPathCoordinate ()
		{
			return bridgePath [0];
		}

		#endregion

		#region Private Methods

		private List<Coordinate> GetPathBetweenPoints (Coordinate start, Coordinate end)
		{
			List<Coordinate> path = new List<Coordinate> ();
			path.Add (start);
			path.Add (end);
			FindConnectedPath (ref path, start, end);
			return path;
		}

		private void ResetGridNode (int x, int y)
		{
			mapGrid [x, y].sign &= ~(sbyte)EMapSign.Obstacle;
			mapGrid [x, y].Penalty = -1;
		}

		#endregion

		#region Public Methods

		public Node GetNode (int x, int y)
		{
			if (ValidX (x) && ValidY (y))
				return mapGrid [x, y];
			return null;
		}

		#endregion
	}
}

