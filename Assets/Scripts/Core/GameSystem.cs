using UnityEngine;
using System.Collections;
using MyCompany.Common;
using MyCompany;
using UnityEngine.UI;
using Resolution = MyCompany.Common.Resolution;
using PathologicalGames;
using MyCompany.MyGame.Level;
using MyCompany.MyGame.Obstacle;
using UnityEngine.SceneManagement;
using MyCompany.MyGame.UI;

namespace MyCompany.MyGame
{
	public sealed class GameSystem : MonoSingleton<GameSystem>
	{
		public bool logMessage = true;
		public bool debugMode = true;
		public MainUI mainUI;

		public Text progress;
		public float speed = 10f;

		#region Attribute

		private bool _paused;

		public bool Paused
		{
			get{ return _paused; }
			set{ _paused = value; }
		}

		public bool Playing
		{
			get
			{
				return !Paused && currentGamePlay.startPlay;
			}
		}

		public LevelBlockFactory BlockFactory{ get; private set; }

		public ObstacleFactory ObstaclesFactory{ get; private set; }

		#endregion

		#region Private Member

		private GamePlay currentGamePlay;

		private SpawnPool levelBlockPool;
		private SpawnPool obstaclePool;

		private float counter;

		#endregion


		protected override void OnAwake ()
		{
			DontDestroyOnLoad (gameObject);
			UnityLog.logMessage = logMessage;
		}

		IEnumerator Start ()
		{
			SetupEnvironment ();
			SetupResolution ();

			yield return LoadSplash ();

			DisplayLoadingImage ();

			yield return InitializePoolManagement ();

			yield return LoadGameScene ();
		}

		void Update ()
		{
//			counter += speed * Time.deltaTime;
//			progress.text = counter.ToString ("##.00");

			#if UNITY_EDITOR
			UnityLog.logMessage = logMessage;
			#endif

			if (Input.GetKeyDown (KeyCode.S))
			{
				StartPlay ();
			}

			if (Input.GetKeyDown (KeyCode.G))
			{
				StartGenerate ();
			}
		}

		#region Private Member

		private void DisplayLoadingImage ()
		{
			
		}

		private void SetupPoolManager ()
		{
			GameObject poolManager = new GameObject ("Pool Manager");
			poolManager.transform.SetParent (this.transform);
			GameObject levelBlockPoolGo = new GameObject ("Level Block Pool");
			levelBlockPoolGo.transform.SetParent (poolManager.transform);
			GameObject obstaclePoolGo = new GameObject ("Obstacle Pool");
			obstaclePoolGo.transform.SetParent (poolManager.transform);
			levelBlockPool = PoolManager.Pools.Create (GameDefine.POOL_NAME_LEVELBLOCK, levelBlockPoolGo);
			obstaclePool = PoolManager.Pools.Create (GameDefine.POOL_NAME_OBSTACLE, obstaclePoolGo);
		}

		private void SetupEnvironment ()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}

		private void SetupResolution ()
		{
			Resolution goodRes = Resolution.FindGood (
				                     new Resolution () { width = 1920, height = 1080 },
				                     new Resolution () { width = 1440, height = 1080 },
				                     new Resolution () {
					width = Screen.currentResolution.width,
					height = Screen.currentResolution.height
				},
				                     0.1f,
				                     Screen.dpi,
				                     200
			                     );

			Resolution.ApplyToScreen (goodRes);
		}

		IEnumerator LoadSplash ()
		{
			UnityLog.Log ("Start load splash");
			yield return null;
			UnityLog.Log ("End load splash");
		}

		IEnumerator InitializePoolManagement ()
		{
			UnityLog.Log ("Start initialize pool management.");

//			ResourceRequest request = Resources.LoadAsync<GameObject> ("TestCube");
//			while (!request.isDone)
//				yield return null;
//			GameObject prefab = request.asset as GameObject;
//
//			for (int i = 0; i < 10000; i++)
//				Instantiate (prefab, Vector3.right * i, Quaternion.identity);

			SetupPoolManager ();
			BlockFactory = new LevelBlockFactory ();
			yield return BlockFactory.Init (levelBlockPool, null);
			ObstaclesFactory = new ObstacleFactory ();
			yield return ObstaclesFactory.Init (obstaclePool, null);

			UnityLog.Log ("End initialize pool management.");
		}

		IEnumerator LoadGameSceneDisplayLoading ()
		{
			yield return mainUI.ShowLoadImage ();

			yield return LoadGameScene ();
		}

		IEnumerator LoadGameScene ()
		{
			AsyncOperation op = SceneManager.LoadSceneAsync (GameDefine.GAME_SCENE, LoadSceneMode.Single);
			op.allowSceneActivation = true;
			while (!op.isDone)
				yield return null;

			Scene mainScene = SceneManager.GetSceneByName (GameDefine.GAME_SCENE);
			while (!mainScene.isLoaded)
				yield return null;

			currentGamePlay = GetGamePlay (mainScene);
			UnityLog.Assert ((currentGamePlay != null), "Missing GamePlay component in MainScene");
		}

		GamePlay GetGamePlay (Scene scene)
		{
			GamePlay gamePlay;
			foreach (GameObject go in scene.GetRootGameObjects())
			{
				gamePlay = go.GetComponent<GamePlay> ();
				if (gamePlay != null)
					return gamePlay;
			}
			return null;
		}

		#endregion

		#region Util Methods

		public void PauseGame ()
		{
			
		}

		public void BackToHome ()
		{
			
		}

		public void StartPlay ()
		{
			currentGamePlay.StartPlay ();
		}

		public void StartGenerate ()
		{
			currentGamePlay.StartGenerate ();
		}

		#endregion
	}
}

