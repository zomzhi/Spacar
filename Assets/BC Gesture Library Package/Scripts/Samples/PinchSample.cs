using UnityEngine;
using System.Collections;

public class PinchSample : Sample
{
	
	public float pinchScaleFactor = 0.01f;
	public Transform pinchBoxTransform;
	
	private PinchGesture pinchGesture;
	private ComboControl fingerLocationCombo;
	private ComboControl pinchActionCombo;
	private ComboControl restrictDirectionCombo;

	private Rect settingsLabelRect;
	private Rect fingerLocationLabelRect;
	private Rect fingerLocationListRect;
	private Rect pinchActionLabelRect;
	private Rect pinchActionListRect;
	private Rect restrictDirectionLabelRect;
	private Rect restrictDirectionListRect;
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect keepAspectRatioLabelRect;
	private Rect keepAspectRatioBoxRect;
	private Rect pinchDirectionLabelRect;
	private Rect pinchDirectionBoxRect;
	private Rect pinchFinger1LabelRect;
	private Rect pinchFinger1BoxRect;
	private Rect pinchFinger2LabelRect;
	private Rect pinchFinger2BoxRect;
	private Rect pinchMagnitudeDeltaLabelRect;
	private Rect pinchMagnitudeDeltaBoxRect;
	
	private Rect clearResultButtonRect;
	private bool gestureOccurred = false;
	
	protected override void SetupData ()
	{
		pinchGesture = this.gameObject.GetComponentInChildren<PinchGesture>();
				
		
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		float boxX = propLabelX + 80f;
		
		keepAspectRatioLabelRect  = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 200f, boxHeight);
		keepAspectRatioBoxRect = new Rect(boxX + 60f, keepAspectRatioLabelRect.y, checkboxSize, boxHeight);		
		pinchActionLabelRect = new Rect(propLabelX, keepAspectRatioBoxRect.yMax + titleAfterGap, 120f, boxHeight);
		pinchActionListRect = new Rect(boxX, pinchActionLabelRect.y, 110f, boxHeight);
		restrictDirectionLabelRect = new Rect(propLabelX, pinchActionListRect.yMax + betweenYGap, 120f, boxHeight);
		restrictDirectionListRect = new Rect(boxX, restrictDirectionLabelRect.y, 110f, boxHeight);
		fingerLocationLabelRect = new Rect(propLabelX, restrictDirectionListRect.yMax + betweenYGap, 100f, boxHeight);
		fingerLocationListRect = new Rect(boxX, fingerLocationLabelRect.y, 130f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, fingerLocationLabelRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = propLabelX + 100f;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX, eventLabelRect.y, 100f, boxResultHeight);		
		pinchDirectionLabelRect = new Rect(propLabelX, eventBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		pinchDirectionBoxRect = new Rect(boxX, pinchDirectionLabelRect.y, 100f, boxResultHeight);
		pinchFinger1LabelRect = new Rect(propLabelX, pinchDirectionLabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		pinchFinger1BoxRect = new Rect(boxX, pinchFinger1LabelRect.y, 120f, boxResultHeight);
		pinchFinger2LabelRect = new Rect(propLabelX, pinchFinger1LabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		pinchFinger2BoxRect = new Rect(boxX, pinchFinger2LabelRect.y, 120f, boxResultHeight);
		pinchMagnitudeDeltaLabelRect = new Rect(propLabelX, pinchFinger2LabelRect.yMax + betweenResultsYGap, 150f, boxResultHeight);
		pinchMagnitudeDeltaBoxRect = new Rect(boxX, pinchMagnitudeDeltaLabelRect.y, 50f, boxResultHeight);
		
		clearResultButtonRect = new Rect(propLabelX, pinchMagnitudeDeltaLabelRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
	}
	
	private bool firstOnGUI = true;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			fingerLocationCombo = new ComboControl();
			fingerLocationCombo.SetFingerLocationList((int) pinchGesture.fingerLocation, true);
			pinchActionCombo = new ComboControl();
			pinchActionCombo.SetPinchActionList((int) pinchGesture.pinchAction);
			restrictDirectionCombo = new ComboControl();
			restrictDirectionCombo.SetPinchDirectionList((int) pinchGesture.restrictDirection);
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		
		if (!SampleGUI.sceneSelectionOpen) {
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			
			GUI.Label(keepAspectRatioLabelRect, "Keep Aspect Ratio:", settingsTextStyle);			
			pinchGesture.keepAspectRatio = GuiCheckBox(keepAspectRatioBoxRect, pinchGesture.keepAspectRatio);

			pinchGesture.pinchAction = (PinchGesture.PinchAction) pinchActionCombo.GUIShowList("Action:", pinchActionLabelRect, pinchActionListRect);
			pinchGesture.restrictDirection = (PinchGesture.PinchDirection) restrictDirectionCombo.GUIShowList("Direction:", restrictDirectionLabelRect, restrictDirectionListRect);						
			pinchGesture.fingerLocation = (BaseGesture.FingerLocation) fingerLocationCombo.GUIShowList("Location:", fingerLocationLabelRect, fingerLocationListRect);
			//Debug.Log("SwipepingSample:OnGUI fingerLocation " + swipeGesture.fingerLocation);
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);					
			GUI.Label(pinchDirectionLabelRect, "Direction:", resultsTextStyle);
			GUI.Label(pinchFinger1LabelRect, "Finger1:", resultsTextStyle);
			GUI.Label(pinchFinger2LabelRect, "Finger2:", resultsTextStyle);
			GUI.Label(pinchMagnitudeDeltaLabelRect, "Magnitude:", resultsTextStyle);			
			
			if (gestureOccurred) {
				GUI.Label(eventBoxRect, "Received " + currentCall, resultsTextStyle);			
				GUI.Label(pinchDirectionBoxRect, pinchGesture.pinchDirection.ToString(), resultsTextStyle);
				GUI.Label(pinchFinger1BoxRect, pinchGesture.pinchFinger1.position.ToString(), resultsTextStyle);
				GUI.Label(pinchFinger2BoxRect, pinchGesture.pinchFinger2.position.ToString(), resultsTextStyle);
				GUI.Label(pinchMagnitudeDeltaBoxRect, pinchGesture.pinchMagnitudeDelta.ToString(), resultsTextStyle);
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					gestureOccurred = false;
					pinchGesture.RestoreObject();
				}
			}
//			
//			GUI.Label(maxTimeLabelRect, "Max Time Between:");
//			string maxValueStr = GUI.TextField(maxTimeBoxRect, swipeGesture.maxTimeBetweensSwipes.ToString(), 5);
//			swipeGesture.maxTimeBetweensSwipes = ConvertToPositiveNonZeroFloat(maxValueStr, swipeGesture.maxTimeBetweensSwipes);
		}
	}
	
	//private Vector3 defaultTransform = new Vector3(3, 3, 1);
	
	bool pinching = false;
	bool Pinching {
		get { return pinching; }
		set {
			if (pinching != value) {
				pinching = value;
			}
		}
	}
	
	private string currentCall;
	private int moveIndex = 0;

	void GestureStartPinch (PinchGesture gesture)
	{
		gestureOccurred = true;
		currentCall = "Start";
		Pinching = true;
		moveIndex = 0;
	}
	
	void GestureEndPinch (PinchGesture gesture)
	{
		if (Pinching) {
			currentCall = "End";
			Pinching = false;
		}
	}
	
	void GestureMovePinch (PinchGesture gesture)
	{
		if (Pinching) {
			moveIndex++;
			currentCall = "Move " + moveIndex;
		}
	}
	
}
