using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MyCompany.Common.Input;

namespace MyCompany.MyGame.UI
{

	public class MoveStick : MonoBehaviour
	{
		public RectTransform stickTrans;
		public RectTransform connectBarTrans;

		public float dragRange = 100f;

		public float HorizonMove{ get; private set; }

		RectTransform thisTrans;
		Vector2 startTouchPos;
		Vector2 canvasSize;

		readonly Vector2 pivotLeft = new Vector2 (0f, 0.5f);
		readonly Vector2 pivotRight = new Vector2 (1f, 0.5f);

		TouchHandler stickTouch;

		void Awake ()
		{
			thisTrans = transform as RectTransform;

			SetupTouch ();
			InputManager.Instance.onStickLeftChange += OnStickAreaChange;

			RectTransform canvasTrans = thisTrans.root as RectTransform;
			canvasSize = canvasTrans.GetComponent<CanvasScaler> ().referenceResolution;
			canvasSize.y = canvasSize.x / Screen.width * Screen.height;
			Deactive ();
		}

		void SetupTouch ()
		{
			stickTouch = InputManager.Instance.StickTouch;
			stickTouch.onTouchHold += OnStickMove;
			stickTouch.onTouchEnd += OnStickCancle;
		}

		void OnStickMove (TouchHandler touchHandler)
		{
			if (touchHandler.Moved)
				Show (touchHandler.startPos, touchHandler.Position);
		}

		void OnStickCancle (TouchHandler touchHandler)
		{
			gameObject.SetActive (false);
			HorizonMove = 0f;
		}

		void OnStickAreaChange (bool stickLeft)
		{
			stickTouch.onTouchHold -= OnStickMove;
			stickTouch.onTouchEnd -= OnStickCancle;
			SetupTouch ();
		}

		void Show (Vector2 startPos, Vector2 screenPos)
		{
			if (!gameObject.activeSelf)
			{
				thisTrans.anchoredPosition = startTouchPos = ScreenPosToAnchorPos (startPos);
				gameObject.SetActive (true);
			}

			Vector2 stickAnchorPos = ScreenPosToAnchorPos (screenPos);

			Vector2 offset = stickAnchorPos - startTouchPos;
			offset.x = Mathf.Clamp (offset.x, -dragRange, dragRange);
			offset.y = 0f;
			stickTrans.anchoredPosition = offset;

			connectBarTrans.pivot = offset.x < 0 ? pivotRight : pivotLeft;
			connectBarTrans.sizeDelta = new Vector2 (Mathf.Abs (offset.x), connectBarTrans.sizeDelta.y);

			HorizonMove = Mathf.Clamp (offset.x / dragRange, -1f, 1f);
		}

		public void Deactive ()
		{
			gameObject.SetActive (false);
			HorizonMove = 0f;
		}

		Vector2 ScreenPosToAnchorPos (Vector2 screenPos)
		{
			Vector2 anchorPos = new Vector2 (screenPos.x / Screen.width * canvasSize.x - canvasSize.x * 0.5f,
				                    screenPos.y / Screen.height * canvasSize.y - canvasSize.y * 0.5f);
			return anchorPos;
		}
	}
}
