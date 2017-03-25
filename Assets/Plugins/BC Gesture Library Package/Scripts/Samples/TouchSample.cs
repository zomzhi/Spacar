using UnityEngine;
using System.Collections;

public class TouchSample : Sample
{
	public Texture2D[] followFingerTexture;
	
	private ComboControl fingerStartLocationCombo;
	private ComboControl fingerMoveLocationCombo;
	private ComboControl fingerEndLocationCombo;
	private ComboControl restrictFingersCombo;
	
	private TouchGesture touchGesture;
	
	private Rect settingsLabelRect;
	private Rect fingerStartLocationLabelRect;
	private Rect fingerStartLocationListRect;
	private Rect fingerMoveLocationLabelRect;
	private Rect fingerMoveLocationListRect;
	private Rect fingerEndLocationLabelRect;
	private Rect fingerEndLocationListRect;
	private Rect useAveragePointLabelRect;
	private Rect useAveragePointBoxRect;
	private Rect restrictFingerCountLabelRect;	
	private Rect restrictFingerCountBoxRect;	
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect fingerLabelRect;
	private Rect fingerBoxRect;
	private Rect fingerPositionLabelRect;
	private Rect fingerPositionBoxRect;
	private Rect fingerCountLabelRect;
	private Rect fingerCountBoxRect;
	private Rect isDownLabelRect;
	private Rect isDownBoxRect;
	private Rect clearResultButtonRect;
	
	private bool gestureOccurred = false;
	private string touchEventStr;
	
	protected override void SetupData ()
	{
		touchGesture = this.gameObject.GetComponentInChildren<TouchGesture>();
		
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
				
		fingerStartLocationLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 100f, boxHeight);
		float boxX = propLabelX + 80f;
		fingerStartLocationListRect = new Rect(boxX, fingerStartLocationLabelRect.y, 140f, boxHeight);
		fingerMoveLocationLabelRect = new Rect(propLabelX, fingerStartLocationListRect.yMax + betweenYGap, 100f, boxHeight);
		fingerMoveLocationListRect = new Rect(boxX, fingerMoveLocationLabelRect.y, 140f, boxHeight);
		fingerEndLocationLabelRect = new Rect(propLabelX, fingerMoveLocationListRect.yMax + betweenYGap, 100f, boxHeight);
		fingerEndLocationListRect = new Rect(boxX, fingerEndLocationLabelRect.y, 140f, boxHeight);
		restrictFingerCountLabelRect = new Rect(propLabelX, fingerEndLocationListRect.yMax + betweenYGap, 120f, boxHeight);	
		restrictFingerCountBoxRect = new Rect(boxX, restrictFingerCountLabelRect.y, 145f, boxHeight);	
		
		useAveragePointLabelRect = new Rect(propLabelX, restrictFingerCountBoxRect.yMax + betweenYGap, 120f, boxHeight);
		useAveragePointBoxRect = new Rect(boxX + 30, useAveragePointLabelRect.y, checkboxSize, checkboxSize);
		useAveragePointLabelRect.y += labelCenterOffset;
				
		resultLabelRect = new Rect(sectionX, useAveragePointLabelRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = propLabelX + 100f;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX, eventLabelRect.y, 150f, boxResultHeight);

		fingerCountLabelRect = new Rect(propLabelX, eventBoxRect.yMax + betweenYGap, 100f, boxResultHeight);
		fingerCountBoxRect = new Rect(boxX, fingerCountLabelRect.y, 250f, boxResultHeight);
		fingerLabelRect = new Rect(propLabelX, fingerCountBoxRect.yMax + betweenYGap, 100f, boxResultHeight);
		fingerBoxRect = new Rect(boxX, fingerLabelRect.y, 50f, boxResultHeight);
		fingerPositionLabelRect = new Rect(propLabelX, fingerBoxRect.yMax + betweenYGap, 100f, boxResultHeight);
		fingerPositionBoxRect = new Rect(boxX, fingerPositionLabelRect.y, 250f, boxResultHeight);
		isDownLabelRect = new Rect(propLabelX, fingerPositionBoxRect.yMax + betweenYGap, 100f, boxResultHeight);
		isDownBoxRect = new Rect(boxX, isDownLabelRect.y, 120f, boxResultHeight);
			
		clearResultButtonRect = new Rect(propLabelX, isDownBoxRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
		
	}
	
	private bool firstOnGUI = true;
	private bool ignoreEndForButtonPress = false;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			firstOnGUI = false;
			fingerStartLocationCombo = new ComboControl();
			fingerStartLocationCombo.SetFingerLocationList((int) touchGesture.startsOnObject, false);
			fingerMoveLocationCombo = new ComboControl();
			fingerMoveLocationCombo.SetFingerLocationList((int) touchGesture.movesOnObject, false);
			fingerEndLocationCombo = new ComboControl();
			fingerEndLocationCombo.SetFingerLocationList((int) touchGesture.endsOnObject, false);
			restrictFingersCombo = new ComboControl();
			restrictFingersCombo.SetFingerCountList((int) touchGesture.restrictFingerCount);
		}
		if (SampleGUI.Factory() != null) {
			SampleGUI.Factory().PerformGUI();
		}
		
		if (!SampleGUI.sceneSelectionOpen) {
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			
			touchGesture.startsOnObject = (BaseGesture.FingerLocation) fingerStartLocationCombo.GUIShowList("Start:", fingerStartLocationLabelRect, fingerStartLocationListRect);
			touchGesture.movesOnObject = (BaseGesture.FingerLocation) fingerMoveLocationCombo.GUIShowList("Move:", fingerMoveLocationLabelRect, fingerMoveLocationListRect);
			touchGesture.endsOnObject = (BaseGesture.FingerLocation) fingerEndLocationCombo.GUIShowList("End:", fingerEndLocationLabelRect, fingerEndLocationListRect);
			touchGesture.restrictFingerCount = (BaseGesture.FingerCountRestriction) restrictFingersCombo.GUIShowList("Fingers:", restrictFingerCountLabelRect, restrictFingerCountBoxRect);

			GUI.Label(useAveragePointLabelRect, "Averged point:", settingsTextStyle);
			touchGesture.averagePoint = GuiCheckBox(useAveragePointBoxRect, touchGesture.averagePoint);
		
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);			
			GUI.Label(fingerLabelRect, "Finger used:", resultsTextStyle);
			GUI.Label(fingerPositionLabelRect, "Position:", resultsTextStyle);
			GUI.Label(fingerCountLabelRect, "Finger count:", resultsTextStyle);
			GUI.Label(isDownLabelRect, "Is down:", resultsTextStyle);
			
			if (gestureOccurred) {
				GUI.Label(eventBoxRect, touchEventStr, resultsTextStyle);			
				GUI.Label(fingerBoxRect, touchGesture.finger.Index().ToString(), resultsTextStyle);			
				GUI.Label(fingerPositionBoxRect, position, resultsTextStyle);			
				GUI.Label(fingerCountBoxRect, touchGesture.fingerCount.ToString(), resultsTextStyle);			
				GUI.Label(isDownBoxRect, touchGesture.isDown.ToString(), resultsTextStyle);			
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					gestureOccurred = false;
					ignoreEndForButtonPress = true;
				}
				if (touchGesture.fingerCount > 0) {
					if (touchGesture.averagePoint) {
						DrawFingerTexture(touchGesture.touchPosition, 0);
					}
					else {
						for (int i = 0; i < touchGesture.fingerCount; i++) {
							DrawFingerTexture(fingerPositions[i], i);
						}
					}
				}
				
			}
			
			for (int i = 0; i < touchGesture.isActives.Length; i++) {
				FingerParticle.Factory().ShowParticle( touchGesture.isActives[i], i);
			}
		}
	}
					     
	
	private void DrawFingerTexture(Vector2 touchPosition, int followIndex)
	{
		Vector2 pos = Finger.ScreenVectorToGui(touchPosition);
		bool isActive = touchGesture.averagePoint ? touchGesture.isActive : touchGesture.isActives[followIndex];
		if (pos.x > SampleGUI.Factory().settingsBarRect.xMax && isActive) {
			GUI.DrawTexture (new Rect(pos.x - 24, pos.y - 24, 48, 48), followFingerTexture[followIndex]);
		}
	}
	
	private Vector2[] fingerPositions = new Vector2[5];
	private int moveCount = 0;
	private string position;
	void GestureStartTouch(TouchGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		for (int i = 0; i < fingerPositions.Length; i++) {
			fingerPositions[i] = Vector2.zero;
		}
		touchGesture = gesture;
		FingerParticle.Factory().ShowParticle(true, gesture.finger.Index());
		position = touchGesture.finger.startPosition.ToString();
		fingerPositions[touchGesture.finger.Index()] = touchGesture.finger.position;
		gestureOccurred = true;
		touchEventStr = "Start Touch";
		moveCount = 0;
	}
	void GestureMoveTouch(TouchGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		FingerParticle.Factory().ShowParticles(true);
		gestureOccurred = true;
		touchGesture = gesture;
		fingerPositions[touchGesture.finger.Index()] = touchGesture.finger.position;
		position =  touchGesture.touchPosition.ToString();
		moveCount++;
		touchEventStr = "Move " + moveCount;
		//Debug.Log("TouchSample:GestureMoveTouch " + position);
	}
	void GestureEndTouch(TouchGesture gesture)
	{
		if (ignoreEndForButtonPress) {
			ignoreEndForButtonPress = false;
			return;
		}
		if (FingerParticle.Factory() != null) {
			FingerParticle.Factory().ShowParticles(false);
		}
		gestureOccurred = true;
		touchGesture = gesture;
		position =  touchGesture.averagePoint.ToString();
		EmitSuccess (this.gameObject);
		touchEventStr = "End Touch";
		//Debug.Log("TouchSample:GestureEndTouch " + position);
	}
}

