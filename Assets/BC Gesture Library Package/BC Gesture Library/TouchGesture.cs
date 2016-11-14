using UnityEngine;
using System;

public class TouchGesture : BaseGesture
{
	public FingerLocation startsOnObject = FingerLocation.Always;
	public FingerLocation movesOnObject = FingerLocation.Always;
	public FingerLocation endsOnObject = FingerLocation.Always;
	public FingerCountRestriction restrictFingerCount;	
	
	public bool averagePoint = true;
	public bool isDown = false;
	public bool[] isActives = new bool[5];
	public Vector2 touchPositionStart;
	public Vector2 touchPosition;
	public Vector2 touchPositionEnd;
	public float touchMagnitude;
	
	private bool averageActive = false;
	private bool sentSomething = false;
	private bool gestureIsGoing = false;
	
	public bool isActive { 
		get 
		{
			if (averageActive) return averageActive;
			
			for (int i = 0; i < fingerCount; i++) {
				if (!isActives[i]) {
					return false;
				}
			}
			return true;
		} 
	}
	
	protected override void Start()
	{
		base.Start();
	}
	
	protected override void EnableGesture()
	{
		//Debug.Log("TouchGesture:EnableGesture they are HERE " + baseId);
		base.EnableGesture();
		FingerControl._delegateFingerDownInternal += TouchGestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal += TouchGestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal += TouchGestureDownAndMovingEnd;
	}
	
	protected override void DisableGesture()
	{
		//Debug.Log("TouchGesture:DisableGesture they are GONE " + baseId);
		base.DisableGesture();
		FingerControl._delegateFingerDownInternal -= TouchGestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal -= TouchGestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal -= TouchGestureDownAndMovingEnd;
	}
	protected void TouchGestureIsDown(Finger fingerIn, bool isDownIn)
	{
		if (isDown == isDownIn) {
			return;
		}
		//Debug.Log("TouchGesture:TouchGestureIsDown " + isDownIn + " actives " + ActiveCount() + " now is " + isDown);
		if (!isDownIn) {
			if (ActiveCount() == 0) {
				//Debug.Log("TouchGesture:TouchGestureIsDown set isDown false ");
				isDown = false;
			}
		}
	}
	
	protected void TouchGestureDownAndMovingBegin(Finger fingerIn)
	{

		sentSomething = false;
		finger = fingerIn;
		touchPosition =  CalcPosForAllFingers(fingerIn.startPosition);
		touchPositionStart = touchPosition;
		fingerCount = ActiveCount();
		//Debug.Log("TouchGesture:TouchGestureDownAndMovingBegin fingerCount=" + fingerCount + " isDown " + isDown);
			
		if (FingerActivated(startsOnObject, averagePoint ? touchPosition : fingerIn.startPosition)
		&& FingerCountGood(fingerCount, restrictFingerCount)) {
			isActives[fingerIn.Index()] = true;
			averageActive = averagePoint ? true : false;
			if (!isDown) {
				//Debug.Log("TouchGesture:TouchGestureDownAndMovingBegin send GestureStartTouch");
				GestureMessage("GestureStartTouch");
				sentSomething = true;
				gestureIsGoing = true;
			}
		}
		else {
			averageActive = false;
			isActives[fingerIn.Index()] = false;
		}
		isDown = true;
	}
	protected void TouchGestureDownAndMovingMove(Finger fingerIn)
	{
		touchPosition =  CalcPosForAllFingers(fingerIn.position);
		fingerCount = ActiveCount();
		if (gestureIsGoing && FingerActivated(movesOnObject, averagePoint ? touchPosition : fingerIn.position)
		&& FingerCountGood(fingerCount, restrictFingerCount)) {
			isActives[finger.Index()] = true;
			averageActive = averagePoint ? true : false;
			finger = fingerIn;
			GestureMessage("GestureMoveTouch");
			sentSomething = true;
		}
		else {
			averageActive = false;
			isActives[finger.Index()] = false;
		}
	}
	protected void TouchGestureDownAndMovingEnd(Finger fingerIn)
	{
		isActives[fingerIn.Index()] = false;
		fingerCount = ActiveCount();
		//Debug.Log("TouchGesture:TouchGestureDownAndMovingEnd fingerCount=" + fingerCount + " isDown " + isDown);
		if (fingerCount < 2) {
			averageActive = false;
		}
		if (fingerCount == 0) {
			isDown = false;
			touchPositionEnd = touchPosition;
			Vector2 diff = touchPositionEnd - touchPositionStart;
			touchMagnitude = Mathf.Abs(diff.magnitude);
		}
		else {
			//Debug.Log("TouchGesture:TouchGestureDownAndMovingEnd no end sent count is " + fingerCount);
			return;
		}
		if (FingerActivated(endsOnObject, fingerIn.endPosition) && sentSomething ) {
			finger = fingerIn;
			//Debug.Log("TouchGesture:TouchGestureDownAndMovingEnd Sent GestureEndTouch");
			GestureMessage("GestureEndTouch");
		}
		sentSomething = false;
		gestureIsGoing = false;
	}

}