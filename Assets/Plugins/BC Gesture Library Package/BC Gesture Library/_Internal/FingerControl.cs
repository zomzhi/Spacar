using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class FingerControl : MonoBehaviour
{

	// access to the singleton
	private static FingerControl _factory;
	public static FingerControl Factory ()
	{
		return _factory;
	}

	public abstract int FingerCount();

	private static bool possibleSwipe = false;
	private static int trackPinchCallbacks = 0;
	private static int trackRotateCallbacks = 0;
	private static int trackSwipeCallbacks = 0;
	public static bool trackPinch = false;
	public static bool trackRotate = false;
	public static bool trackSwipe = false;
	public static bool track2FingerGestures = false;

	public static float moveThresholdSquared = 25.0f;
	public static float swipeChangeMaxDistance = 50f;
	public float swipeChangeSecondaryMaxDistance = 100f;
	public float pinchDeltaScale = 1.0f;
	public float maxTapTime = 0.8f;
	public float minSwipeDistance = 2.0f;
	public float swipeDirectionTolerance = 0.2f;
	public float initializeLongPressTime = 1.5f;
	public float vertHorizHalfDegreeThreshold = 15f;
	
	private int activeFingers = 0;

	protected static List<BaseGesture> gestures = new List<BaseGesture> ();
	public Finger[] fingers;
	protected Vector2[,] previousPositions;
	protected const int  previousPositionsEnd = 15;
	protected const int  previousPositionsTurn = 4;
	protected bool previousPositionTurn = false;
	
	protected static float swpDirRightMin;
	protected static float swpDirRightMax;
	protected static float swpDirUpMin;
	protected static float swpDirUpMax;
	protected static float swpDirLeftMin;
	protected static float swpDirLeftMax;
	protected static float swpDirDownMin;
	protected static float swpDirDownMax;
	protected static SwipeGesture.SwipeDirection[,]  friendlySwipeDirections;


	public static void AddPinchCallback ()
	{
		trackPinchCallbacks++;
		trackPinch = true;
		track2FingerGestures = true;
	}
	public static void RemovePinchCallback ()
	{
		trackPinchCallbacks--;
		if (trackPinchCallbacks <= 0) {
			trackPinch = false;
			trackPinchCallbacks = 0;
			if (!trackRotate) {
				track2FingerGestures = false;
			}
		}
		
	}
	public static void AddSwipeCallback ()
	{
		trackSwipeCallbacks++;
		trackSwipe = true;
	}
	public static void RemoveSwipeCallback ()
	{
		trackSwipeCallbacks--;
		if (trackSwipeCallbacks <= 0) {
			trackSwipe = false;
			trackSwipeCallbacks = 0;
		}
		
	}
	
	public static void AddRotateCallback ()
	{
		trackRotateCallbacks++;
		trackRotate = true;
		track2FingerGestures = true;
	}
	public static void RemoveRotateCallback ()
	{
		trackRotateCallbacks--;
		if (trackRotateCallbacks <= 0) {
			trackRotate = false;
			trackRotateCallbacks = 0;
			if (!trackPinch) {
				track2FingerGestures = false;
			}
		}
	}

	// keep a list of gesture objects
	public static void AddGesture (BaseGesture gesture)
	{
		gestures.Add (gesture);
		gesture.StartGesture();
	}
	public static void RemoveGesture (BaseGesture gesture)
	{
		gestures.Remove (gesture);
		gesture.EndGesture();
	}
	public static void RemoveAllGestures ()
	{
		for (int i = 0; i < gestures.Count; i++) {
			gestures[i].EndGesture();
		}
		gestures.Clear ();
	}
	
	public static bool IsSwiping()
	{
		return possibleSwipe;
	}


	protected abstract Finger NewFinger (int index);
	public abstract bool IsFingers();


	protected virtual void Awake ()
	{
		_factory = this;
		
		fingers = new Finger[FingerCount()];
		for (int i = 0; i < fingers.Length; i++) {
			fingers[i] = NewFinger (i);
		}
		
		friendlySwipeDirections = new SwipeGesture.SwipeDirection[Enum.GetValues(typeof(SwipeGesture.SwipeDirection)).Length, 2];
		friendlySwipeDirections[(int) SwipeGesture.SwipeDirection.Any, 0] = SwipeGesture.SwipeDirection.Any;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Any, 1] = SwipeGesture.SwipeDirection.Any;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.None, 0] = SwipeGesture.SwipeDirection.None;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.None, 1] = SwipeGesture.SwipeDirection.None;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Up, 0] = SwipeGesture.SwipeDirection.Minus45;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Up, 1] = SwipeGesture.SwipeDirection.Plus45;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Plus45, 0] = SwipeGesture.SwipeDirection.Up;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Plus45, 1] = SwipeGesture.SwipeDirection.Right;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Right, 0] = SwipeGesture.SwipeDirection.Plus45;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Right, 1] = SwipeGesture.SwipeDirection.Plus135;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Plus135, 0] = SwipeGesture.SwipeDirection.Right;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Plus135, 1] = SwipeGesture.SwipeDirection.Down;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Down, 0] = SwipeGesture.SwipeDirection.Plus135;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Down, 1] = SwipeGesture.SwipeDirection.Minus135;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Minus135, 0] = SwipeGesture.SwipeDirection.Down;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Minus135, 1] = SwipeGesture.SwipeDirection.Left;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Left, 0] = SwipeGesture.SwipeDirection.Minus135;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Left, 1] = SwipeGesture.SwipeDirection.Minus45;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Minus45, 0] = SwipeGesture.SwipeDirection.Left;
		friendlySwipeDirections[(int)SwipeGesture.SwipeDirection.Minus45, 1] = SwipeGesture.SwipeDirection.Up;

		
		
	}
	protected virtual void Start ()
	{
		swpDirRightMin = vertHorizHalfDegreeThreshold;
		swpDirRightMax = 360f - vertHorizHalfDegreeThreshold;
		swpDirUpMin = 90f - vertHorizHalfDegreeThreshold;
		swpDirUpMax = 90f + vertHorizHalfDegreeThreshold;
		swpDirLeftMin = 180f - vertHorizHalfDegreeThreshold;
		swpDirLeftMax = 180f + vertHorizHalfDegreeThreshold;
		swpDirDownMin = 270f - vertHorizHalfDegreeThreshold;
		swpDirDownMax = 270f + vertHorizHalfDegreeThreshold;
		
		previousPositions = new Vector2[FingerCount(), 16];
	}
	
//	int debugUpdateCounter = 0;
	protected virtual void Update ()
	{
		
		//debugUpdateCounter++;
		//Debug.Log("FingerControl:Update Enter " + possibleSwipe + " " + debugUpdateCounter);
		// for re-compile
		if (fingers == null) {
			Debug.LogError ("FingerControl error, data has disappeared - need to restart");
			return;
		}
			
		activeFingers = 0;
		for (int i = 0; i < fingers.Length; i++) {
			Finger finger = fingers[i];
			if (finger.motionState != Finger.FingerMotionState.Inactive) {
				activeFingers++;
			}
		}
		int redoActiveFingers = 0;
		for (int i = 0; i < fingers.Length; i++) {
			Finger finger = fingers[i];
			UpdateFinger (finger);
			if (finger.motionState != Finger.FingerMotionState.Inactive) {
				redoActiveFingers++;
			}
		}
		//Debug.Log("FingerControl:Update Updated Fingers activeFingers " +  activeFingers + " " + debugUpdateCounter);
		activeFingers = redoActiveFingers;
		
		if (track2FingerGestures && activeFingers < 3) {
			UpdateTwoFingerGestures (activeFingers);
		}
		else if (activeFingers == 0) {
			AllFingersInactive();
		}
		//Debug.Log("FingerControl:Update Leave " + possibleSwipe + " " + debugUpdateCounter);
	}
	
	
	protected virtual void UpdateFinger (Finger finger)
	{
		if ((!finger.SetState () || finger.touchPhase == TouchPhase.Canceled) && !finger.isDown) {
			//Debug.Log("FingerControl:UpdateFinger no finger state");
			return;
		}
		
		if (finger.touchPhase == TouchPhase.Ended || finger.touchPhase == TouchPhase.Canceled) {
			//Debug.Log("FingerControl:UpdateFinger finger.touchPhase == " + finger.touchPhase);
			ProcessSwipeEnd (finger);				
			if (finger.hasMoved || finger.downAndMoving) {
				SendEventDownAndMovingEnd (finger);
			}

			else if (finger.motionState == Finger.FingerMotionState.Stationary) {
				StopStationary (finger, finger.position, true);
			}
			else if (finger.motionState == Finger.FingerMotionState.Moving) {
				StopMoving (finger, finger.position);
			}
			
			float elapsedTime = Time.time - finger.startTime;
			finger.Up();
			SendIsDown(finger, false);
			SendEventFingerUp (finger, elapsedTime);
		}

		else {
			// detect if it's the first frame the touch is down.
			//Debug.Log("FingerControl:UpdateFinger check for swipe "  + debugUpdateCounter);
			if (finger.touchPhase == TouchPhase.Began || (!finger.isDown && (finger.touchPhase == TouchPhase.Moved || finger.touchPhase == TouchPhase.Stationary))) {
				//Debug.Log("FingerControl:UpdateFinger phase == TouchPhase.Began || !finger.active, phase = " + finger.touchPhase + " " + finger.position);
				ProcessSwipeBegin(finger);
				finger.Down (finger.position);
				finger.motionState = Finger.FingerMotionState.Stationary;
				finger.onlyStationary = true;
				finger.touchPhase = TouchPhase.Stationary;
				SendIsDown(finger, true);
				SendEventFingerDown (finger);
			}
			
			else if (finger.touchPhase == TouchPhase.Moved) {
				//Debug.Log("FingerControl:UpdateFinger finger.touchPhase == TouchPhase.Moved ");
				SendEventFingerMoving(finger);
				// the finger has moved but the state has not yet been set to moving
				if (finger.motionState != Finger.FingerMotionState.Moving) {
					bool hasMoved = false;
					Vector2 moveStartPos = finger.position;
					
					// already moving
					if (finger.hasMoved) {
						hasMoved = true;
					}
					else {
						// check if finger has moved enough yet to count as a move
						Vector2 moveDelta = finger.position - finger.startPosition;
						if (moveDelta.sqrMagnitude > moveThresholdSquared) {
							hasMoved = true;
							moveStartPos = finger.startPosition;
						}
					}
					// set state to moving
					if (hasMoved) {
						if (finger.motionState == Finger.FingerMotionState.Stationary) {
							//Debug.Log("FingerControl:UpdateFinger TouchPhase.Moved StopStationary " + debugUpdateCounter);
							StopStationary (finger, finger.position, false);
						}
						finger.motionState = Finger.FingerMotionState.Moving;
						SendEventFingerMoveBegin (finger, moveStartPos);
						
						if (!finger.hasMoved) {
							finger.downAndMoving = true;
							finger.downAndMovingStartTime = Time.time;
							SendEventDownAndMovingBegin (finger);							
							finger.hasMoved = true;
						}
					}
					else {
						//Debug.Log("FingerControl:UpdateFinger TouchPhase.Moved FingerIsStationary " + debugUpdateCounter);
						FingerIsStationary (finger, finger.position);
					}
				}
				
				if (finger.hasMoved) {
					//Debug.Log("FingerControl:UpdateFinger TouchPhase.Moved finger.hasMoved " + debugUpdateCounter);
					for (int i = 1; i < 16; i++) {
						previousPositions[SegmentListFingerIndex(finger), i] = previousPositions[SegmentListFingerIndex(finger), i - 1];
					}
					previousPositions[SegmentListFingerIndex(finger), 0] = finger.position;
					if (finger.deltaPosition.sqrMagnitude > 0) {
						//Debug.Log("FingerControl:UpdateFinger TouchPhase.Moved ProcessSwipeMove " + debugUpdateCounter);
						ProcessSwipeMove (finger, finger.deltaPosition);
						if (finger.motionState == Finger.FingerMotionState.Moving) {
							//Debug.Log("FingerControl:UpdateFinger TouchPhase.Moved SendEventFingerMove " + debugUpdateCounter);
							SendEventFingerMove (finger, finger.position);
						}
						if (finger.downAndMoving) {
							//Debug.Log("FingerControl:UpdateFinger TouchPhase.Moved SendEventDownAndMoving"  + debugUpdateCounter);
							SendEventDownAndMoving (finger, finger.position, finger.deltaPosition);
							//Debug.Log("FingerControl:UpdateFinger Down and Move " + SegmentListFingerIndex(finger) + " " + finger.position);
						}
					}
				}
			}
			else if (finger.touchPhase == TouchPhase.Stationary) {
				//Debug.Log("FingerControl:UpdateFinger FingerIsStationary ");
				FingerIsStationary (finger, finger.position);
			}
			
		}
		//Debug.Log("FingerControl:UpdateFinger Leave " + debugUpdateCounter);
	}

	public float pinchDotProductVariance = -0.7f;
	// -1 is perfectly opposite in direction, 1 is 90 degress
	public float pinchMinimumDistance = 0.5f;
	public static float pinchRotateMinimumMoveDifference = 0.02f;
	public static float rotationDotProductVariance = -0.7f;
	// -1 is perfectly opposite in direction
	public static float minRotateAmount = 0.5f;

	private Finger[] twoFinger = { null, null };
	private bool pinching = false;
	private bool rotating = false;
	private void UpdateTwoFingerGestures (int activeFingerCount)
	{
		
		if (activeFingerCount == 2) {
			//Debug.Log("FingerControl:UpdateTwoFingerGestures " + activeFingerCount + ", trackPinch = " + trackPinch + ", pinching=" + pinching + ", rotating=" + rotating + " " + test);test++;
				
			if (!rotating || !pinching) {
				twoFinger[0] = fingers[0];
				twoFinger[1] = fingers[1];
			}
			Finger finger1 = twoFinger[0];
			Finger finger2 = twoFinger[1];
			
			if (!finger1.hasMoved || !finger2.hasMoved) {
				return;
			}
			
			if (finger1.touchPhase != TouchPhase.Moved || finger2.touchPhase != TouchPhase.Moved) {
				return;
			}
			
			Vector2 prev2FingerDelta = finger1.previousPosition - finger2.previousPosition;
			Vector2 cur2FingerDelta = finger1.position - finger2.position;
			float magnitudeDelta = cur2FingerDelta.magnitude - prev2FingerDelta.magnitude;
			
			if (Mathf.Abs (magnitudeDelta) < pinchRotateMinimumMoveDifference) {
				return;
			}
			
			float dotProduct = Vector2.Dot (finger1.deltaPosition.normalized, finger2.deltaPosition.normalized);
			
			if (trackRotate && dotProduct < rotationDotProductVariance) {
				Vector2 curDir = cur2FingerDelta.normalized;
				
				if (!rotating) {
					Vector2 startDelta = finger1.startPosition - finger2.startPosition;
					Vector2 startDir = startDelta.normalized;
					
					// check if we went past the minimum rotation amount threshold
					float angle = Mathf.Rad2Deg * SignedAngle (startDir, curDir);
					
					if (Mathf.Abs (angle) >= minRotateAmount) {
						rotating = true;
						SendEventRotateBegin (finger1, finger2);
						SendEventRotateMove (finger1, finger2, (Mathf.Sign (angle) - angle) * minRotateAmount);
					}
				}
				else {
					Vector2 prevDir = prev2FingerDelta.normalized;
					float angle = Mathf.Rad2Deg * SignedAngle (prevDir, curDir);
					SendEventRotateMove (finger1, finger2, angle);
				}
			}
			
			if (trackPinch && dotProduct < pinchDotProductVariance) {
				PinchGesture.PinchDirection pinchDirection = PinchGesture.CalculatePinchDirection (finger1.position, finger2.position);
				//Debug.Log("FingerControl:UpdateTwoFingerGestures " + pinchDirection +  " cur2FingerDelta=" + prev2FingerDelta);
				if (!pinching) {
					pinching = true;
					//Debug.Log("FingerControl:UpdateTwoFingerGestures begin" );
					SendEventPinchBegin (finger1, finger2, pinchDirection, magnitudeDelta);
				}
				else {
					//Debug.Log("FingerControl:UpdateTwoFingerGestures move" );
					SendEventPinchMove (finger1, finger2, pinchDirection, magnitudeDelta);
				}
			}
		}
		else {
			if (pinching) {
				SendEventPinchEnd (twoFinger[0], twoFinger[1]);
				pinching = false;
			}
			
			if (rotating) {
				SendEventRotateEnd (twoFinger[0], twoFinger[1]);
				rotating = false;
			}
			twoFinger[0] = null;
			twoFinger[1] = null;
		}
		
	}

	private void StopMoving (Finger finger, Vector2 fingerPos)
	{
		finger.motionState = Finger.FingerMotionState.Inactive;
		SendEventFingerMoveEnd (finger, fingerPos);
	}

	private void StopStationary (Finger finger, Vector2 fingerPos, bool ended)
	{
		finger.motionState = Finger.FingerMotionState.Inactive;
		if (!ended) {
			finger.onlyStationary = false;
		}
		SendEventFingerStationaryEnd (finger, fingerPos, Time.time - finger.timeSpentStationary);
	}

	protected void FingerIsStationary (Finger finger, Vector2 fingerPos)
	{
		
		if (finger.motionState != Finger.FingerMotionState.Stationary) {
			if (possibleSwipe || finger.hasMoved) {
				// ignore the stationary to avoid messing up a swipe
				return;
			}
			
			if (finger.motionState == Finger.FingerMotionState.Moving) {
				StopMoving (finger, fingerPos);
			}
			//Debug.Log("FingerControl:FingerIsStationary motionState was " + finger.motionState + " reset stationary");
			finger.motionState = Finger.FingerMotionState.Stationary;
			finger.timeSpentStationary = Time.time;
			finger.stationaryPosition = fingerPos;
			SendEventFingerStationaryBegin (finger, fingerPos);
		}
		
		float elapsedTime = Time.time - finger.timeSpentStationary;
		
		
		SendEventFingerStationary (finger, elapsedTime);
	}
	
	private void AllFingersInactive()
	{
		possibleSwipe = false;
		swipeFingerCount = 0;
		swipeFingersCount = 0;
		segmentLists = null;
		//Debug.Log("FingerControl:AllFingersInactive !possibleSwipe " + debugUpdateCounter);
	}
	
	private int swipeFingerCount = 0;
	private int swipeFingersCount = 0;
	private SwipeSegmentList[] segmentLists;
	
	private void ProcessSwipeBegin(Finger finger)
	{
		if (!trackSwipe) {
			//Debug.Log("FingerControl:ProcessSwipeBegin no swipes");
			return;
		}
		if (swipeFingerCount == 0 || segmentLists == null) {
			//Debug.Log("FingerControl:ProcessSwipeBegin new swipe");
			possibleSwipe = true;
			swipeFingersCount = 0;
			segmentLists = new SwipeSegmentList[FingerCount()];
			segmentLists[SegmentListFingerIndex(finger)] = new SwipeSegmentList();
			segmentLists[SegmentListFingerIndex(finger)].Add(new SwipeSegment(null));
			clearPreviousPositions(-1);
		}
		else {
			if (segmentLists[SegmentListFingerIndex(finger)] == null) {
				segmentLists[SegmentListFingerIndex(finger)] = new SwipeSegmentList();
			}
			segmentLists[SegmentListFingerIndex(finger)].Add(new SwipeSegment(null));
		}
		SwipeSegment segment = segmentLists[SegmentListFingerIndex(finger)][0];
		segment.startPosition = finger.startPosition;

		if (possibleSwipe) {
			swipeFingerCount++;
			swipeFingersCount++;
			//Debug.Log("FingerControl:ProcessSwipeBegin add another =" + swipeFingersCount);
		}
	}
	
	private int SegmentListFingerIndex(Finger finger)
	{
		if (FingerCount() > 1) {
			return finger.Index();
		}
		return 0;
	}
	
	private void clearPreviousPositions(int index)
	{
		if (index < 0) {
			for (int o = 0; o < FingerCount(); o++) {
				for (int i = 0; i < 16; i++) {
					//Debug.Log("FingerControl:clearPreviousPositions " + o + ", "  + i);
					previousPositions[o, i] = Vector2.zero;
				}
			}
		}
		else {
			for (int i = 0; i < 16; i++) {
				//Debug.Log("FingerControl:clearPreviousPositions " + o + ", "  + i);
				previousPositions[index, i] = Vector2.zero;
			}
		}
		previousPositionTurn = false;
	}

	
	private void CancelPossibleSwipe()
	{
		//Debug.Log("FingerControl:CancelPossibleSwipe");
		segmentLists = null;
		possibleSwipe = false;
		swipeFingerCount = 0;
		SendEventSwipeCancel();
	}
	
	private SwipeSegment getLastSegment(Finger finger)
	{
		if (segmentLists == null) {
			//Debug.Log("FingerControl:getLastSegment  *** not good *** segmentLists is null " + SegmentListFingerIndex(finger));
			return null;
		}
		SwipeSegmentList list = segmentLists[SegmentListFingerIndex(finger)];
		if (list == null || list.Count == 0) {
			//Debug.Log("FingerControl:getLastSegment  *** not good *** segmentList " + SegmentListFingerIndex(finger) +  (list == null ? " is null " : " is emptry"));
			ProcessSwipeBegin(finger);
			return getLastSegment(finger);
		}
		return list[list.Count - 1];
	}
	
	private void ProcessSwipeMove (Finger finger, Vector2 moveDelta)
	{	
		if (!possibleSwipe) {
			//Debug.Log("FingerControl:ProcessSwipeMove coming in with CancelPossibleSwipe " + SegmentListFingerIndex(finger) + ", possibleSwipe=" + possibleSwipe);
			CancelPossibleSwipe();
			return;
		}
		
		SwipeSegment currentSwipeSegment = getLastSegment(finger);
		//Debug.Log("FingerControl:ProcessSwipeMove finger=" + SegmentListFingerIndex(finger) + ", smoveDelta=" + moveDelta + " currentSwipeSegment.startPosition=" + currentSwipeSegment.startPosition + " finger.position=" + finger.position);
		
		SwipeGesture.SwipeDirection direction = GetSwipeDirection(currentSwipeSegment.startPosition, finger.position);
		//Debug.Log("FingerControl:ProcessSwipeMove direction=" + direction + ", segment Direction=" + currentSwipeSegment.direction);
//		SwipeGesture.SwipeDirection directionThis = GetSwipeDirection(finger.previousPosition, finger.position);
//		Debug.Log("FingerControl:ProcessSwipeMove direction=" + direction + ", directionThis=" + directionThis);
//		 
//		if (!FriendlySwipeDirections(direction, directionThis)) {
//			Debug.Log("FingerControl:ProcessSwipeMove CancelPossibleSwipe on FriendlySwipeDirections " + SegmentListFingerIndex(finger) + " direction " + direction + " with smaller " + directionThis);
//			CancelPossibleSwipe();
//			return;
//		}
		
		
		//Debug.Log("FingerControl:ProcessSwipeMove previousPositions=" + previousPositionsEnd + " " + previousPositions[SegmentListFingerIndex(finger), previousPositionsEnd]);
		if (previousPositions[SegmentListFingerIndex(finger), previousPositionsEnd] != Vector2.zero) {
			SwipeGesture.SwipeDirection directionEnd = GetSwipeDirection(previousPositions[SegmentListFingerIndex(finger), previousPositionsTurn], previousPositions[SegmentListFingerIndex(finger), 0]);
			if (directionEnd != currentSwipeSegment.direction) {
				//Debug.Log("FingerControl:ProcessSwipeMove  previousPositionTurn, new direction is " + directionEnd);
				direction = directionEnd;
				previousPositionTurn = true;
			}
		}
		
		//Debug.Log("FingerControl:ProcessSwipeMove currentSwipeSegment.direction=" + currentSwipeSegment.direction  + " direction " + direction);
		if (currentSwipeSegment.direction != direction || previousPositionTurn) {
			// the swipe direction not set yet, take it
			if (currentSwipeSegment.direction == SwipeGesture.SwipeDirection.None) {
				currentSwipeSegment.Initalize(finger, finger.startPosition, direction);
				//Debug.Log("FingerControl:ProcessSwipeMove " + SegmentListFingerIndex(finger) + " direction set from Any to " + currentSwipeSegment.direction);
				
				// the swipe direction change cancelling a chance of a swipe
			}
			else {
				Vector3 delta = finger.position - currentSwipeSegment.startPosition;
				float distance = delta.magnitude;
	
				//Debug.Log("FingerControl:ProcessSwipeMove distance=" + distance);
				if (distance < swipeChangeMaxDistance || (distance < swipeChangeSecondaryMaxDistance && FriendlySwipeDirections(currentSwipeSegment.direction, direction)) ) {
					//oldSwipeDirection = currentSwipeSegment.direction;
					//if (currentSwipeSegment.direction != direction) Debug.Log("FingerControl:ProcessSwipeMove under swipeChangeMaxDistance change direction to " + currentSwipeSegment.direction + ", distance=" + distance);
					currentSwipeSegment.direction = direction;
				}
				else if (currentSwipeSegment.direction != direction) {
					//Debug.Log("FingerControl:ProcessSwipeMove CANCEL " + SegmentListFingerIndex(finger) + " direction changed to " + direction + " from " + currentSwipeSegment.direction + ", distance=" + distance);
					SwipeChangeDirection(finger, direction);
				}
				previousPositionTurn = false;
			}
		}
		
		finger.swipeDirection = currentSwipeSegment.direction;
		if (currentSwipeSegment.direction != SwipeGesture.SwipeDirection.None || currentSwipeSegment.startPosition == Vector2.zero) {
			SendEventSwipeMove(finger, finger.swipeDirection, swipeFingersCount);
		}
	}
	
	private void SwipeChangeDirection(Finger finger, SwipeGesture.SwipeDirection direction)
	{
		SwipeGesture.SwipeDirection swipeDir;
		Vector2 swipeEndPosition = finger.position;
		SwipeSegment currentSwipeSegment = getLastSegment(finger);
		if (previousPositionTurn) {
			swipeEndPosition = previousPositions[SegmentListFingerIndex(finger), previousPositionsTurn];
		}
		currentSwipeSegment.endPosition = swipeEndPosition;
		//VerifySwipeSegments(finger);
		float swipeVelocity;
		float swipeDistance;
		if (IsASwipe (currentSwipeSegment.startPosition, swipeEndPosition, finger, out swipeDir, out swipeDistance, out swipeVelocity)) {
			//Debug.Log("FingerControl:SwipeChangeDirection change swipe " + swipeDir + ", swipeDistance=" + swipeDistance + " start/end pos=" + swipeEndPosition);
			currentSwipeSegment.Set(swipeDir, swipeDistance, swipeVelocity);
			SendEventSwipeSegmentCreate (currentSwipeSegment);
			currentSwipeSegment = new SwipeSegment(currentSwipeSegment);			
			currentSwipeSegment.Initalize(finger, swipeEndPosition, direction);
			segmentLists[SegmentListFingerIndex(finger)].Add(currentSwipeSegment);
			clearPreviousPositions(SegmentListFingerIndex(finger));
			//Debug.Log("FingerControl:SwipeChangeDirection swipeSegment direction " + currentSwipeSegment.direction);
		}
	}

	private void ProcessSwipeEnd (Finger finger)
	{
		if (segmentLists == null) {
			return;
		}
		
		bool sentSwipe = false;
		
		//Debug.Log("FingerControl:ProcessSwipeEnd " + finger.position);
		finger.endPosition = finger.position;
		SwipeSegment currentSwipeSegment = getLastSegment(finger);
		if (currentSwipeSegment == null) {
			//Debug.Log("FingerControl:ProcessSwipeEnd CancelPossibleSwipe  no current segment ");
			//CancelPossibleSwipe();
			return;
		}
			
		currentSwipeSegment.endPosition = finger.position;
		VerifySwipeSegments(finger);
		if (!possibleSwipe || !(finger.hasMoved || finger.downAndMoving)) {
			//Debug.Log("FingerControl:ProcessSwipeEnd CancelPossibleSwipe " + SegmentListFingerIndex(finger) + " cancel swipe  possibleSwipe=" + possibleSwipe);
			CancelPossibleSwipe();
			return;
		}
		
		SwipeGesture.SwipeDirection swipeDir;
		float swipeVelocity;
		float swipeDistance;
						
		if (IsASwipe (currentSwipeSegment.startPosition, currentSwipeSegment.endPosition, finger, out swipeDir, out swipeDistance, out swipeVelocity)) {			
			swipeFingerCount--;
			if (swipeFingerCount == 0) {
				currentSwipeSegment.Set(swipeDir, swipeDistance, swipeVelocity);
				//Debug.Log("FingerControl:ProcessSwipeEnd " + SegmentListFingerIndex(finger) + " send swipe event " + swipeDir + " swipeFingersCount=" + swipeFingersCount + " segment count=" + segmentLists[SegmentListFingerIndex(finger)].Count);
				SendEventSwipeSegmentCreate (currentSwipeSegment);
				SendEventSwipe (fingers, segmentLists, swipeFingersCount);
				sentSwipe = true;
			}
//			else {
//				Debug.Log("FingerControl:ProcessSwipeEnd no swipe because of finger count " + SegmentListFingerIndex(finger) + " swipeFingerCount=" + swipeFingerCount + " " + swipeDir);
//			}
		}
		else {
			swipeFingerCount--;
			segmentLists[SegmentListFingerIndex(finger)].RemoveAt(segmentLists[SegmentListFingerIndex(finger)].Count - 1);
			if (swipeFingerCount == 0 && segmentLists[SegmentListFingerIndex(finger)].Count > 0) {
				SwipeSegment endSegment = segmentLists[SegmentListFingerIndex(finger)][segmentLists[SegmentListFingerIndex(finger)].Count - 1];
				finger.endPosition = endSegment.endPosition;
				//Debug.Log("FingerControl:ProcessSwipeEnd " + SegmentListFingerIndex(finger) + " send swipe event " + swipeDir + " swipeFingersCount=" + swipeFingersCount + " segment count=" + segmentLists[SegmentListFingerIndex(finger)].Count);
				SendEventSwipe (fingers, segmentLists, swipeFingersCount);
				sentSwipe = true;
			}			
		}
		if (swipeFingerCount == 0) {
			//Debug.Log("FingerControl:ProcessSwipeEnd CancelPossibleSwipe  at end, zero fingers ");
			CancelPossibleSwipe();
		}
		else if (!sentSwipe) {
			SendEventSwipeCancel();
		}
	}
	
	private void VerifySwipeSegments(Finger finger)
	{
		if (segmentLists[SegmentListFingerIndex(finger)].Count < 2) {
			return;
		}
		SwipeSegmentList list = segmentLists[SegmentListFingerIndex(finger)];
		SwipeSegment lastSegment = list[list.Count - 1];
		SwipeSegment previousSegment = list[list.Count - 2];
		if (lastSegment.direction == previousSegment.direction) {
			previousSegment.Merge(lastSegment);
			list.RemoveAt(list.Count - 1);
		}
	}
	
	public SwipeSegmentList TryToMakeOneSegment(SwipeSegmentList segments)
	{
		if (segments.Count < 2) {
			return segments;
		}
		
		SwipeGesture.SwipeDirection direction = GetSwipeDirection(segments[0].startPosition, segments[segments.Count - 1].startPosition);
		//Debug.Log("FingerControl:TryToMakeOneSegment for " + direction + " with " + segments.Count + " segments");
		
//		string debug = "";
//		for (int i = 0; i < segments.Count; i++) {
//			debug += " " + i.ToString() + " [" + segments[i].direction.ToString() + "-" + segments[i].distance.ToString() + "]";
//		}
		//Debug.Log("FingerControl:TryToMakeOneSegment " + debug);
		
		for (int i = 0; i < segments.Count; i++) {
			if (!FriendlySwipeDirections(direction, segments[i].direction)) {
				//Debug.Log("FingerControl:TryToMakeOneSegment failed " + direction + " does not go with segment " + i + " direction " + segments[i].direction);
				if (segments[i].distance > swipeChangeSecondaryMaxDistance) {
					return segments;
				}
				//Debug.Log("FingerControl:TryToMakeOneSegment direction " + segments[i].direction + " forgiven for small distance " + segments[i].distance);
			}
		}
		
		SwipeSegment theSegment = segments[0];
		theSegment.CombineToSingleAfter(true, direction, segments[segments.Count - 1]);
		segments = new SwipeSegmentList();
		segments.Add(theSegment);
		//Debug.Log("FingerControl:TryToMakeOneSegment combined on " + direction + " returning " + segments.Count + " segments");
		return segments;
	}

	
	private static Vector2 toVector2 = new Vector2(1, 0);
	public static SwipeGesture.SwipeDirection GetSwipeDirection(Vector2 fromPosition, Vector2 toPosition)
	{
		Vector2 fromVector2 = toPosition - fromPosition;
		float resultAngle = Vector2.Angle( fromVector2, toVector2);
		Vector3 crossBig = Vector3.Cross( fromVector2, toVector2);		
		if (crossBig.z > 0f) {
			resultAngle = 360f - resultAngle;
		}
		
		// angles are in degress with zero to right increasing counter clockwise
		if (resultAngle >= 0 && resultAngle < swpDirRightMin) {
			return SwipeGesture.SwipeDirection.Right;
		}
		if (resultAngle <  swpDirUpMin) {
			return SwipeGesture.SwipeDirection.Plus45;
		}
		if (resultAngle <  swpDirUpMax) {
			return SwipeGesture.SwipeDirection.Up;
		}
		if (resultAngle <  swpDirLeftMin) {
			return SwipeGesture.SwipeDirection.Minus45;
		}
		if (resultAngle <  swpDirLeftMax) {
			return SwipeGesture.SwipeDirection.Left;
		}
		if (resultAngle <  swpDirDownMin) {
			return SwipeGesture.SwipeDirection.Minus135;
		}
		if (resultAngle <  swpDirDownMax) {
			return SwipeGesture.SwipeDirection.Down;
		}
		if (resultAngle < swpDirRightMax) {
			return SwipeGesture.SwipeDirection.Plus135;
		}
		return SwipeGesture.SwipeDirection.Right;


	}
	
	public static bool FriendlySwipeDirections(SwipeGesture.SwipeDirection direction1, SwipeGesture.SwipeDirection direction2)
	{
		if (direction1 == direction2) {
			return true;
		}
		
		if (direction1 == SwipeGesture.SwipeDirection.None || direction2 == SwipeGesture.SwipeDirection.None) {
			return false;
		}
		
		if (direction1 == SwipeGesture.SwipeDirection.Any || direction2 == SwipeGesture.SwipeDirection.Any) {
			return true;
		}
		
		
		if (friendlySwipeDirections[(int)direction1, 0] == direction2 || friendlySwipeDirections[(int)direction1, 1] == direction2) {
			return true;
		}

		
		return false;
	}
	
	public static SwipeGesture.SwipeDirection[] GetFriendlyDirections(SwipeGesture.SwipeDirection direction)
	{
		SwipeGesture.SwipeDirection[] directions = new SwipeGesture.SwipeDirection[2];
		directions[0] = friendlySwipeDirections[(int)direction, 0];
		directions[1] = friendlySwipeDirections[(int)direction, 1];
		return directions;
	}
	
	
	public static bool IsOppositeSwipeDirection(SwipeGesture.SwipeDirection direction)
	{
		switch(direction) {
		case SwipeGesture.SwipeDirection.Any:
			return true;
		case SwipeGesture.SwipeDirection.None:
			return false;
		case SwipeGesture.SwipeDirection.Up:
			if (direction == SwipeGesture.SwipeDirection.Down) {
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Plus45:
			if (direction == SwipeGesture.SwipeDirection.Minus135) {
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Right:
			if (direction == SwipeGesture.SwipeDirection.Left) {
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Plus135:
			if (direction == SwipeGesture.SwipeDirection.Minus45) {
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Down:
			if (direction == SwipeGesture.SwipeDirection.Up) {
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Minus135:
			if (direction == SwipeGesture.SwipeDirection.Plus45) {
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Left:
			if (direction == SwipeGesture.SwipeDirection.Right) {
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Minus45:
			if (direction == SwipeGesture.SwipeDirection.Plus135) {
				return true;
			}
			break;
		}
		
		return false;
	}


	private bool IsASwipe (Vector2 segmentStart,  Vector2 segmentEnd, Finger finger, out SwipeGesture.SwipeDirection direction, out float swipeDistance, out float velocity)
	{
		//Debug.Log("FingerControl:IsASwipe ");
		direction = GetSwipeDirection(segmentStart, segmentEnd);
		//Debug.Log("FingerControl:IsASwipe  segment start end " +segmentStart + "," + segmentEnd);
		//Debug.Log("FingerControl:IsASwipe  finger start end " + finger.startPosition + "," + finger.endPosition);
		velocity = 0f;
		swipeDistance = 0f;
		if (!finger.possibleSwipe) {
			//Debug.Log("FingerControl:IsASwipe not possible !finger.possibleSwipe");
			return false;
		}
				
		Vector3 delta = segmentEnd - segmentStart;
		float distance = delta.magnitude;
		
		if (distance < minSwipeDistance) {
			//Debug.Log("FingerControl:IsASwipe no swipe too short " + distance);
			return false;
		}
		
		//Debug.Log("FingerControl:IsASwipe is swipe set variables ");
		float elapsedTime = Time.time - finger.downAndMovingStartTime;
		velocity = distance / elapsedTime;
		swipeDistance = distance;
		
		return true;
	}

	// returns signed angle in radians
	public static float SignedAngle (Vector2 @from, Vector2 to)
	{
		// perpendicular dot product
		float perpDot = (@from.x * to.y) - (@from.y * to.x);
		return Mathf.Atan2 (perpDot, Vector2.Dot (@from, to));
	}

	// internal event callback system	
	public static event FingerIsDownCallBackInternal _delegateIsDownInternal;
	public static event FingerDownCallBackInternal _delegateFingerDownInternal;
	public static event FingerMovingCallBackInternal _delegateFingerMovingInternal;
	public static event FingerUpCallBackInternal _delegateFingerUpInternal;
	public static event FingerStationaryBeginCallBackInternal _delegateFingerStationaryBeginInternal;
	public static event FingerStationaryCallBackInternal _delegateFingerStationaryInternal;
	public static event FingerStationaryEndCallBackInternal _delegateFingerStationaryEndInternal;
	public static event FingerMoveBeginCallBackInternal _delegateFingerMoveBeginInternal;
	public static event FingerMoveCallBackInternal _delegateFingerMoveInternal;
	public static event FingerMoveEndCallBackInternal _delegateFingerMoveEndInternal;
	public static event DownAndMovingBeginCallBackInternal _delegateDownAndMovingBeginInternal;
	public static event DownAndMovingMoveCallBackInternal _delegateDownAndMovingMoveInternal;
	public static event DownAndMovingEndCallBackInternal _delegateDownAndMovingEndInternal;
	public static event PinchBeginCallBackInternal _delegatePinchBeginInternal;
	public static event PinchMoveCallBackInternal _delegatePinchMoveInternal;
	public static event PinchEndCallBackInternal _delegatePinchEndInternal;
	public static event SwipeCallBackInternal _delegateSwipeInternal;
	public static event SwipeCancelCallBackInternal _delegateSwipeCancelInternal;
	public static event SwipeMoveCallBackInternal _delegateSwipeMoveInternal;
	public static event RotateBeginCallBackInternal _delegateRotateBeginInternal;
	public static event RotateMoveCallBackInternal _delegateRotateMoveInternal;
	public static event RotateEndCallBackInternal _delegateRotateEndInternal;
	public static event SwipeSegmentCreate _delegateSwipeSegmentCreate;

	public delegate void FingerIsDownCallBackInternal (Finger finger, bool isDown);
	public delegate void FingerDownCallBackInternal (Finger finger);
	public delegate void FingerMovingCallBackInternal (Finger finger);
	public delegate void FingerUpCallBackInternal (Finger finger);
	public delegate void FingerStationaryBeginCallBackInternal (Finger finger, Vector2 fingerPos);
	public delegate void FingerStationaryCallBackInternal (Finger finger, float elapsedTime);
	public delegate void FingerStationaryEndCallBackInternal (Finger finger, Vector2 fingerPos, float elapsedTime);
	public delegate void FingerMoveBeginCallBackInternal (Finger finger, Vector2 fingerPos);
	public delegate void FingerMoveCallBackInternal (Finger finger, Vector2 fingerPos);
	public delegate void FingerMoveEndCallBackInternal (Finger finger, Vector2 fingerPos);
	public delegate void DownAndMovingBeginCallBackInternal (Finger finger);
	public delegate void DownAndMovingMoveCallBackInternal (Finger finger, Vector2 fingerPos, Vector2 delta);
	public delegate void DownAndMovingEndCallBackInternal (Finger finger);
	public delegate void PinchBeginCallBackInternal (Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta);
	public delegate void PinchMoveCallBackInternal (Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta);
	public delegate void PinchEndCallBackInternal (Finger finger1, Finger finger2);
	public delegate void SwipeCallBackInternal (Finger[] fingers,  SwipeSegmentList[] segments, int fingersDown);
	public delegate void SwipeCancelCallBackInternal ();
	public delegate void SwipeMoveCallBackInternal (Finger finger,  SwipeGesture.SwipeDirection direction, int fingersDown);
	public delegate void RotateBeginCallBackInternal (Finger finger1, Finger finger2);
	public delegate void RotateMoveCallBackInternal (Finger finger1, Finger finger2, float rotationAngleDelta);
	public delegate void RotateEndCallBackInternal (Finger finger1, Finger finger2);
	public delegate void SwipeSegmentCreate(SwipeSegment segment);


	protected void SendEventFingerDown (Finger finger)
	{
		if (_delegateFingerDownInternal != null) {
			_delegateFingerDownInternal (finger);
		}
	}
	protected void SendEventFingerMoving (Finger finger)
	{
		if (_delegateFingerMovingInternal != null) {
			_delegateFingerMovingInternal (finger);
		}
	}
	protected void SendEventFingerUp (Finger finger)
	{
		if (_delegateFingerUpInternal != null) {
			_delegateFingerUpInternal (finger);
		}
	}
	protected void SendIsDown (Finger finger, bool isDown)
	{
		if (_delegateIsDownInternal != null) {
			_delegateIsDownInternal (finger, isDown);
		}
	}
	
	

	protected void SendEventFingerStationaryBegin (Finger finger, Vector2 fingerPos)
	{
		if (_delegateFingerStationaryBeginInternal != null) {
			_delegateFingerStationaryBeginInternal (finger, fingerPos);
		}
	}

	protected void SendEventFingerStationary (Finger finger, float elapsedTime)
	{
		if (_delegateFingerStationaryInternal != null) {
			_delegateFingerStationaryInternal (finger, elapsedTime);
		}
	}

	protected void SendEventFingerStationaryEnd (Finger finger, Vector2 fingerPos, float elapsedTime)
	{
		if (_delegateFingerStationaryEndInternal != null) {
			_delegateFingerStationaryEndInternal (finger, fingerPos, elapsedTime);
		}
	}

	protected void SendEventFingerMoveBegin (Finger finger, Vector2 fingerPos)
	{
		if (_delegateFingerMoveBeginInternal != null) {
			_delegateFingerMoveBeginInternal (finger, fingerPos);
		}
	}

	protected void SendEventFingerMove (Finger finger, Vector2 fingerPos)
	{
		if (_delegateFingerMoveInternal != null) {
			_delegateFingerMoveInternal (finger, fingerPos);
		}
	}

	protected void SendEventFingerMoveEnd (Finger finger, Vector2 fingerPos)
	{
		if (_delegateFingerMoveEndInternal != null) {
			_delegateFingerMoveEndInternal (finger, fingerPos);
		}
	}

	protected void SendEventFingerUp (Finger finger, float timeHeldDown)
	{
		if (_delegateFingerUpInternal != null) {
			_delegateFingerUpInternal (finger);
		}
	}

	protected void SendEventDownAndMovingBegin (Finger finger)
	{
		if (_delegateDownAndMovingBeginInternal != null) {
			_delegateDownAndMovingBeginInternal (finger);
		}
	}

	protected void SendEventDownAndMoving (Finger finger, Vector2 fingerPos, Vector2 delta)
	{
		if (_delegateDownAndMovingMoveInternal != null) {
			_delegateDownAndMovingMoveInternal (finger, fingerPos, delta);
		}
	}

	protected void SendEventDownAndMovingEnd (Finger finger)
	{
		if (_delegateDownAndMovingEndInternal != null) {
			_delegateDownAndMovingEndInternal (finger);
		}
	}

	protected void SendEventPinchBegin (Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta)
	{
		if (_delegatePinchBeginInternal != null) {
			_delegatePinchBeginInternal (finger1, finger2, pinchDirection, magnitudeDelta);
		}
	}

	protected void SendEventPinchMove (Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta)
	{
		if (_delegatePinchMoveInternal != null) {
			_delegatePinchMoveInternal (finger1, finger2, pinchDirection, magnitudeDelta);
		}
	}

	protected void SendEventPinchEnd (Finger finger1, Finger finger2)
	{
		if (_delegatePinchEndInternal != null) {
			_delegatePinchEndInternal (finger1, finger2);
		}
	}

	protected void SendEventSwipe (Finger[] fingers,  SwipeSegmentList[] segments, int fingersDown)
	{
		if (_delegateSwipeInternal != null) {
			_delegateSwipeInternal (fingers, segments, fingersDown);
		}
	}
	protected void SendEventSwipeMove (Finger finger,  SwipeGesture.SwipeDirection direction, int fingersDown)
	{
		if (_delegateSwipeMoveInternal != null) {
			_delegateSwipeMoveInternal (finger, direction, fingersDown);
		}
	}
	protected void SendEventSwipeCancel ()
	{
		if (_delegateSwipeCancelInternal != null) {
			_delegateSwipeCancelInternal ();
		}
	}
	
	protected void SendEventSwipeSegmentCreate (SwipeSegment segment)
	{
		if (_delegateSwipeSegmentCreate != null) {
			_delegateSwipeSegmentCreate (segment);
		}
	}
	protected void SendEventRotateBegin (Finger finger1, Finger finger2)
	{
		if (_delegateRotateBeginInternal != null) {
			_delegateRotateBeginInternal (finger1, finger2);
		}
	}

	protected void SendEventRotateMove (Finger finger1, Finger finger2, float rotationAngleDelta)
	{
		if (_delegateRotateMoveInternal != null) {
			_delegateRotateMoveInternal (finger1, finger2, rotationAngleDelta);
		}
	}

	protected void SendEventRotateEnd (Finger finger1, Finger finger2)
	{
		if (_delegateRotateEndInternal != null) {
			_delegateRotateEndInternal (finger1, finger2);
		}
	}

}
