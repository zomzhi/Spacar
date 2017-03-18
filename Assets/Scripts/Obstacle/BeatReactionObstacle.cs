using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyCompany.MyGame.Audio;
using MyCompany.MyGame;
using MyCompany;
using DG.Tweening;
using System;

namespace MyCompany.MyGame.Obstacle
{
	public class BeatReactionObstacle : ObstacleBase
	{

		#region Beat动画类型定义

		public class BeatAnimationBase : CommonAnimateBase
		{
			/// <summary>
			/// 响应的Beat类型
			/// </summary>
			public BeatDetection.EventType eventType = BeatDetection.EventType.Energy;
			public float toDuratioin;
			public float backDuration;

			/// <summary>
			/// 是否处于Beat动画过程
			/// </summary>
			/// <value><c>true</c> if not playing; otherwise, <c>false</c>.</value>
			public bool NotPlaying{ get { return (sequence == null || !sequence.IsActive ()); } }

			protected Sequence sequence;

			public void DoBeatAnimation (Action<Transform> onComplete)
			{
				if (sequence != null)
					sequence.Kill (false);

				sequence = GetSequence ();
				sequence.OnComplete (() => {
					Reset ();
					onComplete (animateTransform);
				});
				sequence.OnKill (() => sequence = null);
			}

			public virtual Sequence GetSequence ()
			{
				return null;
			}
		}

		public class BeatMaterialAnimationBase : BeatAnimationBase
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
		public class BeatAnimateScale : BeatAnimationBase
		{
			public Vector3 animateScale = Vector3.one;
			public bool multiplyOrigin = false;

			Vector3 initScale;

			public override void Init ()
			{
				if (NotPlaying)
					initScale = animateTransform.localScale;
			}

			public override void Reset ()
			{
				animateTransform.localScale = initScale;
			}

			public override Sequence GetSequence ()
			{
				Sequence newSeq = DOTween.Sequence ();
				Vector3 destScale = multiplyOrigin ? new Vector3 (animateTransform.localScale.x * animateScale.x, animateTransform.localScale.y * animateScale.y, animateTransform.localScale.z * animateScale.z) : animateScale;
				newSeq.Append (animateTransform.DOScale (destScale, toDuratioin));
				newSeq.Append (animateTransform.DOScale (initScale, backDuration));
				return newSeq;
			}
		}

		[System.Serializable]
		public class BeatAnimatePosition : BeatAnimationBase
		{
			public Vector3 movePosition;
			public bool relative = true;

			Vector3 initPosition;

			public override void Init ()
			{
				if (NotPlaying)
					initPosition = animateTransform.position;
			}

			public override void Reset ()
			{
				animateTransform.position = initPosition;
			}

			public override Sequence GetSequence ()
			{
				Sequence newSeq = DOTween.Sequence ();
				newSeq.Append (animateTransform.DOMove (movePosition, toDuratioin).SetRelative (relative));
				newSeq.Insert (toDuratioin, animateTransform.DOMove (initPosition, backDuration));
				return newSeq;
			}
		}

		[System.Serializable]
		public class BeatAnimateRotation : BeatAnimationBase
		{
			public Transform rotTransform;
			public Vector3 rotAngle;
			public RotateMode mode = RotateMode.Fast;

			Quaternion initRotation;

			public override void Init ()
			{
				if (NotPlaying)
					initRotation = animateTransform.rotation;
			}

			public override void Reset ()
			{
				animateTransform.rotation = initRotation;
			}

			public override Sequence GetSequence ()
			{
				Sequence newSeq = DOTween.Sequence ();
				if (rotTransform != null)
					newSeq.Append (animateTransform.DORotateQuaternion (rotTransform.rotation, toDuratioin));
				else
					newSeq.Append (animateTransform.DORotate (rotAngle, toDuratioin, mode));
				newSeq.Insert (toDuratioin, animateTransform.DORotateQuaternion (initRotation, backDuration));
				return newSeq;
			}
		}

		[System.Serializable]
		public class BeatAnimateMaterialColor : BeatMaterialAnimationBase
		{
			public Color toColor;
			public string propertyName;

			Color initColor;

			public override void Init ()
			{
				if (NotPlaying)
				{
					if (!string.IsNullOrEmpty (propertyName))
						initColor = ModifiedMat.GetColor (propertyName);
					else
						initColor = ModifiedMat.color;
				}
			}

			public override void Reset ()
			{
				if (!string.IsNullOrEmpty (propertyName))
					ModifiedMat.SetColor (propertyName, initColor);
				else
					ModifiedMat.color = initColor;
			}

			public override Sequence GetSequence ()
			{
				Sequence newSeq = DOTween.Sequence ();
				if (!string.IsNullOrEmpty (propertyName))
					newSeq.Append (ModifiedMat.DOColor (toColor, propertyName, toDuratioin));
				else
					newSeq.Append (ModifiedMat.DOColor (toColor, toDuratioin));
				if (!string.IsNullOrEmpty (propertyName))
					newSeq.Insert (toDuratioin, ModifiedMat.DOColor (initColor, propertyName, backDuration));
				else
					newSeq.Insert (toDuratioin, ModifiedMat.DOColor (initColor, backDuration));
				return newSeq;
			}
		}

		[System.Serializable]
		public class BeatAnimateMaterialFloat : BeatMaterialAnimationBase
		{
			public float toValue;
			public string propertyName;
			public Ease ease = Ease.Linear;

			float initValue;

			public override void Init ()
			{
				if (NotPlaying)
					initValue = ModifiedMat.GetFloat (propertyName);
			}

			public override void Reset ()
			{
				ModifiedMat.SetFloat (propertyName, initValue);
			}

			public override Sequence GetSequence ()
			{
				Sequence newSeq = DOTween.Sequence ();
				newSeq.Append (ModifiedMat.DOFloat (toValue, propertyName, toDuratioin).SetEase (ease));
				newSeq.Insert (toDuratioin, ModifiedMat.DOFloat (initValue, propertyName, backDuration).SetEase (ease));
				return newSeq;
			}
		}

		#endregion

		public BeatDetection.EventType testType = BeatDetection.EventType.Energy;
		BeatDetection.EventInfo testInfo = new BeatDetection.EventInfo ();

		[Header ("Beat Animation List")]
		public List<BeatAnimateScale> beatAnimateScaleList = new List<BeatAnimateScale> ();
		public List<BeatAnimatePosition> beatAnimatePositionList = new List<BeatAnimatePosition> ();
		public List<BeatAnimateRotation> beatAnimateRotationList = new List<BeatAnimateRotation> ();
		public List<BeatAnimateMaterialColor> beatAnimateMatColorList = new List<BeatAnimateMaterialColor> ();
		public List<BeatAnimateMaterialFloat> beatAnimateMatFloatList = new List<BeatAnimateMaterialFloat> ();

		List<BeatAnimationBase> totalBeatAnimationList = new List<BeatAnimationBase> ();

		BeatDetection beatDetection;

		protected override void OnAwake ()
		{
			beatDetection = GameSystem.Instance.AudioMgr.beatDetection;
			UnityLog.Assert (beatDetection != null, "Missing BeatDetection!");

			totalBeatAnimationList.AddRange (beatAnimateScaleList.ToArray ());
			totalBeatAnimationList.AddRange (beatAnimatePositionList.ToArray ());
			totalBeatAnimationList.AddRange (beatAnimateRotationList.ToArray ());
			totalBeatAnimationList.AddRange (beatAnimateMatColorList.ToArray ());
			totalBeatAnimationList.AddRange (beatAnimateMatFloatList.ToArray ());

		}

		void Update ()
		{
			#if UNITY_EDITOR
			if (Input.GetKeyDown (KeyCode.B))
			{
				testInfo.messageInfo = testType;
				OnBeatEvent (testInfo);
			}
			#endif
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			beatDetection.CallBackFunction += OnBeatEvent;
		}

		protected override void OnDeactivated ()
		{
			base.OnDeactivated ();
			beatDetection.CallBackFunction -= OnBeatEvent;
		}

		void OnDestroy ()
		{
			beatDetection.CallBackFunction -= OnBeatEvent;
		}

		/// <summary>
		/// 注册到BeatDetection的响应函数
		/// </summary>
		/// <param name="info">Info.</param>
		protected virtual void OnBeatEvent (BeatDetection.EventInfo info)
		{
			if (!gameObject.activeInHierarchy || !Activated)
				return;

			foreach (BeatAnimationBase beatAnimation in totalBeatAnimationList)
			{
				if (beatAnimation.eventType == info.messageInfo)
				{
					// 暂停所有该物体正在播放的公共动画
					List<Tweener> playingList = GetPlayingList (beatAnimation.animateTransform);
					if (playingList != null)
					{
						foreach (Tweener tweener in playingList)
						{
							tweener.Pause ();
						}
					}

					//  初始化记录初始数据
					beatAnimation.Init ();

					// 开始Beat动画
					beatAnimation.DoBeatAnimation (OnBeatAnimationOver);
				}
			}
		}

		/// <summary>
		/// Beat动画结束后回调此函数，继续播放公有动画
		/// </summary>
		/// <param name="animateTransform">Animate transform.</param>
		private void OnBeatAnimationOver (Transform animateTransform)
		{
			List<Tweener> playingList = GetPlayingList (animateTransform);
			if (playingList != null)
			{
				foreach (Tweener tweener in playingList)
				{
					tweener.Play ();
				}
			}
		}
	}
}

