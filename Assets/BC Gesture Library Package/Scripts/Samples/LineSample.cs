using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineSample : Sample
{
	public GUIStyle listTextStyle;
	public GUIStyle dataTextStyle;
	public GUIStyle errorTextStyle;
	
	private LineGesture lineGesture;
	private ComboControl fingerStartLocationCombo;
	private ComboControl fingerEndLocationCombo;
	private ComboControl identificationCombo;
	private ComboControl restrictDirectionCombo;
	private ComboControl lineFactoryCombo;
	
	private Rect settingsLabelRect;
	private Rect fingerStartLocationLabelRect;
	private Rect fingerStartLocationListRect;
	private Rect fingerEndLocationLabelRect;
	private Rect fingerEndLocationListRect;
	private Rect identificationLabelRect;
	private Rect identificationBoxRect;
	private Rect restrictDirectionLabelRect;
	private Rect restrictDirectionBoxRect;
	private Rect lineFactoryAddLabelRect;
	private Rect lineFactoryAddBoxRect;
	private Rect lineFactoryLabelRect;
	private Rect lineFactoryBoxRect;
	private Rect returnSwipeAlwaysLabelRect;
	private Rect returnSwipeAlwaysBoxRect;
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect fingerLabelRect;
	private Rect fingerBoxRect;	
	private Rect lineSegmentLabelRect;
	private Rect lineSegmentHitRect;
	private Rect clearResultButtonRect;
	private Rect[] segmentRects = new Rect[30];
	private Rect[] segmentBeforeRects = new Rect[30];
	private Rect lineFactoryListLabelRect;
	private Rect[] lineFactoryListRects = new Rect[30];
	private List<Vector3> fingerLines = new List<Vector3>();
	private List<Rect> fingerLinesGUI = new List<Rect>();
	private ArrayList fingerLinesGUIList = new ArrayList();
	private Rect errorTextRect;
	
	private bool gestureOccurred = false;
	private float lineZ = 0.0f;
	
	protected override void SetupData ()
	{
		lineGesture = this.gameObject.GetComponentInChildren<LineGesture>();
		
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		
		restrictDirectionLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 120f, boxHeight);
		float boxX = propLabelX + 105f;
		restrictDirectionBoxRect = new Rect(boxX, restrictDirectionLabelRect.y, 120f, boxHeight);
		lineFactoryAddLabelRect = new Rect(propLabelX, restrictDirectionBoxRect.yMax + betweenYGap, 120f, boxHeight);
		lineFactoryAddBoxRect = new Rect(boxX, lineFactoryAddLabelRect.y, checkboxSize, checkboxSize);
		lineFactoryAddLabelRect.y += labelCenterOffset;
		identificationLabelRect = new Rect(propLabelX, lineFactoryAddBoxRect.yMax + betweenYGap, 120f, boxHeight);
		identificationBoxRect = new Rect(boxX, identificationLabelRect.y, 120f, boxHeight);
		lineFactoryLabelRect = new Rect(propLabelX, identificationBoxRect.yMax + betweenYGap, 120f, boxHeight);
		lineFactoryBoxRect = new Rect(boxX, lineFactoryLabelRect.y, 120f, boxHeight);
		
		returnSwipeAlwaysLabelRect = new Rect(propLabelX, lineFactoryBoxRect.yMax + betweenYGap, 120f, boxHeight);
		returnSwipeAlwaysBoxRect = new Rect(boxX, returnSwipeAlwaysLabelRect.y, checkboxSize, checkboxSize);
		returnSwipeAlwaysLabelRect.y += labelCenterOffset;
		
		fingerStartLocationLabelRect = new Rect(propLabelX, returnSwipeAlwaysBoxRect.yMax + betweenYGap, 100f, boxHeight);
		fingerStartLocationListRect = new Rect(boxX, fingerStartLocationLabelRect.y, 120f, boxHeight);
		fingerEndLocationLabelRect = new Rect(propLabelX, fingerStartLocationListRect.yMax + betweenYGap, 100f, boxHeight);
		fingerEndLocationListRect = new Rect(boxX, fingerEndLocationLabelRect.y, 120f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, fingerEndLocationListRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = propLabelX + 100f;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX - 25, eventLabelRect.y, 200f, boxResultHeight);
		fingerLabelRect = new Rect(propLabelX, eventBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		fingerBoxRect = new Rect(boxX, fingerLabelRect.y, 100f, boxResultHeight);
		
		clearResultButtonRect = new Rect(propLabelX, fingerBoxRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
		
		
		lineFactoryListLabelRect = new Rect(305f, sectionY, 100f, 12f);
		float currentY = lineFactoryListLabelRect.y + boxHeight - 3f;
		float listX = lineFactoryListLabelRect.x + 10f;
		for (int i = 0; i < lineFactoryListRects.Length; i++) {
			lineFactoryListRects[i] = new Rect(listX, currentY, 300f, 12f);
			currentY += boxHeight - 3f;
		}
		
		lineSegmentLabelRect = new Rect(Screen.width - 90f, sectionY, 100f, 15f);
		listX = lineSegmentLabelRect.x;
		float listXBefore = listX - 90;
		lineSegmentHitRect = new Rect(listX, lineSegmentLabelRect.y + boxHeight, 100f, 15f);
		errorTextRect = new Rect(listX - 620, lineSegmentHitRect.y, 700f, 15f);
		
		currentY = lineSegmentHitRect.y + boxHeight - 3f;
		for (int i = 0; i < segmentRects.Length; i++) {
			segmentRects[i] = new Rect(listX, currentY, 300f, 12f);
			segmentBeforeRects[i] = new Rect(listXBefore, currentY, 300f, 12f);
			currentY += boxHeight - 3f;
		}
		
		FingerControl._delegateSwipeSegmentCreate += OnSwipeSegmentCreate;
	}
	
	private bool firstOnGUI = true;
	private LineFactory.LineType lineType = LineFactory.LineType.None;
	private LineFactory.LineType lineTypeOld = LineFactory.LineType.None;
	private bool typesAdditive = true;
	
	private void OnGUI () 
	{
			
		if (firstOnGUI) {
			initializeGUI();
			fingerStartLocationCombo = new ComboControl();
			fingerStartLocationCombo.SetFingerLocationList((int) lineGesture.startsOnObject, false);
			fingerEndLocationCombo = new ComboControl();
			fingerEndLocationCombo.SetFingerLocationList((int) lineGesture.endsOnObject, false);
			identificationCombo = new ComboControl();
			identificationCombo.SetLineIdentificationList((int) lineGesture.lineIdentification);
			restrictDirectionCombo = new ComboControl();
			restrictDirectionCombo.SetLineSwipeDirectionList((int) lineGesture.restrictLineSwipeDirection);
			lineFactoryCombo = new ComboControl();
			lineFactoryCombo.SetLineFactoryList(0);
			lineFactoryCombo.SetButtonName("Add Type");
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		
		if (SampleGUI.sceneSelectionOpen) {
			ToggleVisibility(true);
		}
		else {
			ToggleVisibility(lineGesture.startsOnObject != BaseGesture.FingerLocation.Always || lineGesture.endsOnObject != BaseGesture.FingerLocation.Always);
		
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			
			// draw line types before lists so they be under
			GUI.Label(lineFactoryListLabelRect, "Line types:", listTextStyle);
			if (lineGesture.lineFactoryLineType != null) {
				for (int i = 0; i < lineGesture.lineFactoryLineType.Length; i++) {
					GUI.Label(lineFactoryListRects[i], lineGesture.lineFactoryLineType[i].ToString(), listTextStyle);
				}
			}
			
						
			GUI.Label(lineFactoryAddLabelRect, "Types additive:", settingsTextStyle);
			
			typesAdditive = GuiCheckBox(lineFactoryAddBoxRect, typesAdditive);
			
			GUI.Label(returnSwipeAlwaysLabelRect, "Always Return:", settingsTextStyle);
			
			lineGesture.returnSwipeAlways = GuiCheckBox(returnSwipeAlwaysBoxRect, lineGesture.returnSwipeAlways);
						
			lineGesture.startsOnObject = (BaseGesture.FingerLocation) fingerStartLocationCombo.GUIShowList("Start:", fingerStartLocationLabelRect, fingerStartLocationListRect);
			lineGesture.endsOnObject = (BaseGesture.FingerLocation) fingerEndLocationCombo.GUIShowList("End:", fingerEndLocationLabelRect, fingerEndLocationListRect);
			//Debug.Log("SwipepingSample:OnGUI fingerLocation " + lineGesture.fingerLocation);
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);									
			GUI.Label(fingerLabelRect, "Finger used:", resultsTextStyle);
			GUI.Label(lineSegmentLabelRect, "Segments:", dataTextStyle);
			
			if (fingerLinesGUIList != null && fingerLinesGUIList.Count > 0) {
				for (int l = 0; l < fingerLinesGUIList.Count; l++) {
					List<Rect> numberRects = (List<Rect>) fingerLinesGUIList[l];
					if (numberRects.Count > 1) {
						for (int i = 0; i < numberRects.Count; i++) {
							GUI.Label(numberRects[i], (i + 1).ToString(), dataTextStyle);					
						}
					}
				}
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					ClearAll();
				}
			}
			
			if (gestureOccurred && (lineGesture.swipedLineType != LineFactory.LineType.None || lineGesture.returnSwipeAlways)) {
				GUI.Label(eventBoxRect, "Received " + lineGesture.swipedLineType.ToString(), resultsTextStyle);
				GUI.Label(fingerBoxRect, lineGesture.finger.Index().ToString(), resultsTextStyle);
				
				if (lineGesture.swipedLineType == LineFactory.LineType.None) {
					if (lineGesture.GetFactoryTypesCount() == 1 && lineGesture.errorString != null && lineGesture.errorString.Length > 0) {
						GUI.Label(errorTextRect, "No match - " + lineGesture.errorString, errorTextStyle);
					}
					else if (lineGesture.GetFactoryTypesCount() == 0) {
						GUI.Label(errorTextRect, "No line types have been chosen", errorTextStyle);
					}
					else {
						GUI.Label(lineSegmentHitRect, "No match", dataTextStyle);
					}
					
					
				}
				else {
					GUI.Label(lineSegmentHitRect, lineGesture.swipedLineType.ToString(), dataTextStyle);
				}
				SwipeSegment segment;
				float dist;
				int rectIndex = 0;
				int[] segmentRectIndexes = new int[lineGesture.swipeList.Count];
				if (lineGesture.swipeList != null) {
					SwipeSegmentList swipeSegments;
					if (lineGesture.swipedLineType != LineFactory.LineType.None) {
						GUI.Label(segmentRects[rectIndex], lineGesture.lineIdentificationUsed.ToString(), dataTextStyle);
						rectIndex++;
						rectIndex++;
					}
					segmentRectIndexes[0] = rectIndex;
					for (int i = 0; i < lineGesture.swipeList.Count && rectIndex < segmentRects.Length; i++) {
						swipeSegments = (SwipeSegmentList) lineGesture.swipeList[i];
						if (lineGesture.swipedLineType != LineFactory.LineType.None && !swipeSegments.isForwardUsed) {
							GUI.Label(segmentRects[rectIndex], "Backward", dataTextStyle);
							rectIndex++;
						}
						segmentRectIndexes[i] = rectIndex;
						for (int j = 0; j < swipeSegments.Count && rectIndex < segmentRects.Length; j++) {
							segment =  (SwipeSegment) swipeSegments[j];
							dist = Mathf.Round(segment.distance);
							GUI.Label(segmentRects[rectIndex], segment.direction.ToString() + " " + dist.ToString(), dataTextStyle);
							rectIndex++;
						}					
						rectIndex++;
					}
					
					//Debug.Log("LineSample:OnGUI swipeList.Count " + lineGesture.swipeList.Count + "  compressedSwipeSegments " + (lineGesture.compressedSwipeSegments != null ? "Exists" : "Null") + "  compressedSwipeSegmentsList " + (lineGesture.compressedSwipeSegmentsList != null ? "Exists" : "Null"));
					if (lineGesture.swipeList.Count == 1 && lineGesture.compressedSwipeSegments != null) {
						rectIndex = segmentRectIndexes[0];
						for (int i = 0; i < lineGesture.compressedSwipeSegments.Count && rectIndex < segmentRects.Length; i++) {
							segment =  (SwipeSegment) lineGesture.compressedSwipeSegments[i];
							dist = Mathf.Round(segment.distance);
							GUI.Label(segmentBeforeRects[rectIndex], segment.direction.ToString() + " " + dist.ToString(), dataTextStyle);
							rectIndex++;
						}
					}
					else if (lineGesture.swipeList.Count > 1 && lineGesture.compressedSwipeSegmentsList != null) {
						for (int i = 0; i < lineGesture.swipeList.Count && rectIndex < segmentRects.Length; i++) {
							rectIndex = segmentRectIndexes[i];
							if (lineGesture.compressedSwipeSegmentsList[i] != null) { 
								swipeSegments = (SwipeSegmentList) lineGesture.compressedSwipeSegmentsList[i];
							}
							else {
								//swipeSegments = (SwipeSegmentList) lineGesture.swipeList[i];
								continue;
							}
							for (int j = 0; j < swipeSegments.Count && rectIndex < segmentRects.Length; j++) {
								segment =  (SwipeSegment) swipeSegments[j];
								dist = Mathf.Round(segment.distance);
								GUI.Label(segmentBeforeRects[rectIndex], segment.direction.ToString() + " " + dist.ToString(), dataTextStyle);
								rectIndex++;
							}					
							rectIndex++;
						}
					}
												
				}
				
			}
			
			
			lineGesture.restrictLineSwipeDirection = (LineGesture.LineSwipeDirection) restrictDirectionCombo.GUIShowList("Direction:", restrictDirectionLabelRect, restrictDirectionBoxRect);
			lineGesture.lineIdentification = (LineGesture.LineIdentification) identificationCombo.GUIShowList("Identification:", identificationLabelRect, identificationBoxRect);
			lineType = (LineFactory.LineType) lineFactoryCombo.GUIShowList("Type:", lineFactoryLabelRect, lineFactoryBoxRect);
			if (lineTypeOld != lineType) {
				lineGesture.AddLineFactoryType(lineType, !typesAdditive);
				lineTypeOld = lineType;
			}
			
		}
	}
	
	
	private bool currentVisible = true;

	public void ToggleVisibility(bool isVisible) {
	    if (currentVisible == isVisible) {
			return;
		}
		
	    Renderer[] renderers = this.gameObject.GetComponentsInChildren<Renderer>();;
	    for (int i = 0; i < renderers.Length; i++) {
	        renderers[i].enabled = isVisible;
	    }
		currentVisible = isVisible;
	}	
	private Vector2 lastPosition = Vector2.zero;
	private bool onFirstLine = false;
	private bool lockPositions = false;

	public void OnSwipeSegmentCreate(SwipeSegment segment)
	{
		if (!isDown || segment.startPosition == Vector2.zero || segment.endPosition == Vector2.zero) {
			return;
		}
		if (lineGesture.ActiveCount() > 1) {
			ClearAll();
			return;
		}
		lockPositions = true;
		
		if (fingerLines.Count == 0 || lastPosition == Vector2.zero || lastPosition.x != segment.startPosition.x || lastPosition.y != segment.startPosition.y) {
			//Debug.Log("LineSample:OnSwipeSegmentCreate adding start position " + fingerLines.Count + " " + segment.startPosition);
			if (onFirstLine) {
				fingerLines[0] = Sample.ScreenToWorldPosition(segment.startPosition, lineZ);
				//Debug.Log("LineSample:OnSwipeSegmentCreate onFirstLine update start 0 " + fingerLines[0]);
			}
			else {
				fingerLines.Add(Sample.ScreenToWorldPosition(segment.startPosition, lineZ));
				//Debug.Log("LineSample:OnSwipeSegmentCreate  add start " + fingerLines[fingerLines.Count - 1]);
			}
			AddFingerLinesGUI(segment.startPosition, fingerLinesGUI);
		}
		if (onFirstLine) {
			fingerLines[1] = Sample.ScreenToWorldPosition(segment.endPosition, lineZ);
			fingerLines.Add(fingerLines[1]);
			//Debug.Log("LineSample:OnSwipeSegmentCreate onFirstLine update end 1 " + fingerLines[1]);
			onFirstLine = false;
		}
		else {
			fingerLines[fingerLines.Count - 1] = Sample.ScreenToWorldPosition(segment.endPosition, lineZ);
			fingerLines.Add(fingerLines[fingerLines.Count - 1]);
			//Debug.Log("LineSample:OnSwipeSegmentCreate  add end 1 " + fingerLines[fingerLines.Count - 1]);
		}
		AddFingerLinesGUI(segment.endPosition, fingerLinesGUI);
		lastPosition = segment.endPosition;
	
		//Debug.Log("LineSample:OnSwipeSegmentCreate " + fingerLines.Count + " " + segment.startPosition + " to " + segment.endPosition + " - " + fingerLines[fingerLines.Count - 1]);
		lockPositions = false;
	}
	
	
	private void ClearAll()
	{
		gestureOccurred = false;
		ClearAllPoints();
	}
	
	private bool isDown = true;
	// Message from LineGesture
	void GestureLineSwipe(LineGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		
		//Debug.Log("LineSample:GestureLineSwipe " + gesture.swipeSegments.Count + " segments " + lineGesture.swipedLineType.ToString());
		lineGesture = gesture;
		gestureOccurred = true;
		
		ClearAllPoints();
		
		if (lineGesture.swipedLineType == LineFactory.LineType.None) {
			GUILine.Factory().SetLineColor(GUILine.DrawColor.Bad);
		}
		else {
			GUILine.Factory().SetLineColor(GUILine.DrawColor.Normal);
		}
						
		
		if (lineGesture.swipeList != null) {
			SwipeSegmentList swipeSegments;
			//Debug.Log("LineSample:GestureLineSwipe load swipeList.Count " + lineGesture.swipeList.Count);
			for (int i = 0; i < lineGesture.swipeList.Count; i++) {
				fingerLinesGUI = new List<Rect>();
				fingerLines = new List<Vector3>();
				swipeSegments = (SwipeSegmentList) lineGesture.swipeList[i];
				//Debug.Log("LineSample:GestureLineSwipe " + i  + " with " + swipeSegments);
				LoadFinalSegments(swipeSegments, fingerLines, fingerLinesGUI);
				fingerLinesGUIList.Add(fingerLinesGUI);

				if (lineGesture.swipedLineType != LineFactory.LineType.None &&
				    	((lineGesture.swipeList.Count == 1 && lineGesture.compressedSwipeSegments == null) || 
				    	(lineGesture.swipeList.Count > 1 && (lineGesture.compressedSwipeSegmentsList == null || lineGesture.compressedSwipeSegmentsList[i] == null)))) {
				
					//Debug.Log("LineSample:GestureLineSwipe " + i  + " are good");
					GUILine.Factory().AddGoodPoints(fingerLines);
				}
				else {
					//Debug.Log("LineSample:GestureLineSwipe " + i  + " is bad or line has compressed");
					GUILine.Factory().AddLinePoints(fingerLines);
				
					if (lineGesture.swipedLineType != LineFactory.LineType.None) {
						List<Vector3> fingerLinesFinal = new List<Vector3>();
						//Debug.Log("LineSample:GestureLineSwipe " + i  + " line has compressed");
						if (lineGesture.swipeList.Count > 1) {
							if (lineGesture.compressedSwipeSegmentsList != null) {
								LoadFinalSegments((SwipeSegmentList)lineGesture.compressedSwipeSegmentsList[i], fingerLinesFinal, null);
							}
							else {
								//Debug.Log("LineSample:GestureLineSwipe " + i  + " *** error **** lineGesture.compressedSwipeSegmentsList is null");
							}
						}
						else {
							LoadFinalSegments((SwipeSegmentList)lineGesture.compressedSwipeSegments, fingerLinesFinal, null);
						}
						GUILine.Factory().AddGoodPoints(fingerLinesFinal);
					}
				}
			}
		}
		
	}
	
	public void GestureLineSwipeFailure(LineGesture gesture)
	{
		GestureLineSwipe(gesture);
	}
	
	private void AddFingerLinesGUI(Vector2 screenPoint, List<Rect> numberList)
	{
		if (numberList != null) {
			Vector2 guiPoint = Sample.ScreenToGuiPoint(screenPoint);
			numberList.Add(new Rect(guiPoint.x, guiPoint.y, 25, boxHeight));
		}
	}
	
	private void LoadFinalSegments(SwipeSegmentList segments, List<Vector3> linesList, List<Rect> numberList)
	{
		linesList.Clear();
		if (numberList != null) numberList.Clear();
		if (segments == null) {			
			//Debug.Log("LineSample:LoadFinalSegments segments give are null");
			return;
		}
		//Debug.Log("LineSample:LoadFinalSegments loading " +  segments.Count);
		for (int i = 0; i < segments.Count; i++) {
			SwipeSegment swipeSegment = segments[i];
			if (linesList.Count == 0) {
				linesList.Add(Sample.ScreenToWorldPosition(swipeSegment.startPosition, lineZ));
				if (numberList != null) AddFingerLinesGUI(swipeSegment.startPosition, numberList);
				//Debug.Log("LineSample:LoadFinalSegments GestureLineSwipe add start" + i   + " " + linesList[linesList.Count -1]);
			}
			linesList.Add(Sample.ScreenToWorldPosition(swipeSegment.endPosition, lineZ));
			if (numberList != null) AddFingerLinesGUI(swipeSegment.endPosition, numberList);
			//Debug.Log("LineSample:LoadFinalSegments GestureLineSwipe add " + i  + " count "  + " " + linesList[linesList.Count -1]);
		}
	}
	
	private void ClearAllPoints()
	{
		fingerLines.Clear();
		fingerLinesGUI.Clear();
		GUILine.Factory().ClearAllPoints();
		GUILine.Factory().ClearAllGoodPoints();
		fingerLinesGUIList.Clear();
		GUILine.Factory().SetLineColor(GUILine.DrawColor.Normal);
	}
	
	void GestureStartTouch(TouchGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen || gesture.fingerCount > 1) {
			return;
		}

		isDown = true;
		//Debug.Log("LineSample:GestureStartTouch");
		if (lineGesture.performingSwipe) {
			if (fingerLines.Count > 0) {
				fingerLinesGUIList.Add(fingerLinesGUI);
				fingerLinesGUI = new List<Rect>();
				GUILine.Factory().AddLinePoints(fingerLines);
				fingerLines = new List<Vector3>();
				GUILine.Factory().SetLineColor(GUILine.DrawColor.Normal);
			}
		}
		else {
			ClearAll();
		}
		
		lastPosition = Vector2.zero;
		fingerLines.Clear();
		GUILine.Factory().AddLinePoints(fingerLines);
		fingerLinesGUI.Clear();
		fingerLinesGUIList.Add(fingerLinesGUI);
		fingerLines.Add(Sample.ScreenToWorldPosition(gesture.finger.position, lineZ));
		fingerLines.Add(Sample.ScreenToWorldPosition(gesture.finger.position, lineZ));
		onFirstLine = true;
//		FingerParticle.Factory().showTrail = true;
	}
	
	void GestureMoveTouch(TouchGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen || lockPositions || !isDown || fingerLines.Count < 2 || gesture.fingerCount > 1) {
			return;
		}
		
		//Debug.Log("LineSample:GestureMoveTouch " + gesture.finger.position);
		fingerLines[fingerLines.Count - 1] = Sample.ScreenToWorldPosition(gesture.finger.position, lineZ);
	}
	void GestureEndTouch(TouchGesture gesture)
	{
//		FingerParticle.Factory().showTrail = false;
		isDown = false;
	}
}

