using UnityEngine;
using System;
using System.Collections.Generic;

public class LongPressGesture : BaseGesture
{
	public FingerLocation pressLocation = FingerLocation.Over;
	public float longPressTime = 1.5f;
	public float maxPressDistance = 20f;
	public FingerCountRestriction restrictFingerCount = FingerCountRestriction.Any;
	
	public float timeDifference;
	
	private List<Finger> fingersUsed = new List<Finger>();
	private bool possiblePress = false;
	private float lastStartTime = 0f;
	
	protected override void EnableGesture()
	{
		base.EnableGesture();
		possiblePress = false;
		FingerControl._delegateFingerDownInternal += GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal += GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal += GestureDownAndMovingEnd;
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		possiblePress = false;
		FingerControl._delegateFingerDownInternal -= GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal -= GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal -= GestureDownAndMovingEnd;
	}
			
	protected void GestureDownAndMovingBegin(Finger fingerIn)
	{
		//Debug.Log("LongPressGesture:GestureDownAndMovingBegin " + fingers.Count);
		CleanFingers(fingersUsed);
		if (fingersUsed.Count == 0) {
			//Debug.Log("LongPressGesture:GestureDownAndMovingBegin possiblePress");
			possiblePress = true;
			timeDifference = 0;
		}
		if (!fingersUsed.Contains(fingerIn)) {
			fingersUsed.Add(fingerIn);
			lastStartTime = Time.time;
			if (!FingerActivated(pressLocation, fingerIn.position)) {
				//Debug.Log("LongPressGesture:GestureDownAndMovingBegin FingerActivated falsed possiblePress is false");
				possiblePress = false;
			}
			
		}
	}
	protected void GestureDownAndMovingMove(Finger fingerIn)
	{
		if (!possiblePress || !distanceGood(finger.position, finger.startPosition)) {
			//Debug.Log("LongPressGesture:GestureDownAndMovingMove possiblePress is false");
			return;
		}
		CheckLongPress();
	}
	protected void GestureDownAndMovingEnd(Finger fingerIn)
	{
		//Debug.Log("LongPressGesture:GestureDownAndMovingEnd");
		if (fingersUsed.Contains(fingerIn)) {
			fingersUsed.Remove(fingerIn);
			//Debug.Log("LongPressGesture:GestureDownAndMovingEnd Remove finger " + fingers.Count);
		}
		CleanFingers(fingersUsed);
		distanceGood(fingerIn.endPosition, fingerIn.startPosition);
		possiblePress = false;
	}
	
	
	void Update()
	{
		if (!possiblePress) {
			return;
		}
		
		float timeDiff = Time.time - lastStartTime;
		if (timeDiff >= longPressTime) {
			//Debug.Log("LongPressGesture:Update call CheckLongPress, time is over " + timeDiff);
			CheckLongPress();
		}
	}
	
	protected void CheckLongPress()
	{
		if (!possiblePress) {
			//Debug.Log("LongPressGesture:CheckLongPress possiblePress false ");
			return;
		}
		
		float timeDiff = Time.time - lastStartTime;
		//Debug.Log("LongPressGesture:CheckLongPress timeDifference=" + timeDiff);
		if (timeDiff >= longPressTime) {
			if (!FingerCountGood(fingersUsed.Count, restrictFingerCount) ) {
				//Debug.Log("LongPressGesture:CheckLongPress  FingerCountGood failed " + fingers.Count + " for " + restrictFingerCount);
				possiblePress = false;
				return;
			}
			//Debug.Log("LongPressGesture:CheckLongPress  success");
			finger = fingersUsed[0];
			fingerCount = fingersUsed.Count;
			timeDifference = timeDiff;
			
			GestureMessage("GestureLongPress");
			possiblePress = false;
		}
	}
	
	private bool distanceGood(Vector2 point1, Vector2 point2)
	{
		//Debug.Log("LongPressGesture:distanceGood compare " + point1 + " verses " + point2);
		Vector2 diffVect = point1 - point2;
		if (diffVect.magnitude > maxPressDistance) {
			//Debug.Log("LongPressGesture:distanceGood moved possiblePress set false is " + diffVect.magnitude + " away");
			possiblePress = false;
			return false;
		}
		return true;
	}
	

	public void LongPressDoneCallBack(Finger finger)
	{
	}
	 
}