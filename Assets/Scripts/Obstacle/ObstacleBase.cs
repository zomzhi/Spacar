using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyCompany.Common.Interface;
using MyCompany.MyGame.Level;
using MyCompany.Common;
using DG.Tweening;

namespace MyCompany.MyGame.Obstacle
{
	public class ObstacleBase : MonoBehaviour, IRectangleInt, IPreloadable
	{
		#region Public Member

		[System.Serializable]
		public class CommonAnimateBase
		{
			public Transform animateTransform;

			protected Vector3 startPosition;
			protected Quaternion startRot;
			protected Vector3 startScale;

			public virtual void Init ()
			{
				startPosition = animateTransform.position;
				startRot = animateTransform.rotation;
				startScale = animateTransform.localScale;
			}

			public virtual void Reset ()
			{
				animateTransform.position = startPosition;
				animateTransform.rotation = startRot;
				animateTransform.localScale = startScale;
			}

			public virtual Tweener DoCommonAnimate ()
			{
				return null;
			}
		}

		[System.Serializable]
		public class AnimatePath : CommonAnimateBase
		{
			public Transform[] pathTrans;
			public float duration;
			public PathType pathType = PathType.CatmullRom;
			public bool closedPath = true;
			public float lookAhead = 0.001f;

			public override Tweener DoCommonAnimate ()
			{
				Vector3[] path = new Vector3[pathTrans.Length];
				for (int i = 0; i < pathTrans.Length; i++)
				{
					path [i] = pathTrans [i].position;
				}

				Tweener tweener = animateTransform.DOPath (path, duration, pathType)
					.SetOptions (closedPath).SetLoops (-1).SetLookAt (lookAhead);

				return tweener;
			}
		}

		[System.Serializable]
		public class AnimateRandomPoint : CommonAnimateBase
		{
			public Transform[] patrolPoints;
			public Vector2 durationRange;
			public Vector2 delayRange;
			public Ease ease = Ease.Linear;
			public bool randomMove = false;

			private int curIndex = 0;

			public Vector3 GetRandomPointPos ()
			{
				if (randomMove)
				{
					curIndex += UnityEngine.Random.Range (1, patrolPoints.Length);
					curIndex %= patrolPoints.Length;
				}
				else
				{
					curIndex = (curIndex + 1) % patrolPoints.Length;
				}
				return patrolPoints [curIndex].position;
			}

			public float GetDuration ()
			{
				return UnityEngine.Random.Range (durationRange.x, durationRange.y);
			}

			public float GetDelay ()
			{
				return UnityEngine.Random.Range (delayRange.x, delayRange.y);
			}

			public override Tweener DoCommonAnimate ()
			{
				Tweener tweener = animateTransform.DOMove (GetRandomPointPos (), GetDuration ()).SetDelay (GetDelay ()).SetAutoKill (false);
				tweener.OnComplete (() => {
					tweener.ChangeStartValue (animateTransform.position);
					tweener.ChangeEndValue (GetRandomPointPos (), GetDuration ());
					tweener.SetDelay (GetDelay ());
					tweener.Restart ();
				});
				return tweener;
			}
		}

		[System.Serializable]
		public class AnimatePosition : CommonAnimateBase
		{
			public Transform destTrans;
			public Vector3 destPosition;
			public bool relative = true;
			public bool onlyDelayFirst = false;
			public Vector2 durationRange;
			public Vector2 delayRange;

			Vector3[] destinations = new Vector3[2];
			int curIndex = 0;

			public override void Init ()
			{
				base.Init ();
				destinations [1] = (destTrans != null) ? destTrans.position : 
					((relative) ? animateTransform.TransformPoint (destPosition) : destPosition);
				destinations [0] = startPosition;
			}

			public float GetDuration ()
			{
				return UnityEngine.Random.Range (durationRange.x, durationRange.y);
			}

			public float GetDelay ()
			{
				return UnityEngine.Random.Range (delayRange.x, delayRange.y);
			}

			public Vector3 GetPosition ()
			{
				curIndex = (curIndex + 1) % 2;
				return destinations [curIndex];
			}

			public override Tweener DoCommonAnimate ()
			{
				Tweener tweener;
				tweener = animateTransform.DOMove (GetPosition (), GetDuration ()).SetDelay (GetDelay ()).SetAutoKill (false);
				tweener.OnComplete (() => {
					tweener.ChangeStartValue (animateTransform.position);
					tweener.ChangeEndValue (GetPosition (), GetDuration ());
					tweener.Restart (!onlyDelayFirst);
				});
				return tweener;
			}
		}

		[System.Serializable]
		public class AnimateRotation : CommonAnimateBase
		{
			public Transform rotTransform;
			public Vector3 rotVector;
			public Vector2 durationRange;
			public Vector2 delayRange;
			public bool onlyDelayFirst = false;

			Quaternion[] destRotations = new Quaternion[2];
			int curIndex = 0;

			public override void Init ()
			{
				base.Init ();
				destRotations [0] = startRot;
				destRotations [1] = (rotTransform != null) ? rotTransform.rotation : startRot * Quaternion.Euler (rotVector);
			}

			public float GetDuration ()
			{
				return UnityEngine.Random.Range (durationRange.x, durationRange.y);
			}

			public float GetDelay ()
			{
				return UnityEngine.Random.Range (delayRange.x, delayRange.y);
			}

			public Quaternion GetDestRotation ()
			{
				curIndex = (curIndex + 1) % 2;
				return destRotations [curIndex];
			}

			public override Tweener DoCommonAnimate ()
			{
				Tweener tweener;
				tweener = animateTransform.DORotateQuaternion (GetDestRotation (), GetDuration ()).SetDelay (GetDelay ()).SetAutoKill (false);
				tweener.OnComplete (() => {
					tweener.SetDelay (GetDelay ());
					tweener.ChangeStartValue (animateTransform.rotation);
					tweener.ChangeEndValue (GetDestRotation (), GetDuration ());
					tweener.Restart (!onlyDelayFirst);
				});

				return tweener;
			}
		}

		[System.Serializable]
		public class AnimateScale : CommonAnimateBase
		{
			public Vector3 scale = Vector3.one;
			public Vector2 delayRange;
			public float duration;
			public Ease ease;
			public LoopType loopType = LoopType.Yoyo;

			public float GetDelay ()
			{
				return UnityEngine.Random.Range (delayRange.x, delayRange.y);
			}

			public override Tweener DoCommonAnimate ()
			{
				Tweener tweener = animateTransform.DOScale (scale, duration)
					.SetEase (ease).SetLoops (-1, loopType).SetAutoKill (false);
				return tweener;
			}
		}

		public class AnimateMaterial : CommonAnimateBase
		{
			public string materialName;

			public Material ModifiedMat
			{
				get
				{
					if (mat == null)
					{
						Renderer renderer = animateTransform.GetComponent<Renderer> ();
						UnityLog.Assert (renderer != null, "Transform missing renderer component!", animateTransform);
						if (!string.IsNullOrEmpty (materialName))
						{
							for (int i = 0; i < renderer.materials.Length; i++)
							{
								if (renderer.materials [i].name.Contains (materialName))
								{
									mat = renderer.materials [i];
									break;
								}
							}
						}
						else
						{
							mat = renderer.material;
						}
					}
					return mat;
				}
			}

			protected Material mat;
		}

		[System.Serializable]
		public class AnimateMaterialColor : AnimateMaterial
		{
			public Color[] colorList;
			public float duration;
			public string propertyName;

			Color initColor;
			int curIndex = 0;

			public override void Init ()
			{
				if (!string.IsNullOrEmpty (propertyName))
					initColor = ModifiedMat.GetColor (propertyName);
				else
					initColor = ModifiedMat.color;
			}

			public override void Reset ()
			{
				if (!string.IsNullOrEmpty (propertyName))
					ModifiedMat.SetColor (propertyName, initColor);
				else
					ModifiedMat.color = initColor;
			}

			public Color GetCurColor ()
			{
				return colorList [curIndex];
			}

			public Color GotoNextColor ()
			{
				curIndex = (curIndex + 1) % colorList.Length;
				return colorList [curIndex];
			}

			public override Tweener DoCommonAnimate ()
			{
				Tweener tweener;
				if (!string.IsNullOrEmpty (propertyName))
					tweener = ModifiedMat.DOColor (GetCurColor (), duration);
				else
					tweener = ModifiedMat.DOColor (GetCurColor (), propertyName, duration);
				tweener.SetAutoKill (false);
				tweener.SetLoops (-1);
				tweener.OnStepComplete (() => {
					tweener.ChangeStartValue (GetCurColor ());
					GotoNextColor ();
					tweener.ChangeEndValue (GetCurColor ());
				});

				return tweener;
			}
		}

		[System.Serializable]
		public class AnimateMaterialFloat : AnimateMaterial
		{
			public string propertyName;
			public float floatValue;
			public float duration;
			public LoopType loopType = LoopType.Yoyo;
			public Ease ease = Ease.Linear;

			float initValue;

			public override void Init ()
			{
				if (!string.IsNullOrEmpty (propertyName))
				{
					initValue = ModifiedMat.GetFloat (propertyName);
				}
			}

			public override void Reset ()
			{
				if (!string.IsNullOrEmpty (propertyName))
				{
					ModifiedMat.SetFloat (propertyName, initValue);
				}
			}

			public override Tweener DoCommonAnimate ()
			{
				Tweener tweener = ModifiedMat.DOFloat (floatValue, propertyName, duration)
					.SetLoops (-1, loopType).SetEase (ease);
				return tweener;
			}
		}

		[Header ("Common Animation List")]
		public List<AnimatePath> animatePathList = new List<AnimatePath> ();
		public List<AnimatePosition> animatePositionList = new List<AnimatePosition> ();
		public List<AnimateRandomPoint> animateRandomPointList = new List<AnimateRandomPoint> ();
		public List<AnimateRotation> animateRotationList = new List<AnimateRotation> ();
		public List<AnimateScale> animateScaleList = new List<AnimateScale> ();
		public List<AnimateMaterialColor> animateColorList = new List<AnimateMaterialColor> ();
		public List<AnimateMaterialFloat> animateMatFloatList = new List<AnimateMaterialFloat> ();

		#endregion

		#region Protected Member

		/// <summary>
		/// stores common playing tweeners for each transform component.
		/// </summary>
		protected Dictionary<Transform, List<Tweener>> transTweenerDict = new Dictionary<Transform, List<Tweener>> ();

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

		/// <summary>
		/// 是否处于激活状态，只有处于激活状态的障碍需要更新动画
		/// </summary>
		/// <value><c>true</c> if activated; otherwise, <c>false</c>.</value>
		public bool Activated{ get; private set; }

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

		private List<CommonAnimateBase> commonAnimateBaseList = new List<CommonAnimateBase> ();

		#endregion

		#region Internal Methods

		void Awake ()
		{
			thisTrans = transform;

			commonAnimateBaseList.AddRange (animatePathList.ToArray ());
			commonAnimateBaseList.AddRange (animatePositionList.ToArray ());
			commonAnimateBaseList.AddRange (animateRandomPointList.ToArray ());
			commonAnimateBaseList.AddRange (animateRotationList.ToArray ());
			commonAnimateBaseList.AddRange (animateScaleList.ToArray ());
			commonAnimateBaseList.AddRange (animateColorList.ToArray ());
			commonAnimateBaseList.AddRange (animateMatFloatList.ToArray ());

			OnAwake ();
		}

		void Start ()
		{
			Activate ();
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


		/// <summary>
		/// 激活公共动画
		/// </summary>
		public void Activate ()
		{
			Activated = true;

			foreach (CommonAnimateBase commonAnimation in commonAnimateBaseList)
			{
				commonAnimation.Init ();
				AddTransformTweener (commonAnimation.animateTransform, commonAnimation.DoCommonAnimate ());
			}


			OnActivated ();
		}

		/// <summary>
		/// 禁用公共动画并重置到初始状态
		/// </summary>
		public void DeactivateAndReset ()
		{
			Activated = false;

			foreach (KeyValuePair<Transform, List<Tweener>> pair in transTweenerDict)
			{
				List<Tweener> tweenerList = pair.Value;
				foreach (Tweener tweener in tweenerList)
				{
					tweener.Kill ();
				}
				tweenerList.Clear ();
				tweenerList = null;
			}
			transTweenerDict.Clear ();

			foreach (CommonAnimateBase commonAnimation in commonAnimateBaseList)
			{
				commonAnimation.Reset ();
			}

			OnDeactivated ();
		}

		#endregion

		#region Protected Methods

		protected virtual void OnAwake ()
		{
		}


		protected List<Tweener> GetPlayingList (Transform tweenTransform)
		{
			if (transTweenerDict.ContainsKey (tweenTransform))
				return transTweenerDict [tweenTransform];
			else
				return null;
		}

		protected virtual void OnActivated ()
		{
		}

		protected virtual void OnDeactivated ()
		{
		}

		#endregion

		#region Private Methods

		private void AddTransformTweener (Transform targetTrans, Tweener tweener)
		{
			// add to tweener list
			List<Tweener> tweenerList;
			if (transTweenerDict.TryGetValue (targetTrans, out tweenerList))
			{
				tweenerList.Add (tweener);
			}
			else
			{
				tweenerList = new List<Tweener> ();
				tweenerList.Add (tweener);
				transTweenerDict.Add (targetTrans, tweenerList);
			}
		}

		#endregion
	}
}

