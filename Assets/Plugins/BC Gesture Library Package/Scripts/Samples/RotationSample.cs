using UnityEngine;
using System.Collections;

public class RotationSample : Sample
{
	
	public float rotateScaleFactor = 0.01f;
	public Transform rotateBoxTransform;
	
	private RotateGesture rotateGesture;
	private ComboControl fingerLocationCombo;
	private ComboControl rotateAxisCombo;
	private ComboControl restrictFingersCombo;

	private Rect settingsLabelRect;
	private Rect fingerLocationLabelRect;
	private Rect fingerLocationListRect;
	private Rect rotateAxisLabelRect;
	private Rect rotateAxisListRect;
	private Rect restrictFingerCountLabelRect;	
	private Rect restrictFingerCountBoxRect;	
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect rotateLabelRect;
	private Rect rotateBoxRect;
	private Rect fingerCountLabelRect;
	private Rect fingerCountBoxRect;
	private Rect rotateFinger1LabelRect;
	private Rect rotateFinger1BoxRect;
	private Rect rotateFinger2LabelRect;
	private Rect rotateFinger2BoxRect;
	
	private Rect clearResultButtonRect;
	private bool gestureOccurred = false;
	
	protected override void SetupData ()
	{
		rotateGesture = this.gameObject.GetComponentInChildren<RotateGesture>();
						
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		
		rotateAxisLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 120f, boxHeight);
		float boxX = propLabelX + 90f;
		rotateAxisListRect = new Rect(boxX, rotateAxisLabelRect.y, 105f, boxHeight);
		fingerLocationLabelRect = new Rect(propLabelX, rotateAxisListRect.yMax + betweenYGap, 100f, boxHeight);
		fingerLocationListRect = new Rect(boxX, fingerLocationLabelRect.y, 120f, boxHeight);
		restrictFingerCountLabelRect = new Rect(propLabelX, fingerLocationListRect.yMax + betweenYGap, 100f, boxHeight);
		restrictFingerCountBoxRect = new Rect(boxX - 15, restrictFingerCountLabelRect.y, 135f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, restrictFingerCountLabelRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = propLabelX + 100f;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX, eventLabelRect.y, 300f, boxResultHeight);		
		rotateLabelRect = new Rect(propLabelX, eventBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		rotateBoxRect = new Rect(boxX, rotateLabelRect.y, 120f, boxResultHeight);
		fingerCountLabelRect = new Rect(propLabelX, rotateBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		fingerCountBoxRect = new Rect(boxX, fingerCountLabelRect.y, 120f, boxResultHeight);
		rotateFinger1LabelRect = new Rect(propLabelX, fingerCountBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		rotateFinger1BoxRect = new Rect(boxX, rotateFinger1LabelRect.y, 120f, boxResultHeight);
		rotateFinger2LabelRect = new Rect(propLabelX, rotateFinger1LabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		rotateFinger2BoxRect = new Rect(boxX, rotateFinger2LabelRect.y, 120f, boxResultHeight);
		
		clearResultButtonRect = new Rect(propLabelX, rotateFinger2LabelRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
	}
	private bool firstOnGUI = true;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			fingerLocationCombo = new ComboControl();
			fingerLocationCombo.SetFingerLocationList((int) rotateGesture.fingerLocation, true);
			rotateAxisCombo = new ComboControl();
			rotateAxisCombo.SetRotateAxisList((int) rotateGesture.rotateAxis);
			restrictFingersCombo = new ComboControl();
			restrictFingersCombo.SetFingerCountList((int) rotateGesture.restrictFingerCount);
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		
		if (SampleGUI.sceneSelectionOpen) {
			rotateGesture.doRotate = false;
		}
		else {
			rotateGesture.doRotate = true;
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			rotateGesture.rotateAxis = (RotateGesture.RotateAxis) rotateAxisCombo.GUIShowList("Axis:", rotateAxisLabelRect, rotateAxisListRect);
			rotateGesture.fingerLocation = (BaseGesture.FingerLocation) fingerLocationCombo.GUIShowList("Location:", fingerLocationLabelRect, fingerLocationListRect);
			rotateGesture.restrictFingerCount = (BaseGesture.FingerCountRestriction) restrictFingersCombo.GUIShowList("Fingers:", restrictFingerCountLabelRect, restrictFingerCountBoxRect);
			//Debug.Log("SwipepingSample:OnGUI fingerLocation " + swipeGesture.fingerLocation);
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);					
			GUI.Label(rotateLabelRect, "Rotation:", resultsTextStyle);
			GUI.Label(fingerCountLabelRect, "Finger Count:", resultsTextStyle);
			GUI.Label(rotateFinger1LabelRect, "Finger1:", resultsTextStyle);
			GUI.Label(rotateFinger2LabelRect, "Finger2:", resultsTextStyle);
			
			if (gestureOccurred) {
				GUI.Label(eventBoxRect, currentCall, resultsTextStyle);			
				GUI.Label(rotateBoxRect, this.transform.rotation.ToString(), resultsTextStyle);
				GUI.Label(fingerCountBoxRect, rotateGesture.fingerCount.ToString(), resultsTextStyle);
				GUI.Label(rotateFinger1BoxRect, rotateGesture.rotateFinger1.position.ToString(), resultsTextStyle);
				if (rotateGesture.rotateFinger2 != null) {
					GUI.Label(rotateFinger2BoxRect, rotateGesture.rotateFinger2.position.ToString(), resultsTextStyle);
				}
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					gestureOccurred = false;
					rotateGesture.RestoreObject();
				}
			}
//			
//			GUI.Label(maxTimeLabelRect, "Max Time Between:");
//			string maxValueStr = GUI.TextField(maxTimeBoxRect, swipeGesture.maxTimeBetweensSwipes.ToString(), 5);
//			swipeGesture.maxTimeBetweensSwipes = ConvertToPositiveNonZeroFloat(maxValueStr, swipeGesture.maxTimeBetweensSwipes);
		}
	}
	
	private string currentCall;
	private int moveIndex = 0;

	void GestureStartRotate (RotateGesture gesture)
	{
		gestureOccurred = true;
		currentCall = "Start";
		moveIndex = 0;
	}
	
	void GestureMoveRotate (RotateGesture gesture)
	{
		gestureOccurred = true;
		moveIndex++;
		currentCall = "Rotate " + moveIndex;
	}
	
	void GestureEndRotate (RotateGesture gesture)
	{
		currentCall = "End";
		moveIndex = 0;
	}

	
}

