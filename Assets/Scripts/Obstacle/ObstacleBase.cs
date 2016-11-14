using UnityEngine;
using System.Collections;
using MyCompany.Common.Interface;
using MyCompany.MyGame.Level;
using MyCompany.Common;

namespace MyCompany.MyGame.Obstacle
{
	public class ObstacleBase : MonoBehaviour, IRectangleInt, IPreloadable
	{
		#region Public Member

		#endregion

		#region Attribute

		protected Transform thisTrans;
		public Transform Trans
		{
			get{ return thisTrans; } 
		}

		public int width{ get { return (int)specificWidth; } }

		public int height{ get { return (int)specificHeight; } }

		public Vector3 leftBottom{ get { return Trans.position; } }

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

		public int CoordX{ get; protected set; }
		public int CoordY{ get; protected set; }
		public int AreaSize{ get { return width * height; } }

		#endregion

		#region Private Member

		[SerializeField] private GameDefine.BLOCK_SPECIFICATION specificWidth;
		[SerializeField] private GameDefine.BLOCK_SPECIFICATION specificHeight;

		[SerializeField]
		private bool isPreload = true;
		[SerializeField]
		private int preloadAmount = GameDefine.PRELOAD_AMOUNT;
		[SerializeField]
		private int preloadFrames = 0;

		#endregion

		#region Internal Methods

		void Awake ()
		{
			thisTrans = transform;
		}

		#endregion

		#region Util Methods

		public void SetCoordinate (Coordinate coord)
		{
			SetCoordinate (coord.x, coord.y);
		}

		public void SetCoordinate (int x, int y)
		{
			CoordX = x;
			CoordY = y;
		}

		#endregion
	}
}

