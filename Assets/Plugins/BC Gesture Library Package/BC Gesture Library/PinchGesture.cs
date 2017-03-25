using UnityEngine;
using System;

public class PinchGesture : BaseGesture
{
	public enum PinchAction {Both, Close, Open}
	public enum PinchDirection
	{
		All, // used for definition, never returned as direction
		Vertical,
		LeftDiagonal,
		Horizontal,
		RightDiagonal
	}
	
	public bool doPinch = true;
	public bool keepAspectRatio = false;
	public FingerLocation fingerLocation = FingerLocation.Over;
	public PinchAction pinchAction = PinchAction.Both;
	public PinchDirection restrictDirection = PinchDirection.All;
	public float pinchScaleFactor = 0.01f;
	
	public PinchDirection pinchDirection;
	public Finger pinchFinger1;
	public Finger pinchFinger2;
	public float pinchMagnitudeDelta;
	
	private bool pinching = false;
	private Vector3 originalScale;
	
	protected override void Start()
	{
		base.Start();
		originalScale = this.transform.localScale;
	}
	
	
	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddPinchCallback();
		FingerControl._delegatePinchBeginInternal += PinchGestureDownAndMovingBegin;
		FingerControl._delegatePinchMoveInternal += PinchGestureDownAndMovingMove;
		FingerControl._delegatePinchEndInternal += PinchGestureDownAndMovingEnd;
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemovePinchCallback();
		FingerControl._delegatePinchBeginInternal -= PinchGestureDownAndMovingBegin;
		FingerControl._delegatePinchMoveInternal -= PinchGestureDownAndMovingMove;
		FingerControl._delegatePinchEndInternal -= PinchGestureDownAndMovingEnd;
	}
	
	protected void PinchGestureDownAndMovingBegin(Finger finger1, Finger finger2, PinchDirection pinchDirectionIn, float magnitudeDelta)
	{
		if (!FingerActivated(fingerLocation, finger1.position)) {
			return;
		}
		if ((pinchAction == PinchAction.Close && magnitudeDelta > 0) ||
		    (pinchAction == PinchAction.Open && magnitudeDelta < 0)) {
			//Debug.Log("PinchGesture:PinchGestureDownAndMovingBegin pinch rejected action is " + pinchAction + " magnitudeDelta=" + magnitudeDelta);
			return;
		}
		
		
		pinching = true;
		pinchFinger1 = finger1;
		pinchFinger2 = finger2;
		pinchDirection = pinchDirectionIn;
		GestureMessage("GestureStartPinch");
		//Debug.Log("PinchGesture:PinchGestureDownAndMovingBegin move " + pinchDirection);
	}
	protected void PinchGestureDownAndMovingMove(Finger finger1, Finger finger2, PinchDirection pinchDirectionIn, float magnitudeDelta)
	{
		fingerCount = 2;
		if (!pinching) {
			return;
		}
		if (restrictDirection != PinchDirection.All && restrictDirection != pinchDirectionIn) {
			//Debug.Log("PinchGesture:PinchGestureDownAndMovingMove pinch cancelled direction is " + restrictDirection + " given direction=" + pinchDirectionIn);
			PinchGestureDownAndMovingEnd(finger1, finger2);
			return;
		}
		pinchFinger1 = finger1;
		pinchFinger2 = finger2;
		pinchMagnitudeDelta = magnitudeDelta;
		pinchDirection = pinchDirectionIn;
		GestureMessage("GestureMovePinch");
		//Debug.Log("PinchGesture:PinchGestureDownAndMovingMove move " + pinchDirection + ", magnitudeDelta=" + magnitudeDelta);
		TargetMovePinch ();
	}
	protected void PinchGestureDownAndMovingEnd(Finger finger1, Finger finger2)
	{
		if (!pinching) {
			return;
		}
		pinching = false;
		pinchFinger1 = finger1;
		pinchFinger2 = finger2;
		GestureMessage("GestureEndPinch");
	}
		
	public static PinchDirection CalculatePinchDirection(Vector2 position1, Vector2 position2)
	{
		if (position1.y >  position2.y) {
			Vector2 tempPos = position2;
			position2 = position1;
			position1 = tempPos;
		}
		Vector2 positionDelta = position2 - position1;
		
		if ( Mathf.Abs(positionDelta.y) < 50f) {
			return PinchDirection.Horizontal;
		}
		else if ( Mathf.Abs(positionDelta.x) < 50f) {
			return PinchDirection.Vertical;
		}
		
		if (position1.x > position2.x) {
			return PinchDirection.LeftDiagonal;
		}
		
		return PinchDirection.RightDiagonal;
	}
	
	private void TargetMovePinch ()
	{
		if (pinching && doPinch) {
			// change the scale of the target based on the pinch delta value
			if (keepAspectRatio) {
				ScaleTargetXY(pinchMagnitudeDelta, pinchMagnitudeDelta);
			}
			else if (pinchDirection == PinchDirection.Vertical) {
				ScaleTargetY(pinchMagnitudeDelta);
			}
			else if (pinchDirection == PinchDirection.Horizontal) {
				ScaleTargetX(pinchMagnitudeDelta);
			}
			else {
				ScaleTargetXY(pinchMagnitudeDelta, pinchMagnitudeDelta);
			}
		}
	}
	
	public void RestoreObject()
	{
		if (targetCollider != null) {
			targetCollider.gameObject.transform.localScale = originalScale;
		}
	}
	
	Vector3 vector31X = new Vector3(1f, 0f, 0f);
	Vector3 vector31Y = new Vector3(0f, 1f, 0f);
	Vector3 vector31Z = new Vector3(0f, 0f, 1f);
	
	private void ScaleTargetXY(float scaleValueX, float scaleValueY)
	{
		float scaleByX = scaleValueX * pinchScaleFactor;
		float scaleByY = scaleValueY * pinchScaleFactor;
		if (scaleByX < 0f && targetCollider.gameObject.transform.localScale.x <= (scaleByX* -1f)) {
			scaleByX = 0;
		}
		if (scaleByY < 0f && targetCollider.gameObject.transform.localScale.y <= (scaleByY* -1f)) {
			scaleByY = 0;
		}
		Vector3 scaleVector = (vector31X * scaleByX) + (vector31Y * scaleByY);
		//Debug.Log("PinchGesture:ScaleTargetXY scaleBy=" + scaleByX + ", " + scaleByY + "  " + scaleVector + ", scaleVal=" + scaleValueX + ", " + scaleValueY);
		targetCollider.gameObject.transform.localScale +=  scaleVector;
	}
	private void ScaleTargetX(float scaleValue)
	{
		float scaleBy = scaleValue * pinchScaleFactor;
		if (scaleValue < 0f && targetCollider.gameObject.transform.localScale.x <= (scaleBy* -1f)) {
			return;
		}
		//Debug.Log("PinchGesture:ScaleTargetY scaleBy=" + scaleBy + "  " + (vector31X * scaleBy) + ", scaleVal=" + scaleValue);
		targetCollider.gameObject.transform.localScale += vector31X * scaleBy;
	}
	
	private void ScaleTargetY(float scaleValue)
	{
		float scaleBy = scaleValue * pinchScaleFactor;
		if (scaleValue < 0f && targetCollider.gameObject.transform.localScale.y <= (scaleBy* -1f)) {
			return;
		}
		//Debug.Log("PinchGesture:ScaleTargetY scaleBy=" + scaleBy + "  " + (vector31Y * scaleBy) + ", scaleVal=" + scaleValue);
		targetCollider.gameObject.transform.localScale += vector31Y * scaleBy;
	}
	private void ScaleTargetZ(float scaleValue)
	{
		float scaleBy = scaleValue * pinchScaleFactor;
		if (scaleValue < 0f && targetCollider.gameObject.transform.localScale.z <= (scaleBy* -1f)) {
			return;
		}
		//Debug.Log("PinchGesture:ScaleTargetZ scaleBy=" + scaleBy + "  " + (vector31Z * scaleBy) + ", scaleVal=" + scaleValue);
		targetCollider.gameObject.transform.localScale += vector31Z * scaleBy;
	}
	

}