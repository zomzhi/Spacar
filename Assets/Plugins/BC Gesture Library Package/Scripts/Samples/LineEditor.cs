using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineEditor : Sample
{
	public float maxSqrIgnoreLength = 2f;
	public float maxSqrAttachLength = 0.15f;
	
	public GUIStyle listTextStyle;
	public GUIStyle dataTextStyle;
	public GUIStyle errorTextStyle;
	
	
	private LineGesture lineGesture;
	private ComboControl fingerStartLocationCombo;
	private ComboControl fingerEndLocationCombo;
	private ComboControl identificationCombo;
	private ComboControl restrictDirectionCombo;
	private ComboControl lineFactoryCombo;
	
//	private Rect settingsLabelRect;
//	private Rect fingerStartLocationLabelRect;
//	private Rect fingerStartLocationListRect;
//	private Rect fingerEndLocationLabelRect;
//	private Rect fingerEndLocationListRect;
//	private Rect identificationLabelRect;
//	private Rect identificationBoxRect;
//	private Rect restrictDirectionLabelRect;
//	private Rect restrictDirectionBoxRect;
//	private Rect lineFactoryAddLabelRect;
//	private Rect lineFactoryAddBoxRect;
//	private Rect lineFactoryLabelRect;
//	private Rect lineFactoryBoxRect;
//	private Rect returnSwipeAlwaysLabelRect;
//	private Rect returnSwipeAlwaysBoxRect;
//	
//	private Rect resultLabelRect;
//	private Rect eventLabelRect;
//	private Rect eventBoxRect;
//	private Rect fingerLabelRect;
//	private Rect fingerBoxRect;	
	private Rect lineSegmentLabelRect;
	private Rect lineSegmentHitRect;
	
	private Rect addLabelRect;
	private Rect addCheckRect;
	
	
	
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
	
	private bool addLines = true;
	private float horizontalMinimum;	
	private float horizontalMaximum;	
	private float veritcalMinimum;
	private float veritcalMaximum;
	
	protected override void SetupData ()
	{
		veritcalMinimum = 10;
		veritcalMaximum = Screen.height - SampleGUI.topBarHeight - 15f; 
		horizontalMinimum = SampleGUI.guiBarWidth + 35f;
		horizontalMaximum = Screen.width - 10;
		SampleGUI.sceneSelectionOpen = false;
		SampleGUI.aSceneHasBeenPicked = true;
		SampleGUI.doMenuButtons = false;
		lineGesture = this.gameObject.GetComponentInChildren<LineGesture>();
		
//		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
//		
//		restrictDirectionLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 120f, boxHeight);
//		float boxX = propLabelX + 105f;
//		restrictDirectionBoxRect = new Rect(boxX, restrictDirectionLabelRect.y, 120f, boxHeight);
//		lineFactoryAddLabelRect = new Rect(propLabelX, restrictDirectionBoxRect.yMax + betweenYGap, 120f, boxHeight);
//		lineFactoryAddBoxRect = new Rect(boxX, lineFactoryAddLabelRect.y, checkboxSize, checkboxSize);
//		lineFactoryAddLabelRect.y += labelCenterOffset;
//		identificationLabelRect = new Rect(propLabelX, lineFactoryAddBoxRect.yMax + betweenYGap, 120f, boxHeight);
//		identificationBoxRect = new Rect(boxX, identificationLabelRect.y, 120f, boxHeight);
//		lineFactoryLabelRect = new Rect(propLabelX, identificationBoxRect.yMax + betweenYGap, 120f, boxHeight);
//		lineFactoryBoxRect = new Rect(boxX, lineFactoryLabelRect.y, 120f, boxHeight);
//		
//		returnSwipeAlwaysLabelRect = new Rect(propLabelX, lineFactoryBoxRect.yMax + betweenYGap, 120f, boxHeight);
//		returnSwipeAlwaysBoxRect = new Rect(boxX, returnSwipeAlwaysLabelRect.y, checkboxSize, checkboxSize);
//		returnSwipeAlwaysLabelRect.y += labelCenterOffset;
//		
//		fingerStartLocationLabelRect = new Rect(propLabelX, returnSwipeAlwaysBoxRect.yMax + betweenYGap, 100f, boxHeight);
//		fingerStartLocationListRect = new Rect(boxX, fingerStartLocationLabelRect.y, 120f, boxHeight);
//		fingerEndLocationLabelRect = new Rect(propLabelX, fingerStartLocationListRect.yMax + betweenYGap, 100f, boxHeight);
//		fingerEndLocationListRect = new Rect(boxX, fingerEndLocationLabelRect.y, 120f, boxHeight);
//		
//		resultLabelRect = new Rect(sectionX, fingerEndLocationListRect.yMax + resultLabelAfterGap, 100f, titleHeight);
//		boxX = propLabelX + 100f;
//		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
//		eventBoxRect = new Rect(boxX - 25, eventLabelRect.y, 200f, boxResultHeight);
//		fingerLabelRect = new Rect(propLabelX, eventBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
//		fingerBoxRect = new Rect(boxX, fingerLabelRect.y, 100f, boxResultHeight);
		float editorPropLabelX = 15f;
		float boxX = editorPropLabelX + 105f;
		addLabelRect = new Rect(editorPropLabelX, sectionY, 120f, boxHeight);
		addCheckRect = new Rect(boxX, addLabelRect.y, checkboxSize, checkboxSize);
		addLabelRect.y += labelCenterOffset;
		
		clearResultButtonRect = new Rect(propLabelX, addCheckRect.yMax + betweenYGap + betweenYGap, clearButtonWidth, clearButtonHeight);
		
		
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
		
		//FingerControl._delegateSwipeSegmentCreate += OnSwipeSegmentCreate;
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
		
//			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
//			
//			// draw line types before lists so they be under
//			GUI.Label(lineFactoryListLabelRect, "Line types:", listTextStyle);
//			if (lineGesture.lineFactoryLineType != null) {
//				for (int i = 0; i < lineGesture.lineFactoryLineType.Length; i++) {
//					GUI.Label(lineFactoryListRects[i], lineGesture.lineFactoryLineType[i].ToString(), listTextStyle);
//				}
//			}
//			
//						
//			GUI.Label(lineFactoryAddLabelRect, "Types additive:", settingsTextStyle);
//			
//			typesAdditive = GuiCheckBox(lineFactoryAddBoxRect, typesAdditive);
//			
//			GUI.Label(returnSwipeAlwaysLabelRect, "Always Return:", settingsTextStyle);
//			
//			lineGesture.returnSwipeAlways = GuiCheckBox(returnSwipeAlwaysBoxRect, lineGesture.returnSwipeAlways);
//						
//			lineGesture.startsOnObject = (BaseGesture.FingerLocation) fingerStartLocationCombo.GUIShowList("Start:", fingerStartLocationLabelRect, fingerStartLocationListRect);
//			lineGesture.endsOnObject = (BaseGesture.FingerLocation) fingerEndLocationCombo.GUIShowList("End:", fingerEndLocationLabelRect, fingerEndLocationListRect);
//			//Debug.Log("SwipepingSample:OnGUI fingerLocation " + lineGesture.fingerLocation);
//			
//			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);			
//			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);									
//			GUI.Label(fingerLabelRect, "Finger used:", resultsTextStyle);
//			GUI.Label(lineSegmentLabelRect, "Segments:", dataTextStyle);
			GUI.Label(addLabelRect, "Add lines:", settingsTextStyle);
			UpdateAddLines(GuiCheckBox(addCheckRect, addLines));	
			
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
//				GUI.Label(eventBoxRect, "Received " + lineGesture.swipedLineType.ToString(), resultsTextStyle);
//				GUI.Label(fingerBoxRect, lineGesture.finger.Index().ToString(), resultsTextStyle);
//				
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
//				else {
//					GUI.Label(lineSegmentHitRect, lineGesture.swipedLineType.ToString(), dataTextStyle);
//				}
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
			
			
//			lineGesture.restrictLineSwipeDirection = (LineGesture.LineSwipeDirection) restrictDirectionCombo.GUIShowList("Direction:", restrictDirectionLabelRect, restrictDirectionBoxRect);
//			lineGesture.lineIdentification = (LineGesture.LineIdentification) identificationCombo.GUIShowList("Identification:", identificationLabelRect, identificationBoxRect);
//			lineType = (LineFactory.LineType) lineFactoryCombo.GUIShowList("Type:", lineFactoryLabelRect, lineFactoryBoxRect);
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
	//private Vector2 lastPosition = Vector2.zero;
	//private bool onFirstLine = false;
	private bool lockPositions = false;

//	public void OnSwipeSegmentCreate(SwipeSegment segment)
//	{
//		if (!isDown || segment.startPosition == Vector2.zero || segment.endPosition == Vector2.zero) {
//			return;
//		}
//		if (lineGesture.ActiveCount() > 1) {
//			//ClearAll();
//			return;
//		}
//		lockPositions = true;
//		
//		if (fingerLines.Count == 0 || lastPosition == Vector2.zero || lastPosition.x != segment.startPosition.x || lastPosition.y != segment.startPosition.y) {
//			//Debug.Log("LineSample:OnSwipeSegmentCreate adding start position " + fingerLines.Count + " " + segment.startPosition);
//			if (onFirstLine) {
//				fingerLines[0] = Sample.ScreenToWorldPosition(segment.startPosition, lineZ);
//				//Debug.Log("LineSample:OnSwipeSegmentCreate onFirstLine update start 0 " + fingerLines[0]);
//			}
//			else {
//				fingerLines.Add(Sample.ScreenToWorldPosition(segment.startPosition, lineZ));
//				//Debug.Log("LineSample:OnSwipeSegmentCreate  add start " + fingerLines[fingerLines.Count - 1]);
//			}
//			AddFingerLinesGUI(segment.startPosition, fingerLinesGUI);
//		}
//		if (onFirstLine) {
//			fingerLines[1] = Sample.ScreenToWorldPosition(segment.endPosition, lineZ);
//			fingerLines.Add(fingerLines[1]);
//			//Debug.Log("LineSample:OnSwipeSegmentCreate onFirstLine update end 1 " + fingerLines[1]);
//			onFirstLine = false;
//		}
//		else {
//			fingerLines[fingerLines.Count - 1] = Sample.ScreenToWorldPosition(segment.endPosition, lineZ);
//			fingerLines.Add(fingerLines[fingerLines.Count - 1]);
//			//Debug.Log("LineSample:OnSwipeSegmentCreate  add end 1 " + fingerLines[fingerLines.Count - 1]);
//		}
//		AddFingerLinesGUI(segment.endPosition, fingerLinesGUI);
//		lastPosition = segment.endPosition;
//	
//		//Debug.Log("LineSample:OnSwipeSegmentCreate " + fingerLines.Count + " " + segment.startPosition + " to " + segment.endPosition + " - " + fingerLines[fingerLines.Count - 1]);
//		lockPositions = false;
//	}
	
	
	private void ClearAll()
	{
		gestureOccurred = false;
		ClearAllPoints();
	}
	
	private bool isDown = true;
	private bool testing = false;
	// Message from LineGesture
	void GestureLineSwipe(LineGesture gesture)
	{
		if (!testing || SampleGUI.sceneSelectionOpen) {
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
		previousPoint = Vector2.zero;
		currentPoint = Vector2.zero;
		lastPoint = Vector3.zero;
	}
	
	private void UpdateAddLines(bool returned)
	{
		if (returned == addLines) {
			return;
		}
		addLines = returned;
		GUILine.Factory().SetOneOffLineColor(-1, GUILine.DrawColor.Normal);
		selectedSegmentList = -1;
		movingPoint = false;
		if (addLines) {
		}
		else {
			previousPoint = Vector2.zero;
			currentPoint = Vector2.zero;
			lastPoint = Vector3.zero;
		}
	}

	
	Vector2 previousPoint = Vector2.zero;
	Vector2 currentPoint = Vector2.zero;
	Vector3 lastPoint = Vector3.zero;
	int movingLinePointIndex;
	UserSegmentList segmentList;
	UserSegment currentSegment;
	List<UserSegmentList> compoundSegmentLists = null;
	SwipeGesture.SwipeDirection currentDirection;
	bool addingNewLine = false;
	bool movingPoint = false;
	int selectedSegmentList = -1;
	int selectedSegmentListPoint;
	bool selectedSegmentListOnStart;
	
	
	void GestureStartTouch(TouchGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen || gesture.fingerCount > 1 || !PointIsValid(gesture.finger.startPosition)) {
			Debug.Log("LineSample:GestureStartTouch down rejected");
			return;
		}
		
		isDown = true;
		
		Vector2 thisPoint = gesture.finger.position;
		previousPoint = thisPoint;
		currentPoint = thisPoint;
		
		selectedSegmentList = -1;
		movingPoint = false;
		if (addLines) {
			// look for any existing points to attach to or move
			if (lastPoint != Vector3.zero && HitMatch(lastPoint, thisPoint) ) {
				Debug.Log("LineSample:GestureStartTouch attach to end of previous");
				fingerLines.Add(new Vector3(lastPoint.x, lastPoint.y, lastPoint.z));
				movingLinePointIndex = fingerLines.Count - 1;
				addingNewLine = false;
			}
			    	
			else {		
			//addLines
			
			//Debug.Log("LineSample:GestureStartTouch");
			//if (lineGesture.performingSwipe) {
				// a new line
				if (fingerLines == null || fingerLines.Count > 0) {
					fingerLinesGUIList.Add(fingerLinesGUI);
					fingerLinesGUI = new List<Rect>();
					fingerLines = new List<Vector3>();
					GUILine.Factory().SetLineColor(GUILine.DrawColor.Normal);
				}
				
				addingNewLine = true;
				fingerLines.Clear();
				GUILine.Factory().AddLinePoints(fingerLines);
				fingerLinesGUI.Clear();
				fingerLinesGUIList.Add(fingerLinesGUI);
				fingerLines.Add(Sample.ScreenToWorldPosition(previousPoint, lineZ));
				fingerLines.Add(Sample.ScreenToWorldPosition(previousPoint, lineZ));
				movingLinePointIndex = fingerLines.Count - 1;
				
				if (compoundSegmentLists == null) {
					compoundSegmentLists = new List<UserSegmentList>();
				}
				GUILine.Factory().SetOneOffLineColor(compoundSegmentLists.Count, GUILine.DrawColor.Drawing);
			}
		}
		
		else if (compoundSegmentLists != null) {
			for (int i = 0; i < compoundSegmentLists.Count; i++) {
				int p;
				UserSegmentList segmentList = compoundSegmentLists[i];
				for (p = 0; p < segmentList.Count; p++) {
					Debug.Log("LineEditor:GestureStartTouch compoundSegmentLists " + i + " segmentList " + p + " startPoint " + segmentList[p].startPoint);
					if (HitMatch(segmentList[p].endPoint, thisPoint)) {
						//Debug.Log("LineEditor:GestureStartTouch compoundSegmentLists matched startPoint" + i + " segmentList " + p);
						selectedSegmentListOnStart = false;
						break;
					}
					else if (p == 0 && HitMatch(segmentList[0].startPoint, thisPoint)) {
						Debug.Log("LineEditor:GestureStartTouch compoundSegmentLists matched endPoint " + i + " segmentList " + p);
						selectedSegmentListOnStart = true;
						break;
					}
				}
				if (p < segmentList.Count) {
					Debug.Log("LineEditor:GestureStartTouch compoundSegmentLists selected " + i + " " + p);
					fingerLines = GUILine.Factory().GetLine(i);
					if (fingerLines == null) {
						Debug.Log("LineEditor:GestureStartTouch ***** ERROR ***** selected GUI line point " + i + " " + p + " does not exist");
						return;
					}
					currentSegment = segmentList[i];
					segmentList = compoundSegmentLists[i];
					selectedSegmentList = i;
					selectedSegmentListPoint = p;
					GUILine.Factory().SetOneOffLineColor(selectedSegmentList, GUILine.DrawColor.Selected);
					movingPoint = true;
					if (selectedSegmentListOnStart) {
						movingLinePointIndex = 0;
						previousPoint = WorldToScreenPosition(currentSegment.endPoint);
					}
					else {
						movingLinePointIndex = p + 1;
						previousPoint = WorldToScreenPosition(currentSegment.startPoint);
					}
					
					break;
				}
			}
			if (selectedSegmentList < 0) {
				GUILine.Factory().SetOneOffLineColor(-1, GUILine.DrawColor.Normal);
			}
		}
			
		
		//lastPosition = Vector2.zero;
		
		//onFirstLine = true;
//		FingerParticle.Factory().showTrail = true;
	}
	
	void GestureMoveTouch(TouchGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen || lockPositions || !isDown || gesture.fingerCount > 1) {
			return;
		}
		
		Vector2 touchPosition = gesture.finger.position;
			
		if (addLines || movingPoint) {
			if (previousPoint == Vector2.zero || fingerLines == null || fingerLines.Count < 2) {
				return;
			}
			currentDirection = FingerControl.GetSwipeDirection(previousPoint, touchPosition);
	// convert to world points - previous point from world is wrong		
			currentPoint = CalcDirectionPosition(currentDirection, previousPoint, touchPosition);			
			Vector3 worldPos = Sample.ScreenToWorldPosition(currentPoint, lineZ);
			switch(currentDirection) {
				case SwipeGesture.SwipeDirection.Down:
				case SwipeGesture.SwipeDirection.Up:
					worldPos.x = fingerLines[fingerLines.Count - 2].x;
					break;				
				case SwipeGesture.SwipeDirection.Right:
				case SwipeGesture.SwipeDirection.Left:
					worldPos.y = fingerLines[fingerLines.Count - 2].y;
					break;					
				default:
					break;
			}
			fingerLines[movingLinePointIndex] = worldPos;
			if (movingPoint) {
				segmentList.UpdatePoint(selectedSegmentListPoint, worldPos, selectedSegmentListOnStart);
			}
		}
	}
	
	void GestureEndTouch(TouchGesture gesture)
	{
//		FingerParticle.Factory().showTrail = false;
		//Debug.Log("LineSample:GestureEndTouch fingerLines.Count=" + fingerLines.Count);
		if (addLines) {
			if (fingerLines == null || fingerLines.Count < 2 || previousPoint == Vector2.zero) {
				return;
			}
				
			Vector3 previous = fingerLines[movingLinePointIndex - 1];
			Vector3 current = fingerLines[movingLinePointIndex];
			Vector3 diff = current - previous;
			if (diff.sqrMagnitude < maxSqrIgnoreLength) {
				previousPoint = Vector2.zero;
				currentPoint = Vector2.zero;
				lastPoint = Vector3.zero;
				fingerLines.RemoveRange(fingerLines.Count - 2, 2);
				if (fingerLines.Count == 0) {
					GUILine.Factory().PopPoints();
				}
				Debug.Log("LineSample:GestureEndTouch end too small - segment removed " + diff.sqrMagnitude);
				return;
			}
		
			lastPoint = current;
			if (segmentList == null || addingNewLine) {
				segmentList = new UserSegmentList();
			}
			segmentList.AddSegment(new UserSegment(currentDirection, previous, current));
			if (addingNewLine) {
				compoundSegmentLists.Add(segmentList);
			}
		}
		
		isDown = false;
	}
	
	private Vector2 CalcDirectionPosition(SwipeGesture.SwipeDirection direction, Vector2 fromPos, Vector2 toPos) 
	{
		if (toPos.x < horizontalMinimum) {
			toPos.x = horizontalMinimum;
		}
		if (toPos.x > horizontalMaximum) {
			toPos.x = horizontalMaximum;
		}
		if (toPos.y < veritcalMinimum) {
			toPos.y = veritcalMinimum;
		}
		if (toPos.y > veritcalMaximum) {
			toPos.y = veritcalMaximum;
		}
		Vector2 newPos = toPos;
		switch(direction) {
			case SwipeGesture.SwipeDirection.Down:
			case SwipeGesture.SwipeDirection.Up:
				return new Vector2(fromPos.x, toPos.y);
				
			case SwipeGesture.SwipeDirection.Right:
			case SwipeGesture.SwipeDirection.Left:
				return new Vector2(toPos.x, fromPos.y);
					
				
			case SwipeGesture.SwipeDirection.Plus45:
				newPos = new Vector2(1, 1);
				break;
			case SwipeGesture.SwipeDirection.Plus135:
				newPos = new Vector2(1, -1);
				break;
			case SwipeGesture.SwipeDirection.Minus135:
				newPos = new Vector2(-1, -1);
				break;
			case SwipeGesture.SwipeDirection.Minus45:
				newPos = new Vector2(-1, 1);
				break;
			
			case SwipeGesture.SwipeDirection.Any:
			case SwipeGesture.SwipeDirection.None:
				return toPos;
		}
		Vector2 diff = toPos - fromPos;
		bool isValid = false;
		float reduceBy = 0.75f;
		Vector2 returnPos = newPos;
		while (!isValid && reduceBy > .3f) {
			returnPos = (newPos * Mathf.Abs(diff.magnitude) * reduceBy) + fromPos;
			isValid = PointIsValid(returnPos);
			reduceBy -= 0.01f;
		}
		if (!isValid) {
			Debug.Log("LineEditor:CalcDirectionPosition was not valid");
			returnPos = (newPos * Mathf.Abs(diff.magnitude) * 0.3f) + fromPos;
		}
		//Debug.Log("LineEditor:CalcDirectionPosition " + direction + " diff=" + diff + ", newPos=" + newPos + ", fromPos=" + fromPos+ ", toPos=" + toPos);
		return returnPos;
	}
	
	private bool PointIsValid(Vector2 pos)
	{
		if (SampleGUI.sceneSelectionOpen || 
		    pos.x < horizontalMinimum ||
		    pos.y > veritcalMaximum ||
		   	pos.y < veritcalMinimum ||
		    pos.x > horizontalMaximum) {
			return false;
		}
		return true;
	}
	
	private bool HitMatch(Vector3 point1, Vector2 point2)
	{
		Vector3 point2World = Sample.ScreenToWorldPosition(point2, lineZ);
		Vector3 diff = point1 - point2World;
		if (diff.sqrMagnitude < maxSqrAttachLength) {
			//Debug.Log("LineEditor:HitMatch success " + diff.sqrMagnitude + " " + point1 + " " + point2World);
			return true;
		}
		//Debug.Log("LineEditor:HitMatch failure " + diff.sqrMagnitude + " " + point1 + " " + point2World);
		return false;
	}


}

