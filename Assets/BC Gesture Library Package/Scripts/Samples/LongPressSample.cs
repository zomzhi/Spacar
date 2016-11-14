using UnityEngine;
using System.Collections;

public class LongPressSample : Sample
{
	private LongPressGesture longPressGesture;
	private ComboControl fingerLocationCombo;
	private ComboControl restrictFingersCombo;
	
	private Rect settingsLabelRect;
	private Rect fingerLocationLabelRect;
	private Rect fingerLocationListRect;
	private Rect longPressTimeLabelRect;
	private Rect longPressTimeBoxRect;
	private Rect restrictFingerCountLabelRect;	
	private Rect restrictFingerCountBoxRect;	
	private Rect longPressDistLabelRect;
	private Rect longPressDistBoxRect;
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect fingerLabelRect;
	private Rect fingerBoxRect;
	private Rect fingerCountLabelRect;
	private Rect fingerCountBoxRect;
	private Rect timeDifferenceLabelRect;
	private Rect timeDifferenceBoxRect;
	
	public ParticleEmitter emitter;
	
	private Rect clearResultButtonRect;
	
	private bool gestureOccurred = false;
	
	protected override void SetupData ()
	{
		longPressGesture = this.gameObject.GetComponentInChildren<LongPressGesture>();
				
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		float boxX = propLabelX + 120;
		longPressTimeLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 130f, boxHeight);
		longPressTimeBoxRect = new Rect(boxX, longPressTimeLabelRect.y, 50f, boxHeight);
		restrictFingerCountLabelRect = new Rect(propLabelX, longPressTimeBoxRect.yMax + betweenYGap, 100f, boxHeight);
		restrictFingerCountBoxRect = new Rect(boxX - 30, restrictFingerCountLabelRect.y, 135f, boxHeight);
		longPressDistLabelRect = new Rect(propLabelX, restrictFingerCountBoxRect.yMax + betweenYGap, 100f, boxHeight);
		longPressDistBoxRect = new Rect(boxX, longPressDistLabelRect.y, 50f, boxHeight);				
		fingerLocationLabelRect = new Rect(propLabelX, longPressDistBoxRect.yMax + betweenYGap, 130f, boxHeight);
		fingerLocationListRect = new Rect(boxX, fingerLocationLabelRect.y, 80f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, fingerLocationLabelRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = propLabelX + 120;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX, eventLabelRect.y, 150f, boxResultHeight);
		timeDifferenceLabelRect = new Rect(propLabelX, eventLabelRect.yMax + betweenYGap, 130f, boxResultHeight);
		timeDifferenceBoxRect = new Rect(boxX, timeDifferenceLabelRect.y, 120f, boxResultHeight);
		fingerLabelRect = new Rect(propLabelX, timeDifferenceLabelRect.yMax + betweenYGap, 130f, boxResultHeight);
		fingerBoxRect = new Rect(boxX, fingerLabelRect.y, 50f, boxResultHeight);
		fingerCountLabelRect = new Rect(propLabelX, fingerBoxRect.yMax + betweenYGap, 170f, boxResultHeight);
		fingerCountBoxRect = new Rect(boxX, fingerCountLabelRect.y, 50f, boxResultHeight);
		
		clearResultButtonRect = new Rect(propLabelX, fingerCountBoxRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
		
	}
	

	private bool firstOnGUI = true;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			fingerLocationCombo = new ComboControl();
			fingerLocationCombo.SetFingerLocationList((int) longPressGesture.pressLocation, false);
			restrictFingersCombo = new ComboControl();
			restrictFingersCombo.SetFingerCountList((int) longPressGesture.restrictFingerCount);
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		if (!SampleGUI.sceneSelectionOpen) {
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			
			GUI.Label(longPressTimeLabelRect, "Long press time:", settingsTextStyle);
			longPressGesture.longPressTime = GUITextPositiveNonZeroFloat(longPressTimeBoxRect, longPressGesture.longPressTime, 7);
			longPressGesture.restrictFingerCount = (BaseGesture.FingerCountRestriction) restrictFingersCombo.GUIShowList("Fingers:", restrictFingerCountLabelRect, restrictFingerCountBoxRect);
			GUI.Label(longPressDistLabelRect, "Max Distance:", settingsTextStyle);
			longPressGesture.maxPressDistance = GUITextPositiveNonZeroFloat(longPressDistBoxRect, longPressGesture.maxPressDistance, 6);

			
			
			longPressGesture.pressLocation = (BaseGesture.FingerLocation) fingerLocationCombo.GUIShowList("Location:", fingerLocationLabelRect, fingerLocationListRect);
			//Debug.Log("TappingSample:OnGUI fingerLocation " + longPressGesture.fingerLocation);
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);			
			GUI.Label(timeDifferenceLabelRect, "Time Difference:", resultsTextStyle);			
			GUI.Label(fingerLabelRect, "Finger used:", resultsTextStyle);
			GUI.Label(fingerCountLabelRect, "Finger count:", resultsTextStyle);
			
			if (gestureOccurred) {
				GUI.Label(eventBoxRect, "Received Press", resultsTextStyle);			
				GUI.Label(timeDifferenceBoxRect, longPressGesture.timeDifference.ToString(), resultsTextStyle);
				GUI.Label(fingerBoxRect, longPressGesture.finger.Index().ToString(), resultsTextStyle);
				GUI.Label(fingerCountBoxRect, longPressGesture.fingerCount.ToString(), resultsTextStyle);
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					gestureOccurred = false;
				}
			}
			
		}
	}

	void GestureLongPress(LongPressGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		gestureOccurred = true;
		emitter.Emit();
		EmitSuccess (this.gameObject);
	}	
}

