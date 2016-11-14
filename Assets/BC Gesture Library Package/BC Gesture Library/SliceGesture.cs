using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SliceGesture : BaseGesture
{
	public SwipeGesture.SwipeDirection restrictDirection;
	
	public SwipeGesture.SwipeDirection sliceDirection;
	public Vector3 sliceStartPosition;
	public Vector3 sliceEndPosition;
	
	private bool slicing = false;
	private bool sliced = false;
	public SwipeSegment swipeSegment;
	
	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddSwipeCallback();
		FingerControl._delegateSwipeInternal += SliceGestureOnSwipe;
		FingerControl._delegateDownAndMovingBeginInternal += SliceGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal += SliceGestureDownAndMovingMove;
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveSwipeCallback();
		FingerControl._delegateSwipeInternal -= SliceGestureOnSwipe;
		FingerControl._delegateDownAndMovingBeginInternal -= SliceGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal -= SliceGestureDownAndMovingMove;
	}
	
	protected void SliceGestureOnSwipe(Finger[] fingersIn,  SwipeSegmentList[] segmentsList, int fingerCount)		
	{
		if (fingerCount > 1) {
			return;
		}
		finger = fingersIn[0];
		SwipeSegmentList segments = segmentsList[0];
		
		//Debug.Log("SliceGesture:SliceGestureOnSwipe segments count " + segments.Count);
		if (segments.Count	!= 1) {
			//Debug.Log("SliceGesture:SliceGestureOnSwipe too many segments " + segments.Count + " try to combine");
			segments = FingerControl.Factory().TryToMakeOneSegment(segments);
			if (segments.Count != 1) {
				//Debug.Log("SliceGesture:SliceGestureOnSwipe slice rejected for wrong segment count " + segments.Count);
				return;
			}
		}
		
		swipeSegment = segments[0];
			
		if (sliced && SwipeGesture.DirectionMatches(swipeSegment.direction, restrictDirection)) {
			//Debug.Log("SliceGesture:SliceGestureOnSwipe sent");
			sliceDirection = swipeSegment.direction;
			GestureMessage("GestureSlice");
		}
			 
		sliced = false;		
	}
	protected void SliceGestureDownAndMovingBegin(Finger finger)
	{
		sliced = false;		
		slicing = false;
	}
	
	protected void SliceGestureDownAndMovingMove(Finger fingerIn, Vector2 fingerPos, Vector2 delta)
	{
		if (this.IsOnObject(fingerPos)) {
			if (!slicing && !sliced) {
				slicing = true;
				finger = fingerIn;
				sliceStartPosition = ScreenToWorldPosition(fingerPos);
			}
		}
		else {
			if (slicing) {
				sliceEndPosition = ScreenToWorldPosition(fingerPos);
				slicing = false;
				sliced = true;
			}
		}
	}

}