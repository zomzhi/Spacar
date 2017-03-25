using UnityEngine;
using System;

public class RotateGesture : BaseGesture
{	
	public enum RotateAxis {X, Y, Z}
	
	public bool doRotate = true;
	public float minSqrDistanceToCenter = 5f;
	public FingerLocation fingerLocation = FingerLocation.Over;
	public RotateAxis rotateAxis = RotateAxis.Z;
	public FingerCountRestriction restrictFingerCount = FingerCountRestriction.Any;
	
	public Finger rotateFinger1;
	public Finger rotateFinger2;
	public float rotationAngleDelta;
		
	private bool rotating2Finger = false;
	private Quaternion originalRotation;
	public Vector2 touchPosition;
	public bool isDown = false;
	
	protected override void Start()
	{
		base.Start();
		originalRotation = this.transform.rotation;
	}
		
	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddRotateCallback();
		FingerControl._delegateRotateBeginInternal += RotateGestureDownAndMovingBegin;
		FingerControl._delegateRotateMoveInternal += RotateGestureDownAndMovingMove;
		FingerControl._delegateRotateEndInternal += RotateGestureDownAndMovingEnd;
		FingerControl._delegateFingerDownInternal += GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal += GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal += GestureDownAndMovingEnd;
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveRotateCallback();
		FingerControl._delegateRotateBeginInternal -= RotateGestureDownAndMovingBegin;
		FingerControl._delegateRotateMoveInternal -= RotateGestureDownAndMovingMove;
		FingerControl._delegateRotateEndInternal -= RotateGestureDownAndMovingEnd;
		FingerControl._delegateFingerDownInternal -= GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal -= GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal -= GestureDownAndMovingEnd;
	}
	
	protected void RotateGestureDownAndMovingBegin(Finger finger1, Finger finger2)
	{
		//Debug.Log("RotateGesture:RotateGestureDownAndMovingBegin magnitudeDelta=" + magnitudeDelta);
		if (!FingerActivated(fingerLocation, finger1.position)) {
			return;
		}
				
		rotating2Finger = true;
		rotateFinger1 = finger1;
		rotateFinger2 = finger2;
		GestureMessage("GestureStartRotate");
	}
	protected void RotateGestureDownAndMovingMove(Finger finger1, Finger finger2, float rotationAngleDeltaIn)
	{
		if (!rotating2Finger) {
			return;
		}
		fingerCount = 2;
		if (!FingerCountGood(fingerCount, restrictFingerCount)) {
			return;
		}
		rotateFinger1 = finger1;
		rotateFinger2 = finger2;
		rotationAngleDelta = rotationAngleDeltaIn;
		//Debug.Log("RotateGesture:RotateGestureDownAndMovingMove angle=" + rotationAngleDelta);
		Rotate(rotationAngleDelta);
	}
	
	private void Rotate(float angle)
	{
		if (doRotate && targetCollider != null) {
			switch(rotateAxis) {
			case RotateAxis.X:
       			targetCollider.gameObject.transform.Rotate(angle, 0f, 0f);
				break;
			case RotateAxis.Y:
       			targetCollider.gameObject.transform.Rotate(0f, angle, 0f);
				break;
			case RotateAxis.Z:
       			targetCollider.gameObject.transform.Rotate(0f, 0f, angle);
				break;
			}
			GestureMessage("GestureMoveRotate" );
		}
	}
	
	protected void RotateGestureDownAndMovingEnd(Finger finger1, Finger finger2)
	{
		if (!rotating2Finger) {
			return;
		}
		rotating2Finger = false;
		rotateFinger1 = finger1;
		rotateFinger2 = finger2;
		GestureMessage("GestureEndRotate");
	}
	
	
	protected void GestureDownAndMovingBegin(Finger fingerIn)
	{
		Bounds bounds = GetBounds();
		Vector3 distVect = Camera.main.transform.position - bounds.center;		
		Vector3 worldPosition = ScreenToWorldPosition(CalcPosForAllFingers(fingerIn.startPosition), Mathf.Abs(distVect.magnitude));
		Vector3 startDelta = bounds.center - worldPosition;
		startDir = startDelta.normalized;

		fingerCount = ActiveCount();
		//Debug.Log("TouchGesture:GestureDownAndMovingBegin fingerCount=" + fingerCount + " isDown " + isDown);
			
		isDown = true;
	}
	
	Vector3 startDir;
	Vector3 prevCenter;
	Vector3 prevPosition;
	bool rotatingAround = false;
	
	protected void GestureDownAndMovingMove(Finger fingerIn)
	{
		fingerCount = ActiveCount();
		
		if (!FingerCountGood(fingerCount, restrictFingerCount) || !FingerActivated(fingerLocation)) {
			return;
		}
	
		if (fingerCount == 2 && rotating2Finger) {
			return;
		}
			    	
		if (targetCollider != null) {
			Vector2 nowPosition =  CalcPosForAllFingers(fingerIn.position);
			Bounds bounds = GetBounds();
			if (bounds == emptyBounds) {
				return;
			}
			Vector3 distVect = Camera.main.transform.position - bounds.center;
			Vector3 worldPosition = ScreenToWorldPosition(nowPosition, Mathf.Abs(distVect.magnitude));		
			Vector3 relativeDir = worldPosition - bounds.center;
			//Debug.Log("RotateGesture:GestureDownAndMovingMove " + relativeDir.sqrMagnitude + " verses " + minSqrDistanceToCenter);
			if (relativeDir.sqrMagnitude < minSqrDistanceToCenter) {
				return;
			}
			Vector3 prevDelta = prevPosition - prevCenter;
			Vector3 curDelta = worldPosition - bounds.center;
			float magnitudeDelta = curDelta.magnitude - prevDelta.magnitude;
			
			if (Mathf.Abs (magnitudeDelta) < FingerControl.pinchRotateMinimumMoveDifference) {
				return;
			}
			//Debug.Log("RotateGesture:GestureDownAndMovingMove worldPosition=" + worldPosition + " relativeDir=" + relativeDir + "    bounds=" + bounds + " sqrMagnitude=" + relativeDir.sqrMagnitude);
			
			Vector3 curDir = curDelta.normalized;
			
			float angle;
			if (!rotatingAround) {
				rotatingAround = true;
				
				// check if we went past the minimum rotation amount threshold
				angle = Mathf.Rad2Deg * FingerControl.SignedAngle(startDir, curDir);
				
				if (!rotating2Finger) {
					GestureMessage("GestureStartRotate");
				}
			}
			else {
				angle = Mathf.Rad2Deg * FingerControl.SignedAngle (prevDelta.normalized, curDir);
				Rotate(angle);
			}
			
			rotationAngleDelta = angle;
			rotateFinger1 = fingerIn;
			rotateFinger2 = null;
			//Debug.Log("RotateGesture:GestureDownAndMovingMove angle=" + rotationAngleDelta + " fingers " + fingerCount);
		
			prevPosition = worldPosition;
			prevCenter = bounds.center;
		}
		    
	}
	protected void GestureDownAndMovingEnd(Finger fingerIn)
	{
		fingerCount = ActiveCount();
		if (fingerCount == 0) {
			isDown = false;
			rotating2Finger = false;
			rotatingAround = false;
			rotateFinger1 = fingerIn;
			rotateFinger2 = null;
			GestureMessage("GestureEndRotate");
		}
	}
	
	public void RestoreObject()
	{
		this.transform.rotation = originalRotation;
	}
}