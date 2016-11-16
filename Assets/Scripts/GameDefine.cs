using UnityEngine;
using MyCompany.MyGame.Level;

namespace MyCompany.MyGame
{
	public class GameDefine
	{
		public const string RESOURCE_FOLDER = "Assets/Resources/";
		public const string BLOCK_PREFAB_RESOURCE_PATH = "BlockPrefabs/";
		public const string PREFAB_EXTENSION = ".prefab";

		public const string OBSTACLE_PREFAB_RESOURCE_PATH = "Obstacles/";

		public const string FILE_EXTENSION_JSON = ".json";
		public const string SAVED_LEVEL_PATH = "Assets/Resources/";
		public const string SAVED_LEVEL_RESOURCE_PATH = "SerializedLevel/";
		public const string SAVED_LEVEL_FULL_PATH = SAVED_LEVEL_PATH + SAVED_LEVEL_RESOURCE_PATH;

		public const string GROUND_LAYER_NAME = "ground";

		public const string POOL_NAME_LEVELBLOCK = "Level Block";
		public const string POOL_NAME_OBSTACLE = "Obstacle";

		public const string GAME_SCENE = "_Scenes/MainScene";

		/// <summary>
		/// 默认摇杆在屏幕左区域
		/// </summary>
		public const bool DEFAULT_JOYSTICK_LEFT = true;

		/// <summary>
		/// 移动摇杆向左或向右移动的最大距离
		/// </summary>
		public const int STICK_RANGE = 100;

		/// <summary>
		/// 每种桥最多可接两种类型的桥
		/// </summary>
		public const int MAX_CONNECTABLE_NUM = 2;

		/// <summary>
		/// LevelBlock以及LevelBridge在Z轴上统一高度为2
		/// </summary>
		public const int BLOCK_TALL = 2;

		/// <summary>
		/// 对象池物体的默认预加载数目
		/// </summary>
		public const int PRELOAD_AMOUNT = 10;

		/// <summary>
		/// 默认预加载需要的帧数
		/// </summary>
		public const int DEFAULT_PRELOAD_FRAMES = 2;

		/// <summary>
		/// 默认第一座桥的Block块数
		/// </summary>
		public const int DEFAULT_FIRST_BRIDGE_BLOCK_COUNT = 5;

		/// <summary>
		/// 四周不可通行的格子所增加的代价
		/// </summary>
		public const int NOT_WALKABLE_PENALTY = 20;

		/// <summary>
		/// 初始Block的位置
		/// </summary>
		public static readonly Vector3 DEFAULT_ORIGIN = Vector3.zero;

		public const BLOCK_SPECIFICATION BLOCK_HEIGHT_SPEC = BLOCK_SPECIFICATION.METER_20;

		/// <summary>
		/// 路径宽度，向四周延伸的距离
		/// </summary>
		public const int PATH_WIDTH = 1;

		/// <summary>
		/// 障碍面积小于此值且孤立将有概率被过滤掉
		/// </summary>
		public const int FILTER_AREA_SIZE = 4;

		/// <summary>
		/// Block宽度和高度的规格
		/// </summary>
		public enum BLOCK_SPECIFICATION
		{
			METER_1 = 1,
			METER_2 = 2,
			METER_3 = 3,
			METER_4 = 4,
			METER_5 = 5,
			METER_6 = 6,
			METER_7 = 7,
			METER_8 = 8,
			METER_9 = 9,
			METER_10 = 10,
			METER_12 = 12,
			METER_15 = 15,
			METER_18 = 18,
			METER_20 = 20,
		}

		/// <summary>
		/// 块的旋转，对应于ELevelType
		/// </summary>
		public static readonly Quaternion[] BLOCK_ROTATION = new Quaternion[] {
			Quaternion.identity,				// ALONG_X_FACE_Y
			Quaternion.Euler (-90f, 0f, 0f),	// ALONG_X_FACE_Z
			Quaternion.Euler (0f, 0f, 90f),		// ALONG_Y_FACE_X
			Quaternion.Euler (0f, -90f, 90f),	// ALONG_Y_FACE_Z
			Quaternion.Euler (90f, -90f, 0f),	// ALONG_Z_FACE_X
			Quaternion.Euler (0f, -90f, 0f),	// ALONG_Z_FACE_Y
		};


		/// <summary>
		/// 每种桥可接的下种桥的类型，对应于ElevelType
		/// </summary>
		public static readonly ELevelType[][] CONNECTABLE_TYPE = new ELevelType[][] {
			new ELevelType[] { ELevelType.ALONG_Z_FACE_Y, ELevelType.ALONG_Y_FACE_X },
			new ELevelType[] { ELevelType.ALONG_Y_FACE_Z, ELevelType.ALONG_Z_FACE_Y },
			new ELevelType[] { ELevelType.ALONG_X_FACE_Y, ELevelType.ALONG_Z_FACE_X },
			new ELevelType[] { ELevelType.ALONG_Z_FACE_Y, ELevelType.ALONG_X_FACE_Z },
			new ELevelType[] { ELevelType.ALONG_Y_FACE_X, ELevelType.ALONG_X_FACE_Y },
			new ELevelType[] { ELevelType.ALONG_X_FACE_Y, ELevelType.ALONG_Y_FACE_Z },
		};

		/// <summary>
		/// 每种桥类型的方向向量，按Forward, Right, Up顺序
		/// </summary>
		public static readonly Vector3[][] LEVEL_DIRECTION = new Vector3[][] {
			new Vector3[] { Vector3.right, Vector3.back, Vector3.up },	// ALONG_X_FACE_Y
			new Vector3[] { Vector3.right, Vector3.down, Vector3.back },	// ALONG_X_FACE_Z
			new Vector3[] { Vector3.up, Vector3.back, Vector3.left },	// ALONG_Y_FACE_X
			new Vector3[] { Vector3.up, Vector3.right, Vector3.back },	// ALONG_Y_FACE_Z
			new Vector3[] { Vector3.forward, Vector3.up, Vector3.left },	// ALONG_Z_FACE_X
			new Vector3[] { Vector3.forward, Vector3.right, Vector3.up },	// ALONG_Z_FACE_Y
		};
	}
}

