using System;
using UnityEngine;
using MyCompany.Common.Interface;
using MyCompany.Common;

namespace MyCompany.MyGame.Level
{
	/// <summary>
	/// LevelBlock是组成LevelBridge的地形组件
	/// 注意由于Probuilder的原因，建模坐标是水平方向朝世界z轴的负方向延伸，垂直方向是朝世界x正方向延伸
	/// </summary>
	public class LevelBlock : MonoBehaviour, IRectangleInt, IPreloadable
	{
		#region public member

		// Block会从左至右或从右至左断裂，normalStartIndex标示不会断裂block的起始索引
		public int normalStartIndex;
		// Block会从左至右或从右至左断裂，normalEndIndex标示不会断裂block的终止索引
		public int normalEndIndex;
		// 断裂的block是否可放置物体
		public bool placeable = false;



		[HideInInspector]
		public ELevelType blockType;

		/// <summary>
		/// 所属的桥
		/// </summary>
		[HideInInspector]
		public LevelBridge bridgeBelong;


		private int startX = -1;
		/// <summary>
		/// 路径的初始点x坐标
		/// </summary>
		/// <value>The start x.</value>
		public int StartX
		{
			get
			{
				if (startX < 0)
					startX = GetPathValidCoordX ();
				return startX;
			}
		}

		public int CenterY
		{
			get { return height / 2; }
		}

		#endregion

		#region Private member

		[SerializeField] private GameDefine.BLOCK_SPECIFICATION specificWidth = GameDefine.BLOCK_SPECIFICATION.METER_20;
		private GameDefine.BLOCK_SPECIFICATION specificHeight = GameDefine.BLOCK_HEIGHT_SPEC;

		[SerializeField]
		private bool isPreload = true;
		[SerializeField]
		private int preloadAmount = GameDefine.PRELOAD_AMOUNT;
		[SerializeField]
		private int preloadFrames = 0;

		#endregion

		#region Rectangle Info

		public int width
		{
			get{ return (int)specificWidth; }
		}

		public int height
		{
			get{ return (int)specificHeight; }
		}

		public Vector3 leftBottom
		{
			get{ return transform.position; }
		}

		public Vector3 NormalBottomStartPos
		{
			get{ return leftBottom - transform.forward * normalStartIndex; }
		}

		public Vector3 NormalBottomEndPos
		{
			get{ return leftBottom - transform.forward * (normalEndIndex + 1); }
		}

		public bool IsPreload{ get { return isPreload; } }

		public int PreloadAmount{ get { return preloadAmount; } }

		public int PreloadFrames
		{ 
			get
			{ 
				if (preloadFrames <= 0)
					preloadFrames = Mathf.Min (GameDefine.DEFAULT_PRELOAD_FRAMES, PreloadAmount);
				else
					preloadFrames = Mathf.Min (preloadFrames, PreloadAmount);
				return preloadFrames;
			} 
		}

		#endregion

		#region Private Methods

		void OnDrawGizmos ()
		{
//			Gizmos.DrawCube (leftBottom, Vector3.one * 1.5f);
		}

		#endregion

		#region public methods

		public int GetPathValidCoordX ()
		{
			return UnityEngine.Random.Range (normalStartIndex, normalEndIndex);
		}

		public int GetValidCenterCoordX ()
		{
			return (normalStartIndex + normalEndIndex) / 2;
		}

		/// <summary>
		/// 得到某一位置在normalblock水平方向的比例
		/// </summary>
		/// <returns>The horizon ratio by position.</returns>
		/// <param name="pos">Position.</param>
		public float GetHorizonRatioByPos (Vector3 pos)
		{
			float dist = Vector3.Dot (NormalBottomStartPos - pos, transform.forward);
			float ratio = dist / (normalEndIndex - normalStartIndex + 1);
			ratio = Mathf.Clamp01 (ratio);
			return ratio;
		}

		#endregion
	}
}

