using UnityEngine;
using System.Collections;

public class GridTest : MonoBehaviour
{

	public int width = 100;
	public int height = 50;
	public float gridSize = 1f;

	[Range (0, 100)]
	public int randomFillPercent;
	public bool useRandomSeed;
	public string seed;

	int[,] map;

	void Start ()
	{
		GenerateMap ();
	}


	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.S))
		{
			SmoothMap ();
		}

		if (Input.GetMouseButtonDown (0))
		{
			GenerateMap ();
		}
	}

	void GenerateMap ()
	{
		map = new int[width, height];
		if (useRandomSeed)
		{
			seed = Time.time.ToString ();
		}

		System.Random pseudoRandom = new System.Random (seed.GetHashCode ());

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				map [i, j] = (pseudoRandom.Next (0, 100) < randomFillPercent) ? 1 : 0;
			}
		}
	}

	void SmoothMap ()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int neighbourWallTiles = GetSurroundingWallCount (x, y);
				if (neighbourWallTiles > 4)
					map [x, y] = 1;
				else if (neighbourWallTiles < 4)
					map [x, y] = 0;
			}
		}
	}

	int GetSurroundingWallCount (int gridX, int gridY)
	{
		int wallCount = 0;
		for (int x = gridX - 1; x <= gridX + 1; x++)
		{
			for (int y = gridY - 1; y <= gridY + 1; y++)
			{
				if (IsInMapRange (x, y))
				{
					if (x != gridX || y != gridY)
						wallCount += map [x, y];
				}
				else
				{
//					wallCount++;
				}
			}
		}
		return wallCount;
	}

	bool IsInMapRange (int x, int y)
	{
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	void OnDrawGizmos ()
	{
		if (map != null)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Vector3 pos = new Vector3 (width * -0.5f + x * gridSize, 0f, height * -0.5f + y * gridSize);
					if (map [x, y] == 1)
						Gizmos.color = Color.black;
					else
						Gizmos.color = Color.white;
					Gizmos.DrawCube (pos, Vector3.one * 0.75f);
				}
			}
		}
	}
}
