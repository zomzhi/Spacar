using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineGesture : BaseGesture
{
	public enum LineSwipeDirection {
		Forward, 
		Backward, 
		Either, 
		Anywhere  // only used on cirular line gestures
	}
	
	public enum LineIdentification {
		Precise,
		Clean,
		Sloppy
	}
	public LineSwipeDirection restrictLineSwipeDirection = LineGesture.LineSwipeDirection.Anywhere;
	public LineFactory.LineType[] lineFactoryLineType;
	
	public FingerLocation startsOnObject = FingerLocation.Always;
	public FingerLocation endsOnObject = FingerLocation.Always;
	public LineIdentification lineIdentification = LineIdentification.Sloppy; 
	public bool returnSwipeAlways = false;
	public float matchPositionDiff = 50f;
	public float matchLengthDiffPercent = 0.50f;  // decimal per cent
	public float maxTimeBetweenLines = 0.8f;
		
	public SwipeSegmentList swipeSegments;
	public LineFactory.LineType swipedLineType = LineFactory.LineType.None;
	public List<SwipeSegmentList> swipeList = new List<SwipeSegmentList>();
	public LineIdentification lineIdentificationUsed;
	
	private bool thereAreCompound = false;
	private int maxCompoundLines = 0;
	private int maxCompoundLineSegments = 0;
	private Vector2 fingerStartPos;
	private Vector2 fingerEndPos;
	private float endSwipeTime = -1f;
	private Finger lastFinger;
	private bool finishedSwipe = true;
	public bool performingSwipe = false;
	public SwipeSegmentList compressedSwipeSegments = null;
	public SwipeSegmentList[] compressedSwipeSegmentsList;
	public LineSwipeBase usedLineSwipe;
	public string errorString;

	protected bool useLineFactory = false;
	protected LineFactory lineFactory;
	protected event LineCallBack eventHandlers_Line;
	
	
	public bool HaveLineFactoryType(LineFactory.LineType lineType)
	{
		if (lineFactoryLineType == null || lineFactoryLineType.Length == 0) {
			return false;
		}
		for (int i = 0; i < lineFactoryLineType.Length; i++) {
			if (lineFactoryLineType[i] == lineType) {
				return true;
			}
		}
		return false;
	}
	
	public LineSwipeBase GetLineSwipeDef(LineFactory.LineType lineType)
	{
		UseLineFactory();
		return lineFactory.GetLineSwipe(lineType);
	}
	
	public int GetFactoryTypesCount()
	{
		if (lineFactoryLineType == null) {
			return 0;
		}
		return lineFactoryLineType.Length;
	}
	public void AddLineFactoryType(LineFactory.LineType lineType, bool clearList)
	{
		UseLineFactory();
		if (lineType == LineFactory.LineType.None) {
			clearList = true;
		}
			
		if (clearList) {
			thereAreCompound = false;
			maxCompoundLines = 0;
			maxCompoundLineSegments = 0;

			if (lineType == LineFactory.LineType.None) {
				lineFactoryLineType = null;
			}
			else {
				lineFactoryLineType = new LineFactory.LineType[1];
				lineFactoryLineType[0] = lineType;
			}
			return;
		}
		
		if (HaveLineFactoryType(lineType)) {
			return;
		}
		LineFactory.LineType[] oldList = lineFactoryLineType;
		int oldLength = 0;
		if (oldList != null) {
			oldLength = oldList.Length;
		}
		lineFactoryLineType = new LineFactory.LineType[oldLength + 1];
		if (oldList != null && oldList.Length > 0) {
			oldList.CopyTo(lineFactoryLineType, 0);
		}
		lineFactoryLineType[oldLength] = lineType;
		VerifyType(lineType);
	}
	
	private void VerifyType(LineFactory.LineType lineType)
	{
		UseLineFactory();
		
		if (lineFactory.IsCompound(lineType)) {
			thereAreCompound = true;
			int count = lineFactory.GetCount(lineType);
			if (count > maxCompoundLines) {
				maxCompoundLines = count;
			}
			count = lineFactory.GetMaxSegmentCount(lineType);
			if (count > maxCompoundLineSegments) {
				maxCompoundLineSegments = count;
			}
			//Debug.Log("LineGesture:AddLineFactoryType thereAreCompound " + lineType + ", maxCompoundLineSegments=" + maxCompoundLineSegments + ", maxCompoundLines=" + maxCompoundLines);
		}
	}
	
	public void RemoveLineFactoryType(LineFactory.LineType lineType)
	{
		if (!useLineFactory || lineFactoryLineType == null || lineFactoryLineType.Length == 0) {
			return;
		}
		int found = -1;
		for (int i = 0; i < lineFactoryLineType.Length; i++) {
			if (lineFactoryLineType[i] == lineType) {
				found = i;
				break;
			}
		}
		if (found < 0) {
			return;
		}
		
		LineFactory.LineType[] oldList = lineFactoryLineType;
		lineFactoryLineType = new LineFactory.LineType[oldList.Length - 1];
		int index = 0;
		for (int i = 0; i < oldList.Length; i++) {
			if (i != found) {
				lineFactoryLineType[index++] = oldList[i];
			}
		}
	}
	
	public delegate void LineCallBack (LineGesture gesture);
		
	public void RegisterLineCallback (LineCallBack callback)
	{
		eventHandlers_Line += callback;
	}
	
	protected override void EnableGesture()
	{
		base.EnableGesture();
		if (useLineFactory) {
			UseLineFactory();
		}
		FingerControl.AddSwipeCallback();
		FingerControl._delegateSwipeInternal += SwipeGestureOnSwipe;
		
		if (lineFactoryLineType != null) {
			for (int i = 0; i < lineFactoryLineType.Length; i++) {
				VerifyType(lineFactoryLineType[i]);
			}
		}
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveSwipeCallback();
		FingerControl._delegateSwipeInternal -= SwipeGestureOnSwipe;
	}

	protected void SwipeGestureOnSwipe(Finger[] fingers,  SwipeSegmentList[] segmentsList, int fingerCount)		
	{
		//Debug.Log("LineGesture:SwipeGestureOnSwipe #=" + segmentsList.Count + ", swipeList Count=" + ((swipeList == null) ? "null" : swipeList.Count.ToString()));
		if (finishedSwipe) {
			ClearSwipe();
			swipeList.Clear();
		}
		if (fingerCount > 1 || segmentsList == null || segmentsList.Length == 0) {
			return;
		}
		
		SwipeSegmentList swipeSegmentsFirst = segmentsList[0];
		Finger finger = fingers[0];
		
		if (swipeList.Count	== 0) {
			fingerStartPos = finger.startPosition;
		}
		fingerEndPos = finger.endPosition;
		
		performingSwipe = true;
		swipeList.Add(swipeSegmentsFirst);
		lastFinger = finger;		
		
		if (thereAreCompound) {
			if (swipeList.Count	< maxCompoundLines) {				
				endSwipeTime = Time.time + maxTimeBetweenLines;
				finishedSwipe = false;
				//Debug.Log("LineGesture:SwipeGestureOnSwipe wait for another swipe ");
				return;
			}
			//Debug.Log("LineGesture:SwipeGestureOnSwipe compound wait done, evaluate " + swipeList.Count);
			ClearSwipeTime();
		}
		EvaluateSwipe();
	}
	
	protected void EvaluateSwipe()
	{
		finishedSwipe = true;
		if (swipeList == null || swipeList.Count == 0) {
			return;
		}	
		usedLineSwipe = null;
		
		//Debug.Log("LineGesture:EvaluateSwipe  swipeList.Count " +  swipeList.Count);
		performingSwipe = false;
		if (FingerActivated(startsOnObject, fingerStartPos) &&  FingerActivated(endsOnObject, fingerEndPos) ) {			
			if (swipeList.Count	== 1) {
				swipeSegments = swipeList[0];
			}
			else {
				swipeSegments = null;
			}
			
			swipedLineType = FindLineType(swipeList);

			//Debug.Log("LineGesture:EvaluateSwipe swipedLineType " + swipedLineType);
			if (swipedLineType == LineFactory.LineType.None && !returnSwipeAlways) {
				GestureMessage("GestureLineSwipeFailure");
				return;
			}
		
			finger = lastFinger;
			if (eventHandlers_Line != null) {
				eventHandlers_Line (this);
			}
			
			if (lineFactory != null) {
				if (swipedLineType == LineFactory.LineType.None) {
					if (GetFactoryTypesCount() != 1) {
						usedLineSwipe = lineFactory.GetLineSwipe(lineFactoryLineType[0]).GetUsedLineSwipe();
					}
				}
				else {
					usedLineSwipe = lineFactory.GetLineSwipe(swipedLineType).GetUsedLineSwipe();
				}
			}
			if (usedLineSwipe != null) {
				compressedSwipeSegments = usedLineSwipe.compressedSwipeSegments;
				compressedSwipeSegmentsList = usedLineSwipe.compressedSwipeSegmentsList;
			}
			if (swipedLineType == LineFactory.LineType.None) {
				errorString = LineSwipeBase.lastError;
				if (GetFactoryTypesCount() != 1) {
					usedLineSwipe = null;
					compressedSwipeSegments = null;
					compressedSwipeSegmentsList = null;
				}
			}
			else {
				errorString = "";
			}
			
			GestureMessage("GestureLineSwipe");
		}
		else {
			swipedLineType = LineFactory.LineType.None;
			swipeSegments = null;
			GestureMessage("GestureLineSwipeFailure");
		}
	}
	
	protected void ClearSwipe()
	{
		//Debug.Log("LineGesture:ClearSwipe"); 
		swipeList.Clear();
		finishedSwipe = true;
		ClearSwipeTime();
	}
	
	protected void ClearSwipeTime()
	{
		endSwipeTime = -1f;
	}
	
	void Update()
	{
		if (!thereAreCompound || endSwipeTime <= 0f) {
			return;
		}
		
		if (endSwipeTime < Time.time) {
			//Debug.Log("LineGesture:SwipeGestureOnSwipe wait for Update done, EvaluateSwipe ");
			if (!FingerControl.IsSwiping()) {			
				EvaluateSwipe();
			}
			endSwipeTime = -1f;
		}
	}
	

	private LineFactory.LineType FindLineType(List<SwipeSegmentList> listOfSwipes)
	{
		if (lineFactory == null || lineFactoryLineType == null || lineFactoryLineType.Length == 0) {
			//Debug.Log("LineGesture:LineType return none, none defined");
			return LineFactory.LineType.None;
		}
		
		LineFactory.LineType theType = lineFactory.FindLineType(lineFactoryLineType, listOfSwipes, lineIdentification, restrictLineSwipeDirection, matchPositionDiff, matchLengthDiffPercent);
		if (theType != LineFactory.LineType.None) {
			LineSwipeBase lineSwipeDef = GetLineSwipeDef(theType);
			lineIdentificationUsed = lineSwipeDef.identificationUsed;
			//Debug.Log("LineGesture:FindLineType SUCCESS " + theType + " " + lineIdentificationUsed + " " + lineSwipeDef.compressedSwipeSegments + " " + lineSwipeDef.compressedSwipeSegmentsList);
		}
		
		return theType;
	}
	
	private void UseLineFactory()
	{
		useLineFactory = true;
		if (lineFactory == null) {
			lineFactory = LineFactory.Factory();
		}
	}
	
}

