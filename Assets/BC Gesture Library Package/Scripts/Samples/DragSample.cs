using UnityEngine;
using System.Collections;

public class DragSample : Sample
{
	
	private DragGesture dragGesture;
	private ComboControl fingerLocationCombo;
	private ComboControl dragPositionCombo;
	private ComboControl restrictFingersCombo;
	
	private Rect settingsLabelRect;
	private Rect fingerLocationLabelRect;
	private Rect fingerLocationListRect;
	private Rect restrctFingerCountLabelRect;
	private Rect restrctFingerCountBoxRect;
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect dragPositionLabelRect;
	private Rect dragPositionBoxRect;
	private Rect fingerLabelRect;
	private Rect fingerBoxRect;
	private Rect fingerCountLabelRect;
	private Rect fingerCountBoxRect;
	private Rect swipeFingerCountLabelRect;
	private Rect swipeFingerCountBoxRect;
	private Rect startLabelRect;
	private Rect startBoxRect;
	private Rect endLabelRect;
	private Rect endBoxRect;
	private Rect clearResultButtonRect;
	
	private bool gestureTapOccurred = false;
	private string dragEventStr;
	
	protected override void SetupData ()
	{
		
		dragGesture = this.gameObject.GetComponentInChildren<DragGesture>();		

		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		fingerLocationLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 100f, boxHeight);
		float boxX = fingerLocationLabelRect.x + 70f;
		fingerLocationListRect = new Rect(boxX, fingerLocationLabelRect.y, 135f, boxHeight);
		dragPositionLabelRect = new Rect(propLabelX, fingerLocationListRect.yMax + betweenYGap, 100f, boxHeight);
		dragPositionBoxRect = new Rect(boxX, dragPositionLabelRect.y, 135f, boxHeight);
		restrctFingerCountLabelRect = new Rect(propLabelX, dragPositionBoxRect.yMax + betweenYGap, 150f, boxHeight);
		restrctFingerCountBoxRect = new Rect(boxX, restrctFingerCountLabelRect.y, 145f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, restrctFingerCountBoxRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = resultLabelRect.x + 100f;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX, eventLabelRect.y, 150f, boxResultHeight);

		fingerLabelRect = new Rect(propLabelX, eventLabelRect.yMax + betweenYGap, 100f, boxResultHeight);
		fingerBoxRect = new Rect(boxX, fingerLabelRect.y, 50f, boxResultHeight);
		fingerCountLabelRect = new Rect(propLabelX, fingerBoxRect.yMax + betweenYGap, 100f, boxResultHeight);
		fingerCountBoxRect = new Rect(boxX, fingerCountLabelRect.y, 50f, boxResultHeight);
		startLabelRect = new Rect(propLabelX, fingerCountBoxRect.yMax + betweenYGap, 100f, boxResultHeight);
		startBoxRect = new Rect(boxX, startLabelRect.y, 150f, boxResultHeight);
		endLabelRect = new Rect(propLabelX, startLabelRect.yMax + betweenYGap, 100f, boxResultHeight);
		endBoxRect = new Rect(boxX, endLabelRect.y, 150f, boxResultHeight);
			
		clearResultButtonRect = new Rect(propLabelX, endLabelRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
	}
	

	private bool firstOnGUI = true;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			fingerLocationCombo = new ComboControl();
			fingerLocationCombo.SetFingerLocationList((int) dragGesture.fingerLocation, false);
			dragPositionCombo = new ComboControl();
			dragPositionCombo.SetDragPositionList((int) dragGesture.dragPosition);
			restrictFingersCombo = new ComboControl();
			restrictFingersCombo.SetFingerCountList((int) dragGesture.restrictFingerCount);
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		if (SampleGUI.sceneSelectionOpen) {
			dragGesture.doDrag = false;
		}
		else {
			dragGesture.doDrag = true;
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			
			dragGesture.fingerLocation = (BaseGesture.FingerLocation) fingerLocationCombo.GUIShowList("Location:", fingerLocationLabelRect, fingerLocationListRect);
			dragGesture.dragPosition = (DragGesture.DragPosition) dragPositionCombo.GUIShowList("Dragging:", dragPositionLabelRect, dragPositionBoxRect);
			dragGesture.restrictFingerCount = (BaseGesture.FingerCountRestriction) restrictFingersCombo.GUIShowList("Fingers:", restrctFingerCountLabelRect, restrctFingerCountBoxRect);
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);			
			GUI.Label(fingerLabelRect, "Finger used:", resultsTextStyle);
			GUI.Label(fingerCountLabelRect, "Finger count:", resultsTextStyle);
			GUI.Label(startLabelRect, "Start point:", resultsTextStyle);
			GUI.Label(endLabelRect, "End point:", resultsTextStyle);
			
			if (gestureTapOccurred) {
				GUI.Label(eventBoxRect, dragEventStr, resultsTextStyle);			
				GUI.Label(startBoxRect, dragGesture.startPoint.ToString(), resultsTextStyle);			
				GUI.Label(endBoxRect, dragGesture.endPoint.ToString(), resultsTextStyle);			
				GUI.Label(fingerBoxRect, dragGesture.finger.Index().ToString(), resultsTextStyle);
				GUI.Label(fingerCountBoxRect, dragGesture.dragFingerCount.ToString(), resultsTextStyle);
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					gestureTapOccurred = false;
					dragGesture.RestoreObject();
				}
			}
			
		}
	}
	
	private int moveCount = 0;
	void GestureStartDrag(DragGesture gesture)
	{
		//FingerParticle.Factory().showTrail = true;
		gestureTapOccurred = true;
		dragEventStr = "Start Drag";
		moveCount = 0;
//		FingerParticle.Factory().showTrail = true;
	}
	void GestureMoveDrag(DragGesture gesture)
	{
//		FingerParticle.Factory().showTrail = true;
		gestureTapOccurred = true;
		moveCount++;
		dragEventStr = "Move " + moveCount;
	}
	void GestureEndDrag(DragGesture gesture)
	{
//		FingerParticle.Factory().showTrail = false;
		gestureTapOccurred = true;
		dragEventStr = "End Drag";
	}
		

}

