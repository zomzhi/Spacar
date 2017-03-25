
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SwipeGesture : BaseGesture
{
	public enum SwipeDirection
	{
		Any, // used for definition of any direction, never returned as direction
		LeftDiagonal, // used for definition of Minus45 or Plus135, never returned as direction
		RightDiagonal, // used for definition of Plus45 or Minus135, never returned as direction
		Vertical, // used for definition of Up or Down, never returned as direction
		Horizontal, // used for definition of Left or Right, never returned as direction
		AnyCross, // used for definition of either diagonal, never returned as direction
		AnyPlus, // used for definition of Left, Right, Up or Down, never returned as direction
		Up,
		Plus45,
		Right,
		Plus135,
		Down,
		Minus135,
		Left,
		Minus45,
		None // used to no match any
	}

	public SwipeDirection restrictDirection;
	public FingerCountRestriction restrictFingerCount;	
	public FingerLocation startsOnObject = FingerLocation.Always;
	public FingerLocation movesOnObject = FingerLocation.Always;
	public FingerLocation endsOnObject = FingerLocation.Always;
	public float minGestureLength = 75f;
	public float maxTime = 1.5f;
	
	public SwipeDirection swipeDirection;
	public int swipeFingerCount;
	public Vector2 swipePosition;
	public Vector2 startPosition;
	public Vector2 endPosition;
	
	//Particles
	public Transform directionParticle;
	
	private bool swiping = false;

		
	public delegate void SwipeCallBack (SwipeGesture gesture);
	
	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddSwipeCallback();
		FingerControl._delegateSwipeInternal += SwipeGestureOnSwipe;
		FingerControl._delegateSwipeMoveInternal += SwipeGestureOnSwipeMove;
		FingerControl._delegateSwipeCancelInternal += SwipeGestureOnSwipeCancel;
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveSwipeCallback();
		FingerControl._delegateSwipeInternal -= SwipeGestureOnSwipe;
		FingerControl._delegateSwipeMoveInternal -= SwipeGestureOnSwipeMove;
		FingerControl._delegateSwipeCancelInternal -= SwipeGestureOnSwipeCancel;
	}
	
	protected void SwipeGestureOnSwipeCancel ()
	{
		if (swiping) {
			GestureMessage("GestureSwipeEnd");
		}
	}
	protected void SwipeGestureOnSwipeMove (Finger fingerIn, SwipeGesture.SwipeDirection direction,  int fingerCount)
	{
		swiping = true;
		//Debug.Log("SwipeGesture:SwipeGestureOnSwipeMove " + direction + "  " + fingerCount);
		swipePosition =  CalcPosForAllFingers(fingerIn.position);
		if (FingerActivated(movesOnObject, fingerIn.position) && FingerCountGood(fingerCount, restrictFingerCount) &&
					DirectionMatches(direction, restrictDirection)) {
			swipeFingerCount = fingerCount;
			finger = fingerIn;
			swipeDirection = direction;
			GestureMessage("GestureSwipeMove");
		}
	}

	protected void SwipeGestureOnSwipe (Finger[] fingers,  SwipeSegmentList[] segmentsList, int fingersDown)
	{
		//Debug.Log("SwipeGesture:SwipeGestureOnSwipe fingers #=" + fingers.Length +"  segmentsList#=" + segmentsList.Length + ", fingersDown=" + fingersDown);
		
		swiping = false;
		
		float totalTime = Time.time - fingers[0].startTime;
		if (totalTime > maxTime) {
			//Debug.Log("SwipeGesture:SwipeGestureOnSwip swipe rejected for time " + totalTime + " is over " + maxTime);
			return;
		}
		
		try {
			for (int i = 0; i < fingersDown; i++) {
				if (segmentsList[i] == null || segmentsList[i].Count == 0) {
					continue;
				}
				SwipeSegmentList segments = segmentsList[i];
				if (segments.Count	!= 1) {
					segments = FingerControl.Factory().TryToMakeOneSegment(segments);
					if (segments.Count != 1) {
						//Debug.Log("SwipeGesture:SwipeGestureOnSwip swipe rejected for segment count " + segments.Count);
						return;
					}
				}
				//Debug.Log("SwipeGesture:SwipeGestureOnSwip set direction " + segments[0].direction);
				fingers[i].swipeDirection = segments[0].direction;
			}
			
			swipeDirection = fingers[0].swipeDirection;
			
			if (fingersDown >= 1) {
				Vector2 sumStartPos = new Vector2(0, 0);
				Vector2 sumEndPos = new Vector2(0, 0);
				int[] successes = new int[Enum.GetValues(typeof(SwipeDirection)).Length];
				for (int i = 0; i < successes.Length; i++) {
					successes[i] = 0;
				}
				int successCount = 0;
				for (int i = 0; i < fingersDown; i++) {
					//Debug.Log("SwipeGesture:SwipeGestureOnSwipe startsOnObject " + startsOnObject);
					if (!FingerActivated(startsOnObject, fingers[i].startPosition) || !FingerActivated(endsOnObject, fingers[i].endPosition)) {
						//Debug.Log("SwipeGesture:SwipeGestureOnSwipe swipe for position activation failure " + i + " starts " + fingers[i].startPosition + "  ends " + fingers[i].endPosition);
						return;
					}
					if (swipeDirection != fingers[i].swipeDirection) {
						
						if (FingerControl.FriendlySwipeDirections(swipeDirection, fingers[i].swipeDirection)) {
							successes[(int) fingers[i].swipeDirection]++;
							successCount++;
							//Debug.Log("SwipeGesture:SwipeGestureOnSwipe swipe direcion friendly " + i + " " + fingers[i].swipeDirection + " does not match " + swipeDirection);
							continue;
						}
						//Debug.Log("SwipeGesture:SwipeGestureOnSwipe swipe direcion failure " + i + " " + fingers[i].swipeDirection + " does not match " + swipeDirection);
						return;
					}
					successes[(int) swipeDirection]++;
					sumStartPos += fingers[i].startPosition;
					sumEndPos += fingers[i].endPosition;
				}
				
				if (successCount > 0) {
					int mostAt = 0;
					for (int i = 1; i < successes.Length; i++) {
						if (successes[mostAt] < successes[i]) {
							mostAt = i;
						}
					}
					swipeDirection = (SwipeDirection) mostAt;
					//Debug.Log("SwipeGesture:SwipeGestureOnSwipe most directions is  " + swipeDirection + " with " + successes[mostAt]);
				}
				startPosition = sumStartPos / (float)fingersDown;
				endPosition = sumEndPos / (float)fingersDown;
			}
			else {
				startPosition = fingers[0].startPosition;
				endPosition = fingers[0].endPosition;
			}
			
			Vector2 dist = endPosition - startPosition;
			//Debug.Log("SwipeGesture:SwipeGestureOnSwipe swipe length " + dist.magnitude + " on min " + minGestureLength);
			if (dist.magnitude < minGestureLength) {
				//Debug.Log("SwipeGesture:SwipeGestureOnSwipe swipe rejected on length " + dist.magnitude + " too short with min " + minGestureLength);
				return;
			}
			
			if (!FingerCountGood(fingersDown, restrictFingerCount)  || !DirectionMatches(swipeDirection, restrictDirection)) {
				//Debug.Log("SwipeGesture:SwipeGestureOnSwipe swipe for restrictFingerCount or  restrictDirection " + fingersDown + " " + swipeDirection);
				return;
			}
			
			swipeFingerCount = fingersDown;
			SetFingers(fingers, fingersDown);
			//Debug.Log("SwipeGesture:SwipeGestureOnSwipe GestureSwipe  " + swipeDirection);
			GestureMessage("GestureSwipe");
		}
		finally {
			GestureMessage("GestureSwipeEnd");
		}
	}

	
	public static bool DirectionMatches(SwipeDirection direction, SwipeDirection directionRestrict)
	{
		if (directionRestrict == SwipeDirection.Any || direction == directionRestrict ||
			(directionRestrict == SwipeDirection.LeftDiagonal && (direction == SwipeDirection.Minus45 || direction == SwipeDirection.Plus135)) ||
			(directionRestrict == SwipeDirection.RightDiagonal  && (direction == SwipeDirection.Plus45 || direction == SwipeDirection.Minus135)) ||
			(directionRestrict == SwipeDirection.Vertical  && (direction == SwipeDirection.Up  || direction == SwipeDirection.Down)) ||
			(directionRestrict == SwipeDirection.Horizontal  && (direction == SwipeDirection.Left || direction == SwipeDirection.Right)) ||
			(directionRestrict == SwipeDirection.AnyCross  && (direction == SwipeDirection.Minus45 || direction == SwipeDirection.Plus135 || direction == SwipeDirection.Plus45 || direction == SwipeDirection.Minus135)) ||
			(directionRestrict == SwipeDirection.AnyPlus  && (direction == SwipeDirection.Left || direction == SwipeDirection.Right || direction == SwipeDirection.Up  || direction == SwipeDirection.Down))
		     ) {
			return true;
		}
		return false;
	}
	
	public static SwipeDirection GetDirection(bool isForward, SwipeDirection direction)
	{
		if (isForward) {
			return direction;
		}
		else {
			return GetReverseDirection(direction);
		}
	}
	 	
	public static SwipeDirection GetReverseDirection(SwipeDirection direction)
	{
		switch (direction) {
		case SwipeDirection.Up:
			return SwipeDirection.Down;
		case SwipeDirection.Plus45:
			return SwipeDirection.Minus135;
		case SwipeDirection.Right:
			return SwipeDirection.Left;
		case SwipeDirection.Plus135:
			return SwipeDirection.Minus45;
		case SwipeDirection.Down:
			return SwipeDirection.Up;
		case SwipeDirection.Minus135:
			return SwipeDirection.Plus45;
		case SwipeDirection.Left:
			return SwipeDirection.Right;
		case SwipeDirection.Minus45:
			return SwipeDirection.Plus135;
		case SwipeDirection.Any:
		case SwipeDirection.LeftDiagonal:
		case SwipeDirection.RightDiagonal:
		case SwipeDirection.Vertical:
		case SwipeDirection.Horizontal:
		case SwipeDirection.AnyCross:
		case SwipeDirection.AnyPlus:
			return SwipeDirection.Any;
		}
		return SwipeDirection.Any;
	}



}
