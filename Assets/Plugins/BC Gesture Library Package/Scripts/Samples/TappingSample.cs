using UnityEngine;
using System.Collections;

public class TappingSample : Sample
{
	private TapGesture tapGesture;
	private ComboControl fingerLocationCombo;
	
	private Rect settingsLabelRect;
	private Rect fingerLocationLabelRect;
	private Rect fingerLocationListRect;
	private Rect tapsLabelRect;
	private Rect tapsBoxRect;
	private Rect maxTimeLabelRect;
	private Rect maxTimeBoxRect;
	private Rect maxDistLabelRect;
	private Rect maxDistBoxRect;
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect fingerLabelRect;
	private Rect fingerBoxRect;
	private Rect rateLabelRect;
	private Rect rateBoxRect;
	private Rect rateSliderRect;
	
	private BaseGesture.FingerLocation fingerLocation = BaseGesture.FingerLocation.Over;
	private BaseGesture.FingerLocation fingerLocationOld = BaseGesture.FingerLocation.Over;
	
	public ParticleEmitter emitter;
	
	private Rect clearResultButtonRect;
	
	private bool gestureOccurred = false;
	
	protected override void SetupData ()
	{
		tapGesture = this.gameObject.GetComponentInChildren<TapGesture>();
		fingerLocationOld = fingerLocation;
		
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		tapsLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 100f, boxHeight);
		tapsBoxRect = new Rect(tapsLabelRect.x + 120, tapsLabelRect.y, 30f, boxHeight);
		
		maxTimeLabelRect = new Rect(propLabelX, tapsLabelRect.yMax + betweenYGap, 120f, boxHeight);
		maxTimeBoxRect = new Rect(tapsBoxRect.x, maxTimeLabelRect.y, 50f, boxHeight);
		maxDistLabelRect = new Rect(propLabelX, maxTimeBoxRect.yMax + betweenYGap, 120f, boxHeight);
		maxDistBoxRect = new Rect(tapsBoxRect.x, maxDistLabelRect.y, 50f, boxHeight);
		
		fingerLocationLabelRect = new Rect(propLabelX, maxDistBoxRect.yMax + betweenYGap, 100f, boxHeight);
		fingerLocationListRect = new Rect(tapsBoxRect.x, fingerLocationLabelRect.y, 100f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, fingerLocationLabelRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(tapsBoxRect.x, eventLabelRect.y, 100f, boxResultHeight);
		fingerLabelRect = new Rect(propLabelX, eventLabelRect.yMax + betweenYGap, 100f, boxResultHeight);
		fingerBoxRect = new Rect(eventBoxRect.x, fingerLabelRect.y, 50f, boxResultHeight);
		rateLabelRect = new Rect(propLabelX, fingerBoxRect.yMax + betweenYGap, 130f, boxResultHeight);
		rateBoxRect = new Rect(eventBoxRect.x, rateLabelRect.y, 120f, boxResultHeight);
		rateSliderRect = new Rect(propLabelX,  rateBoxRect.yMax + 1, 200f, 16f);
		
		clearResultButtonRect = new Rect(propLabelX, rateSliderRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
	}
	
	private bool firstOnGUI = true;	
	private float maxTPM = 800;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			fingerLocationCombo = new ComboControl();
			fingerLocationCombo.SetFingerLocationList((int) fingerLocation, false);		
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		
		if (!SampleGUI.sceneSelectionOpen) {
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			
			GUI.Label(tapsLabelRect, "Taps:", settingsTextStyle);			
			tapGesture.taps = GUITextPositiveNaturalIntIncludingZero(tapsBoxRect, tapGesture.taps, 3);
			
			GUI.Label(maxDistLabelRect, "Max Distance:", settingsTextStyle);
			tapGesture.maxTapDistance = GUITextPositiveNonZeroFloat(maxDistBoxRect, tapGesture.maxTapDistance, 6);
			
			
			fingerLocation = (BaseGesture.FingerLocation) fingerLocationCombo.GUIShowList("Location:", fingerLocationLabelRect, fingerLocationListRect);
			//Debug.Log("TappingSample:OnGUI fingerLocation " + tapGesture.fingerLocation);
			if (fingerLocationOld != fingerLocation) {
				tapGesture.startsOnObject = fingerLocation;
				tapGesture.movesOnObject = fingerLocation;
				tapGesture.endsOnObject = fingerLocation;
				fingerLocationOld = fingerLocation;
			}
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);			
			GUI.Label(fingerLabelRect, "Finger used:", resultsTextStyle);
			GUI.Label(rateLabelRect, "Taps Per Minute:", resultsTextStyle);
			
			if (gestureOccurred) {
				GUI.Label(eventBoxRect, "Received Taps", resultsTextStyle);			
				GUI.Label(fingerBoxRect, tapGesture.finger.Index().ToString(), resultsTextStyle);
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					gestureOccurred = false;
				}
				
				if (tapGesture.tapsPerMinute < 1f) {
					GUI.Label(rateBoxRect, "0", resultsTextStyle);
				}
				else {
					GUI.Label(rateBoxRect, tapGesture.tapsPerMinute.ToString(), resultsTextStyle);
				}

				
//					if(smooth)
//		            	smoothTPM = GUILayout.HorizontalSlider(smoothTPM, 0f, maxTPM, GUILayout.Width(maxTPM));
//					else
				GUILayout.BeginArea(rateSliderRect);
				GUILayout.HorizontalSlider(tapGesture.tapsPerMinute / 5f, 0f, maxTPM, GUILayout.Width(maxTPM));
				GUILayout.EndArea();
				
			}
			
			GUI.Label(maxTimeLabelRect, "Max Time Between:");
			tapGesture.maxTimeBetweensTaps = GUITextPositiveNonZeroFloat(maxTimeBoxRect, tapGesture.maxTimeBetweensTaps, 5);
		}
	}

	void GestureTap(TapGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		gestureOccurred = true;
		emitter.Emit();
		EmitSuccess (this.gameObject);
	}	
}