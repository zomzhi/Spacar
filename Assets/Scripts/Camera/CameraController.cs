using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using MyCompany.MyGame.Player;

namespace MyCompany.MyGame.CameraControl
{
	[RequireComponent (typeof(Camera))]
	public class CameraController : MonoBehaviour
	{
		#region Public Member

		public bool manuallyControl = false;
		public float cameraMoveSmoothTime = 1f;
		public float xAngleSmoothTime = 1f;
		public float yAngleSmoothTime = 1f;

		[Range (0f, 1f)]
		public float manualHRatio = 0;
		[Range (0f, 1f)]
		public float manualVRatio = 0;

		[HideInInspector]
		public CameraParameter[] parameters = new CameraParameter[6];

		[HideInInspector]
		public PlayerController playerController;

		[System.Serializable]
		public struct Coefficent
		{
			public float horizonCoef;
			public float verticalCoef;
			public float constCoef;
		}

		[System.Serializable]
		public class CameraParameter
		{
			public string name;
			public Coefficent yAngleCoef;
			public Coefficent xAngleCoef;
			public Coefficent zoomCoef;

			public AnimationCurve verticalTraceDistCurve;

			// 追踪点水平方向偏移，为1时没有偏移，为0时一直位于中线上
			[Range (0f, 1f)]
			public float horizonTraceRatio;

			public float GetXAngle (float horizonRatio, float verticalRatio)
			{
				return xAngleCoef.horizonCoef * horizonRatio + xAngleCoef.verticalCoef * verticalRatio + xAngleCoef.constCoef;
			}

			public float GetYAngle (float horizonRatio, float verticalRatio)
			{
				return yAngleCoef.horizonCoef * horizonRatio + yAngleCoef.verticalCoef * verticalRatio + yAngleCoef.constCoef;
			}

			public float GetZoomDist (float horizonRatio, float verticalRatio)
			{
				return zoomCoef.horizonCoef * horizonRatio + zoomCoef.verticalCoef * verticalRatio + zoomCoef.constCoef;
			}

			public float GetTraceVerticalDist (float verticalRatio)
			{
				return verticalTraceDistCurve.Evaluate (verticalRatio);
			}

			public float GetTraceHorizonDistRatio (float horizonRatio)
			{
				horizonTraceRatio = Mathf.Clamp01 (horizonTraceRatio);
				return (0.5f - horizonRatio) * (1 - horizonTraceRatio);
			}
		}

		#endregion



		#region PrivateMember

		private Vector3 playerPosition = Vector3.zero;
		private Vector3 playerProjectPosition = Vector3.zero;
		private Vector3 tracePosition = Vector3.zero;

		private float horizonRatio;
		private float verticalRatio;

		#endregion

		private Transform camTrans;
		private LevelBridge currentLevelBridge;
		private CameraParameter currentCamParam;
		private float traceVerticalDist;
		private float traceHorizonDist;
		private float destAngleX;
		private float destAngleY;
		private Quaternion traceRotation;
		private Vector3 zoomVec = Vector3.zero;

		Vector3 camDestPosition;
		Quaternion camDestRotation;
		Vector3 camMoveVelocity;
		float curAngleX;
		float curAngleY;
		float xAngleVelocity;
		float yAngleVelocity;

		#region Internal Methods

		void Awake ()
		{
			camTrans = transform;
		}

		void Start ()
		{
			curAngleX = camTrans.eulerAngles.x;
			curAngleY = camTrans.eulerAngles.y;
		}

		void LateUpdate ()
		{
			if (currentLevelBridge == null || GameSystem.Instance.Paused)
				return;

			#if UNITY_EDITOR
			if (manuallyControl)
				UpdatePosRot (manualHRatio, manualVRatio);
			else
				UpdatePosRot (horizonRatio, verticalRatio);
			#else
			UpdatePosRot(horizonRatio, verticalRatio);
			#endif
		}

		void UpdatePosRot (float hRatio, float vRatio)
		{
			playerProjectPosition = currentLevelBridge.leftBottom +
			currentLevelBridge.Right * currentLevelBridge.width * hRatio +
			currentLevelBridge.Forward * currentLevelBridge.height * vRatio +
			currentLevelBridge.Up * GameDefine.BLOCK_TALL;

			traceVerticalDist = currentLevelBridge.dummy ? currentCamParam.GetTraceVerticalDist (0f) : currentCamParam.GetTraceVerticalDist (vRatio);
			// TODO: 应该使用玩家当前处于的block水平方向的ratio与宽度
			traceHorizonDist = currentCamParam.GetTraceHorizonDistRatio (hRatio) * currentLevelBridge.width;
			tracePosition = playerProjectPosition + currentLevelBridge.Forward * traceVerticalDist + currentLevelBridge.Right * traceHorizonDist;
			if (currentLevelBridge.dummy)
			{
				destAngleX = currentCamParam.GetXAngle (hRatio, 0f);
				destAngleY = currentCamParam.GetYAngle (hRatio, 0f);
				zoomVec.z = -currentCamParam.GetZoomDist (hRatio, 0f);
			}
			else
			{
				destAngleX = currentCamParam.GetXAngle (hRatio, vRatio);
				destAngleY = currentCamParam.GetYAngle (hRatio, vRatio);
				zoomVec.z = -currentCamParam.GetZoomDist (hRatio, vRatio);
			}

			traceRotation = Quaternion.Euler (destAngleX, destAngleY, 0f);
			camDestPosition = tracePosition + traceRotation * zoomVec;

			camTrans.position = Vector3.SmoothDamp (camTrans.position, camDestPosition, ref camMoveVelocity, cameraMoveSmoothTime);
			curAngleX = Mathf.SmoothDampAngle (curAngleX, destAngleX, ref xAngleVelocity, xAngleSmoothTime);
			curAngleY = Mathf.SmoothDampAngle (curAngleY, destAngleY, ref yAngleVelocity, yAngleSmoothTime);
			camTrans.rotation = Quaternion.Euler (curAngleX, curAngleY, 0f);
		}

		void OnDrawGizmosSelected ()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube (playerProjectPosition, Vector3.one);
			Gizmos.color = Color.red;
			Gizmos.DrawCube (tracePosition, Vector3.one);
		}

		#endregion



		#region Util Methods

		/// <summary>
		/// 更新摄像机，由Gameplay中的LateUpdate调用
		/// </summary>
		/// <param name="bridge">Bridge.</param>
		/// <param name="_hRatio">H ratio.</param>
		/// <param name="_vRatio">V ratio.</param>
		public void UpdateCamera (LevelBridge bridge, float hRatio, float vRatio)
		{
			if (manuallyControl && Application.platform == RuntimePlatform.WindowsEditor)
				return;

			if (bridge != currentLevelBridge)
				currentLevelBridge = bridge;

			horizonRatio = hRatio;
			verticalRatio = vRatio;
		}

		public void StartFollow (LevelBridge levelBridge)
		{
			currentLevelBridge = levelBridge;
			currentCamParam = parameters [(int)currentLevelBridge.LevelType];
		}

		public void ToNextBridge ()
		{
			if (currentLevelBridge.next != null)
				StartFollow (currentLevelBridge.next);
		}

		public void ToPrevBridge ()
		{
			if (currentLevelBridge.prev != null)
				StartFollow (currentLevelBridge.prev);
		}

		#endregion
	}
}

