using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeLevelTest : MonoBehaviour
{
	public int pathWidth = 3;
	public float gridSize = 1.5f;
	public int width = 20;
	public int height = 80;

	[Range (0, 100)]
	public int randomFillPercent = 30;

	public Coordinate startCoord;
	public Coordinate endCoord;

	int[,] cubeMap;

	Material mat;

	Map map;

	[System.Serializable]
	public class Coordinate
	{
		public int xCoord;
		public int yCoord;

		public Coordinate (int x, int y)
		{
			xCoord = x;
			yCoord = y;
		}

		public override string ToString ()
		{
			return string.Format ("[{0}, {1}]", xCoord, yCoord);
		}
	}

	public class Map
	{
		public int width;
		public int height;

		int[,] map;

		public Map (int _width, int _height)
		{
			width = _width;
			height = _height;
			map = new int[width, height];
		}

		public void ResetMap ()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					map [x, y] = 0;
				}
			}
		}

		public void DrawGizmos ()
		{
			if (map != null)
			{
				float mapGridSize = 1.5f;
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						if (map [x, y] == 3)
							Gizmos.color = Color.red;
						else if (map [x, y] == 2)
							Gizmos.color = Color.green;
						else if (map [x, y] == 1)
							Gizmos.color = Color.black;
						else
							Gizmos.color = Color.white;
						Gizmos.DrawCube (new Vector3 (width * -0.5f + x * mapGridSize, 0f, height * -0.5f + y * mapGridSize), Vector3.one);
					}
				}
			}
		}

		public bool ValidCoordinate (Coordinate coord)
		{
			return ValidXCoord (coord.xCoord) && ValidYCoord (coord.yCoord);
		}

		public bool ValidXCoord (int x)
		{
			return x >= 0 && x < width;
		}

		public bool ValidYCoord (int y)
		{
			return y >= 0 && y < height;
		}

		public void GeneratePathBetweenPoints (Coordinate start, Coordinate end)
		{
			List<Coordinate> path = GetPathBetweenPoints (start, end);
			foreach (Coordinate coord in path)
			{
				map [coord.xCoord, coord.yCoord] = 3;
				if (ValidXCoord (coord.xCoord + 1) && map [coord.xCoord + 1, coord.yCoord] == 0)
					map [coord.xCoord + 1, coord.yCoord] = 2;
				if (ValidXCoord (coord.xCoord - 1) && map [coord.xCoord - 1, coord.yCoord] == 0)
					map [coord.xCoord - 1, coord.yCoord] = 2;
				if (ValidYCoord (coord.yCoord - 1) && map [coord.xCoord, coord.yCoord - 1] == 0)
					map [coord.xCoord, coord.yCoord - 1] = 2;
				if (ValidYCoord (coord.yCoord + 1) && map [coord.xCoord, coord.yCoord + 1] == 0)
					map [coord.xCoord, coord.yCoord + 1] = 2;

				Coordinate upLeft = new Coordinate (coord.xCoord - 1, coord.yCoord + 1);
				Coordinate upRight = new Coordinate (coord.xCoord + 1, coord.yCoord + 1);
				Coordinate bottomLeft = new Coordinate (coord.xCoord - 1, coord.yCoord - 1);
				Coordinate bottomRight = new Coordinate (coord.xCoord + 1, coord.yCoord - 1);
				if (ValidCoordinate (upLeft) && map [upLeft.xCoord, upLeft.yCoord] == 0)
					map [upLeft.xCoord, upLeft.yCoord] = 2;
				if (ValidCoordinate (upRight) && map [upRight.xCoord, upRight.yCoord] == 0)
					map [upRight.xCoord, upRight.yCoord] = 2;
				if (ValidCoordinate (bottomLeft) && map [bottomLeft.xCoord, bottomLeft.yCoord] == 0)
					map [bottomLeft.xCoord, bottomLeft.yCoord] = 2;
				if (ValidCoordinate (bottomRight) && map [bottomRight.xCoord, bottomRight.yCoord] == 0)
					map [bottomRight.xCoord, bottomRight.yCoord] = 2;
			}
		}

		public List<Coordinate> GetPathBetweenPoints (Coordinate start, Coordinate end)
		{
			List<Coordinate> path = new List<Coordinate> ();
			path.Add (start);
			path.Add (end);
			FindConnectPath (ref path, start, end);
			return path;
		}

		public void FillMap (int fillPercent)
		{
			System.Random pseudoRandom = new System.Random (Time.time.GetHashCode ());

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					// reset
					if (map [x, y] == 1)
						map [x, y] = 0;

					// random fill
					if (map [x, y] == 0 && pseudoRandom.Next (0, 100) < fillPercent)
					{
						map [x, y] = 1;
					}
				}
			}
		}

		public void SmoothFill ()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (map [x, y] == 0)
					{
						int fillNeighbor = GetNeightborCount (x, y, 1);
						if (fillNeighbor >= 4)
							map [x, y] = 1;
					}
					else if (map [x, y] == 1)
					{
						int fillNeighbor = GetNeightborCount (x, y, 1);
						if (fillNeighbor < 3)
							map [x, y] = 0;
					}
				}
			}
		}

		int GetNeightborCount (int x, int y, int neighborValue)
		{
			int count = 0;
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = y - 1; j <= y + 1; j++)
				{
					if ((i != x || j != y) && ValidXCoord (i) && ValidYCoord (j))
					{
						if (map [i, j] == neighborValue)
							count++;
					}						
				}
			}
			return count;
		}

		void FindConnectPath (ref List<Coordinate>path, Coordinate start, Coordinate end)
		{
			if (end.yCoord <= start.yCoord + 1)
				return;

			int deltaY = end.yCoord - start.yCoord;
			int startXMaxReach = start.xCoord + deltaY;

			if (startXMaxReach < end.xCoord)
				return;

			int midX = -1, midXMin, midXMax;
			int midY = (end.yCoord + start.yCoord) / 2;
			int startXMin = Mathf.Max (0, start.xCoord - (midY - start.yCoord));
			int startXMax = Mathf.Min (width - 1, start.xCoord + (midY - start.yCoord));
			int endXMin = Mathf.Max (0, end.xCoord - (end.yCoord - midY));
			int endXMax = Mathf.Min (width - 1, end.xCoord + (end.yCoord - midY));

			if (startXMin <= endXMin && startXMax >= endXMin)
			{
				midXMin = endXMin;
				midXMax = Mathf.Min (startXMax, endXMax);
				midX = Random.Range (midXMin, midXMax + 1);
//				print ("Range1: " + startXMin + ", " + startXMax + ", " + endXMin + ", " + endXMax);
			}
			else if (startXMin > endXMin && startXMin <= endXMax)
			{
				midXMin = startXMin;
				midXMax = Mathf.Min (startXMax, endXMax);
				midX = Random.Range (midXMin, midXMax + 1);
//				print ("Range2: " + startXMin + ", " + startXMax + ", " + endXMin + ", " + endXMax);
			}

			if (midX >= 0)
			{
				// 找到了中点
				Coordinate midCoord = new Coordinate (midX, midY);
				path.Add (midCoord);
//				print ("Get mid point:" + start + ", " + end + " => " + midCoord);

				if (Mathf.Abs (midY - start.yCoord) == Mathf.Abs (midX - start.xCoord))
				{
					// 中点与起点连成一条斜线
					int xStep = midX > start.xCoord ? -1 : 1;
					for (int i = 1; i < midY - start.yCoord; i++)
					{
						Coordinate lineCoord = new Coordinate (midX + xStep * i, midY - i);
						path.Add (lineCoord);
//						print ("Line coord: " + lineCoord);
					}
				}
				else
					FindConnectPath (ref path, start, midCoord);

				if (Mathf.Abs (end.yCoord - midY) == Mathf.Abs (midX - end.xCoord))
				{
					// 中点与终点连成一条斜线
					int xStep = midX > end.xCoord ? -1 : 1;
					for (int i = 1; i < end.yCoord - midY; i++)
					{
						Coordinate lineCoord = new Coordinate (midX + xStep * i, midY + i);
						path.Add (lineCoord);
//						print ("Line coord: " + lineCoord);
					}
				}
				else
					FindConnectPath (ref path, midCoord, end);
			}
		}
	}

	public class MapPath
	{
		public Map map;
		public List<PathSegment> path;

		public MapPath (Map _map)
		{
			map = _map;
		}
	}

	public abstract class PathSegment
	{
		public MapPath pathBelong;

		protected List<Coordinate> pathSegment;

		public PathSegment (MapPath path)
		{
			pathBelong = path;
		}


	}

	void Start ()
	{
		cubeMap = new int[width, height];
//		GenerateMap ();
//		GenerateCubeMap ();

		GenerateClassMap ();
	}

	void GenerateClassMap ()
	{
		map = new Map (width, height);
	}

	void GeneratePath (Coordinate start, Coordinate end)
	{
		map.ResetMap ();
		map.GeneratePathBetweenPoints (start, end);
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0))
		{
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000f))
			{
				MeshRenderer mr = hit.collider.GetComponent<MeshRenderer> ();
				Material mat = mr.material;
				if (mat.color == Color.white)
					mat.color = Color.black;
				else
					mat.color = Color.white;
			}
		}

		if (Input.GetKeyDown (KeyCode.Space))
		{
//			GeneratePath ();

			if (map != null)
			{
				GeneratePath (startCoord, endCoord);
			}
		}

		if (Input.GetKeyDown (KeyCode.F))
		{
			if (map != null)
			{
				map.FillMap (randomFillPercent);
			}
		}

		if (Input.GetKeyDown (KeyCode.S))
		{
			if (map != null)
			{
				map.SmoothFill ();
			}
		}
	}

	void GenerateMap ()
	{
		GeneratePath ();
	}

	void GenerateCubeMap ()
	{
		mat = new Material (Shader.Find ("Diffuse"));
		mat.color = Color.white;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				cube.transform.position = new Vector3 (width * -0.5f + x * gridSize, 0f, height * -0.5f + y * gridSize);
				cube.GetComponent<MeshRenderer> ().material = mat;
			}
		}
	}

	void ClearPath ()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				cubeMap [x, y] = 0;
			}
		}
	}

	void GeneratePath ()
	{
		ClearPath ();

		int curX, curY;
		int xStepMin, xStepMax, xStep;
		curX = width / 2;
		curY = 0;

		int destY = height - 5;

		cubeMap [curX, curY] = 2;
		while (curY < destY)
		{
			curY++;
			xStepMin = -1;
			xStepMax = 1;
			if (curX + 1 >= width - 1)
				xStepMax = 0;
			if (curX - 1 <= 0)
				xStepMin = 0;

			xStep = Random.Range (xStepMin, xStepMax + 1);
//			print (string.Format ("({0}, {1}) = {2}", xStepMin, xStepMax, xStep));

			curX += xStep;
			cubeMap [curX, curY] = 2;
		}
	}

	bool IsInMapRange (int x, int y)
	{
		return (x >= 0 && x < width && y >= 0 && y < height);
	}

	void OnDrawGizmos ()
	{
//		if (cubeMap != null)
//		{
//			for (int x = 0; x < width; x++)
//			{
//				for (int y = 0; y < height; y++)
//				{
//					Gizmos.color = (cubeMap [x, y] == 2) ? Color.black : Color.white;
//					Gizmos.DrawCube (new Vector3 (width * -0.5f + x * gridSize, 0f, height * -0.5f + y * gridSize), Vector3.one);
//				}
//			}
//		}

		if (map != null)
			map.DrawGizmos ();
	}
}
