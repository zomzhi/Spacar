using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SwipeSample : Sample
{
	public GUIStyle checkboxStyle;
	
	public Transform[] sliders;
	
	private SwipeGesture swipeGesture;
	private ComboControl fingerStartLocationCombo;
	private ComboControl fingerMoveLocationCombo;
	private ComboControl fingerEndLocationCombo;
	private ComboControl restrictDirectionCombo;
	private ComboControl restrictFingersCombo;
	
	private Rect settingsLabelRect;
	private Rect fingerStartLocationLabelRect;
	private Rect fingerStartLocationListRect;
	private Rect fingerMoveLocationLabelRect;
	private Rect fingerMoveLocationListRect;
	private Rect fingerEndLocationLabelRect;
	private Rect fingerEndLocationListRect;
	private Rect restrictDirectionLabelRect;
	private Rect restrictDirectionBoxRect;
	private Rect restrictFingerCountLabelRect;	
	private Rect restrictFingerCountBoxRect;	
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect fingerLabelRect;
	private Rect fingerBoxRect;
	
	private Rect swipeDirectionLabelRect;
	private Rect swipeDirectionBoxRect;
//	private Rect swipeDistanceLabelRect;
//	private Rect swipeDistanceBoxRect;
//	private Rect swipeVelocityLabelRect;
//	private Rect swipeVelocityBoxRect;
	private Rect swipeFingerCountLabelRect;
	private Rect swipeFingerCountBoxRect;
//	private Rect startLabelRect;
//	private Rect startBoxRect;
//	private Rect endLabelRect;
//	private Rect endBoxRect;	
	private Rect clearResultButtonRect;
	
	private bool gestureOccurred = false;
	private bool gestureMoveOccurred = false;
	
	//particles
	public Transform swipeParticles;
	public Camera cam;
	private Vector3 startLocation;
	private Vector3 particlePosition;
	private string eventName;
	private static Color originalColor = Color.white;
	
	protected override void SetupData ()
	{
		swipeGesture = this.gameObject.GetComponentInChildren<SwipeGesture>();
		
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		
		restrictDirectionLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 120f, boxHeight);
		float boxX = propLabelX + 75f;
		restrictDirectionBoxRect = new Rect(boxX, restrictDirectionLabelRect.y, 140f, boxHeight);
		restrictFingerCountLabelRect = new Rect(propLabelX, restrictDirectionLabelRect.yMax + betweenYGap, 120f, boxHeight);	
		restrictFingerCountBoxRect = new Rect(boxX, restrictFingerCountLabelRect.y, 145f, boxHeight);	
		
		fingerStartLocationLabelRect = new Rect(propLabelX, restrictFingerCountBoxRect.yMax + betweenYGap, 100f, boxHeight);
		fingerStartLocationListRect = new Rect(boxX, fingerStartLocationLabelRect.y, 120f, boxHeight);
		fingerMoveLocationLabelRect = new Rect(propLabelX, fingerStartLocationListRect.yMax + betweenYGap, 100f, boxHeight);
		fingerMoveLocationListRect = new Rect(boxX, fingerMoveLocationLabelRect.y, 120f, boxHeight);
		fingerEndLocationLabelRect = new Rect(propLabelX, fingerMoveLocationListRect.yMax + betweenYGap, 100f, boxHeight);
		fingerEndLocationListRect = new Rect(boxX, fingerEndLocationLabelRect.y, 120f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, fingerEndLocationListRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = propLabelX + 100f;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX, eventLabelRect.y, 150f, boxResultHeight);
		swipeDirectionLabelRect = new Rect(propLabelX, eventLabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		swipeDirectionBoxRect = new Rect(boxX, swipeDirectionLabelRect.y, 100f, boxResultHeight);
		swipeFingerCountLabelRect = new Rect(propLabelX, swipeDirectionLabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		swipeFingerCountBoxRect = new Rect(boxX, swipeFingerCountLabelRect.y, 50f, boxResultHeight);
//		swipeDistanceLabelRect = new Rect(propLabelX, swipeFingerCountLabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
//		swipeDistanceBoxRect = new Rect(boxX, swipeDistanceLabelRect.y, 50f, boxResultHeight);
//		swipeVelocityLabelRect = new Rect(propLabelX, swipeDistanceLabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
//		swipeVelocityBoxRect = new Rect(boxX, swipeVelocityLabelRect.y, 50f, boxResultHeight);
//		startLabelRect = new Rect(propLabelX, swipeVelocityLabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
//		startBoxRect = new Rect(boxX, startLabelRect.y, 150f, boxResultHeight);
//		endLabelRect = new Rect(propLabelX, startLabelRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
//		endBoxRect = new Rect(boxX, endLabelRect.y, 150f, boxResultHeight);
		fingerLabelRect = new Rect(propLabelX, swipeFingerCountBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		fingerBoxRect = new Rect(boxX, fingerLabelRect.y, 50f, boxResultHeight);
		
		clearResultButtonRect = new Rect(propLabelX, fingerLabelRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
		
	}
	
	private bool firstOnGUI = true;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			Background.Factory().SetBackground(sliders[1], 0.1f);
			homePosition = new Vector3(sliders[0].position.x,  sliders[0].position.y,  sliders[0].position.z);
			if (originalColor == Color.white) {
				originalColor =  duplicateColor(Background.Factory().theColor);
				//Debug.Log("SwipeSample:OnGUI   " + originalColor);
			}
			
			fingerStartLocationCombo = new ComboControl();
			fingerStartLocationCombo.SetFingerLocationList((int) swipeGesture.startsOnObject, false);
			fingerMoveLocationCombo = new ComboControl();
			fingerMoveLocationCombo.SetFingerLocationList((int) swipeGesture.movesOnObject, false);
			fingerEndLocationCombo = new ComboControl();
			fingerEndLocationCombo.SetFingerLocationList((int) swipeGesture.endsOnObject, false);
			restrictDirectionCombo = new ComboControl();
			restrictDirectionCombo.SetSwipeDirectionList((int) swipeGesture.restrictDirection);
			restrictFingersCombo = new ComboControl();
			restrictFingersCombo.SetFingerCountList((int) swipeGesture.restrictFingerCount);
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		
		if (!SampleGUI.sceneSelectionOpen) {
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			
			swipeGesture.restrictDirection = (SwipeGesture.SwipeDirection) restrictDirectionCombo.GUIShowList("Direction:", restrictDirectionLabelRect, restrictDirectionBoxRect);
			swipeGesture.restrictFingerCount = (BaseGesture.FingerCountRestriction) restrictFingersCombo.GUIShowList("Fingers:", restrictFingerCountLabelRect, restrictFingerCountBoxRect);
												
			swipeGesture.startsOnObject = (BaseGesture.FingerLocation) fingerStartLocationCombo.GUIShowList("Start:", fingerStartLocationLabelRect, fingerStartLocationListRect);
			swipeGesture.movesOnObject = (BaseGesture.FingerLocation) fingerMoveLocationCombo.GUIShowList("Move:", fingerMoveLocationLabelRect, fingerMoveLocationListRect);
			swipeGesture.endsOnObject = (BaseGesture.FingerLocation) fingerEndLocationCombo.GUIShowList("End:", fingerEndLocationLabelRect, fingerEndLocationListRect);
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);			
			GUI.Label(swipeDirectionLabelRect, "Direction:", resultsTextStyle);
//			GUI.Label(swipeDistanceLabelRect, "Distance:", resultsTextStyle);
//			GUI.Label(swipeVelocityLabelRect, "Velocity:", resultsTextStyle);
			GUI.Label(swipeFingerCountLabelRect, "Finger count:", resultsTextStyle);			
//			GUI.Label(startLabelRect, "Start point:", resultsTextStyle);
//			GUI.Label(endLabelRect, "End point:", resultsTextStyle);
			GUI.Label(fingerLabelRect, "Finger used:", resultsTextStyle);
			
			if (gestureOccurred || gestureMoveOccurred) {
				GUI.Label(eventBoxRect, eventName, resultsTextStyle);			
				GUI.Label(swipeDirectionBoxRect, swipeGesture.swipeDirection.ToString(), resultsTextStyle);
//				GUI.Label(swipeDistanceBoxRect, swipeGesture.swipeDistance.ToString(), resultsTextStyle);
//				GUI.Label(swipeVelocityBoxRect, swipeGesture.swipeVelocity.ToString(), resultsTextStyle);
				GUI.Label(swipeFingerCountBoxRect, swipeGesture.swipeFingerCount.ToString(), resultsTextStyle);
//				GUI.Label(startBoxRect, swipeGesture.fingerUsed.startPosition.ToString(), resultsTextStyle);			
//				GUI.Label(endBoxRect, swipeGesture.fingerUsed.endPosition.ToString(), resultsTextStyle);			
				GUI.Label(fingerBoxRect, swipeGesture.finger.Index().ToString(), resultsTextStyle);
			}
			if (gestureOccurred || gestureMoveOccurred || sliders[0].GetComponent<Renderer>().material.color != originalColor) {
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					//Debug.Log("SwipeSample:OnGUI  ClearButton");
					ClearButton();
				}
			}
//			
//			GUI.Label(maxTimeLabelRect, "Max Time Between:");
//			string maxValueStr = GUI.TextField(maxTimeBoxRect, swipeGesture.maxTimeBetweensSwipes.ToString(), 5);
//			swipeGesture.maxTimeBetweensSwipes = ConvertToPositiveNonZeroFloat(maxValueStr, swipeGesture.maxTimeBetweensSwipes);
		}
	}
	
	private Color duplicateColor(Color val)
	{
		return new Color(val.r, val.g, val.b, val.a);
	}
	
	private bool currentVisible = true;
	
	public void ClearButton()
	{
		//Debug.Log("SwipeSample:Clear" + originalColor);
		gestureOccurred = false;
		sliders[0].GetComponent<Renderer>().material.color = originalColor;
		sliders[1].GetComponent<Renderer>().material.color = originalColor;
		moveSliderColor = originalColor;
		if (moveSlider) {
			stopSlider = true;
		}
	}

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
	void DisplaySuccessSwipe(GameObject targetObj, SwipeGesture.SwipeDirection direction)
	{
		if (targetObj != null) {
			Vector3 heading;
			
			switch (direction) {
			case SwipeGesture.SwipeDirection.Up:
				heading = Vector3.up;
				break;
			case SwipeGesture.SwipeDirection.Plus45:
				heading = new Vector3(1, 1, 0);
				break;
			case SwipeGesture.SwipeDirection.Right:
				heading = Vector3.right;
				break;
			case SwipeGesture.SwipeDirection.Plus135:
				heading = new Vector3(1, -1, 0);
				break;
			case SwipeGesture.SwipeDirection.Down:
				heading = Vector3.down;
				break;
			case SwipeGesture.SwipeDirection.Minus135:
				heading = new Vector3(-1, -1, 0);
				break;
			case SwipeGesture.SwipeDirection.Left:
				heading = Vector3.left;
				break;
			case SwipeGesture.SwipeDirection.Minus45:
				heading = new Vector3(-1, 1, 0);
				break;
			default:
				return;
			}
			
			//finger position
			if(!currentVisible) {
				startLocation.z = 25;
				particlePosition = cam.ScreenToWorldPoint(startLocation);
			}
			
	        Transform clone = Instantiate(swipeParticles, particlePosition, Quaternion.identity) as Transform;
			clone.name = "Particles";
			
			//local position
			if(currentVisible)
				clone.localPosition = transform.position;
			
			ParticleEmitter emitter = clone.GetComponent<ParticleEmitter>();
			emitter.localVelocity = heading * 3;
			emitter.rndVelocity = heading * 2;
			emitter.Emit();
		}
	}
	
	private const float moveAmount = 0.5f;
	void Update()
	{
		if (moveSlider || stopSlider) {
			if (stopSlider && !moveSlider) {
				stopSlider = false;
				return;
			}
			Vector3 newPos = new Vector3( sliders[movingSurface].position.x, sliders[movingSurface].position.y, sliders[movingSurface].position.z);
			bool stopNow = false;
			switch (moveSliderDirection) {
				case SwipeGesture.SwipeDirection.Up:
					newPos.y += moveAmount;
					if (newPos.y > moveSliderTarget.y) {
						stopNow = true;
					}
					break;
				case SwipeGesture.SwipeDirection.Right:
					newPos.x += moveAmount; 	
					if (newPos.x > moveSliderTarget.x) {
						stopNow = true;
					}
					break;
				case SwipeGesture.SwipeDirection.Down:
					newPos.y -= moveAmount; 	
					if (newPos.y < moveSliderTarget.y) {
						stopNow = true;
					}
					break;
				case SwipeGesture.SwipeDirection.Left:
					newPos.x -= moveAmount; 	
					if (newPos.x < moveSliderTarget.x) {
						stopNow = true;
					}
					break;
				default:
					break;
			}
			//Debug.Log("SwipeSample:Update   " + moveSliderDirection + " " + newPos + "   stopSlider=" + stopSlider);
			sliders[movingSurface].position = newPos;
			if (stopNow || stopSlider) {
				moveSlider = false;
				stopSlider = false;
				sliders[movingSurface].position = moveSliderTarget;
				//Debug.Log("SwipeSample:Update  stop " + moveSliderDirection + " " + newPos + " color " + moveSliderColor);
				sliders[movingSurface].GetComponent<Renderer>().material.color = moveSliderColor;
				sliders[backSurface].GetComponent<Renderer>().material.color = moveSliderColor;
				sliders[movingSurface].position = homePosition;
			}
		}
	}

	void GestureSwipe(SwipeGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		
		gestureOccurred = true;
		gestureMoveOccurred = false;
		eventName = "Swipe";
		if (gesture.targetCollider != null) {
			DisplaySuccessSwipe(gesture.targetCollider.gameObject, gesture.swipeDirection);
		}
		EnsureMovement(gesture.startPosition, gesture.endPosition);
		
	}
	
	private bool moveSlider = false;
	private bool stopSlider = false;
	private Vector3 moveSliderTarget;
	private SwipeGesture.SwipeDirection moveSliderDirection = SwipeGesture.SwipeDirection.None;
	private Color moveSliderColor;
	private Vector3 homePosition;
	private Vector3 targetPosition;
	private Vector3 currentPosition;
	private int movingSurface = 0;
	private int backSurface = 1;
	private SwipeGesture.SwipeDirection moveDirection = SwipeGesture.SwipeDirection.None;
	private Vector2 lastPosition;
	private bool doingStartupCounts = false;
	private int[] startupCounts = new int[Enum.GetValues(typeof(SwipeGesture.SwipeDirection)).Length];
	private float menuBarDifference = 7f;
	private float closerOffset = 5f;
	void GestureSwipeMove(SwipeGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		
		gestureMoveOccurred = true;
		eventName = "Swipe Move";
		
		if (moveDirection == SwipeGesture.SwipeDirection.None) {
			if (!doingStartupCounts) {
				for (int i = 0; i < startupCounts.Length; i++) {
					startupCounts[i] = 0;
				}
				doingStartupCounts = true;
				//Debug.Log("SwipeSample:GestureSwipeMove  initalize startupCounts ");
			}
			if (doingStartupCounts) {
				//Debug.Log("SwipeSample:GestureSwipeMove  doing  startupCounts " + gesture.swipeDirection + " has " + startupCounts[(int) gesture.swipeDirection]);
				startupCounts[(int) gesture.swipeDirection]++;
				if (startupCounts[(int) gesture.swipeDirection] < 6) {
					//Debug.Log("SwipeSample:GestureSwipeMove startupCounts " + gesture.swipeDirection + " has " + startupCounts[(int) gesture.swipeDirection]);
					return;
				}
				doingStartupCounts = false;
			}
			
			SetMoveColour(gesture.swipeDirection, gesture.fingerCount, gesture.finger.Index());
			lastPosition = gesture.finger.position;
			currentPosition = new Vector3(homePosition.x, homePosition.y, homePosition.z);
			
			switch (gesture.swipeDirection) {
			case SwipeGesture.SwipeDirection.Up:
			case SwipeGesture.SwipeDirection.Plus45:
				moveDirection = SwipeGesture.SwipeDirection.Up;
				targetPosition = new Vector3(homePosition.x, homePosition.y + Background.Factory().height, homePosition.z);
				break;
			case SwipeGesture.SwipeDirection.Right:
			case SwipeGesture.SwipeDirection.Plus135:
				targetPosition = new Vector3(homePosition.x  + Background.Factory().width, homePosition.y, homePosition.z);
				moveDirection = SwipeGesture.SwipeDirection.Right;
				currentPosition.x += menuBarDifference;
				break;
			case SwipeGesture.SwipeDirection.Down:
			case SwipeGesture.SwipeDirection.Minus135:
				moveDirection = SwipeGesture.SwipeDirection.Down;
				targetPosition = new Vector3(homePosition.x, homePosition.y - Background.Factory().height, homePosition.z);
				break;
			case SwipeGesture.SwipeDirection.Left:
			case SwipeGesture.SwipeDirection.Minus45:
				moveDirection = SwipeGesture.SwipeDirection.Left;
				targetPosition = new Vector3(homePosition.x  - Background.Factory().width, homePosition.y, homePosition.z);
				break;
			default:
				moveDirection = SwipeGesture.SwipeDirection.Down;
				break;
			}
			//Debug.Log("SwipeSample:GestureSwipeMove start move  got " + gesture.swipeDirection + " to " + moveDirection);
		}
		
		currentPosition = CalcMovement(currentPosition, gesture.finger.position, lastPosition);
		sliders[movingSurface].position = currentPosition;
		lastPosition = gesture.finger.position;		
	}
	
	private Vector3 CalcMovement(Vector3 curPos, Vector2 fromPos, Vector2 toPos)
	{
		Vector3 newPos = new Vector3(curPos.x, curPos.y, curPos.z);
		switch (moveDirection) {
		case SwipeGesture.SwipeDirection.Up:
		case SwipeGesture.SwipeDirection.Down:
			newPos.y = CalcMove(curPos.y, fromPos.y, toPos.y);
			break;
		case SwipeGesture.SwipeDirection.Right:
		case SwipeGesture.SwipeDirection.Left:
			newPos.x = CalcMove(curPos.x, fromPos.x, toPos.x);
			break;
		}
		return newPos;
	}
	
	private float CalcMove(float curVal, float fromVal, float toVal)
	{
		return curVal + ((fromVal - toVal) * 0.07f);
	}
	
	private void EnsureMovement(Vector2 fromPos, Vector2 toPos)
	{
		Vector3 newPos = CalcMovement(homePosition, toPos, fromPos);
		//Debug.Log("SwipeSample:EnsureMovement swipe " + fromPos + " to " + toPos + " newPos=" + newPos + " currentPosition=" + currentPosition);
		if ( Mathf.Abs(newPos.magnitude) > Mathf.Abs(currentPosition.magnitude) ) {
			//Debug.Log("SwipeSample:EnsureMovement move it swipe newPos-" + newPos.magnitude + " currentPos-" + currentPosition.magnitude);
			lastPosition = currentPosition;		
			currentPosition = newPos;
			//sliders[movingSurface].position = currentPosition;
			//Debug.Log("SwipeSample:EnsureMovement move it swipe " + fromPos + " to " + toPos + " newPos=" + newPos + " currentPosition=" + currentPosition);
		}
	}
	
	void GestureStartTouch(TouchGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen) {
			return;
		}
		if (gesture.fingerCount > 1 && gestureOccurred) {
			return;
		}
		    	
		gestureOccurred = false;
		gestureMoveOccurred = false;
		startLocation = gesture.finger.position;
		stopSlider = true;
	}
	
	void GestureSwipeEnd(SwipeGesture gesture)
	{
		//Debug.Log("SwipeSample:GestureSwipeEnd");
		if (SampleGUI.sceneSelectionOpen || moveSlider) {
			return;
		}
		
		bool goHome = false;
		moveSliderDirection = moveDirection;
		Vector3 middle = new Vector3(homePosition.x + ((Background.Factory().width + menuBarDifference) / 2f), homePosition.y - (Background.Factory().height / 2f), homePosition.z);
		//Debug.Log("SwipeSample:GestureSwipeEnd end moving " + sliders[movingSurface].position + " middle at " + middle);

		switch (moveSliderDirection) {
		case SwipeGesture.SwipeDirection.Up:
			if ((currentPosition.y - Background.Factory().height) < (middle.y - closerOffset)) {
				goHome = true;
				moveSliderDirection = SwipeGesture.SwipeDirection.Down;
			}
			break;
		case SwipeGesture.SwipeDirection.Right:
			//Debug.Log("SwipeSample:GestureSwipeEnd end moving Right x =" + sliders[movingSurface].position.x + " middle x at " +  middle.x);
			if (currentPosition.x  < (middle.x - closerOffset)) {
				goHome = true;
				moveSliderDirection = SwipeGesture.SwipeDirection.Left;
			}
			break;
		case SwipeGesture.SwipeDirection.Down:
			if (currentPosition.y > (middle.y + closerOffset)) {
				goHome = true;
				moveSliderDirection = SwipeGesture.SwipeDirection.Up;
			}
			break;
		case SwipeGesture.SwipeDirection.Left:
			//Debug.Log("SwipeSample:GestureSwipeEnd end moving Left x + w =" + (sliders[movingSurface].position.x + Background.Factory().width) + " middle x at " +  middle.x);
			if ((currentPosition.x + Background.Factory().width) > (middle.x + closerOffset)) {
				goHome = true;
				moveSliderDirection = SwipeGesture.SwipeDirection.Right;
			}
			break;
		}
		if (goHome) {
			moveSliderTarget = homePosition;
			moveSliderColor = duplicateColor(sliders[movingSurface].GetComponent<Renderer>().material.color);
		}
		else {
			moveSliderTarget = targetPosition;
			moveSliderColor = duplicateColor(sliders[backSurface].GetComponent<Renderer>().material.color);
			Background.Factory().SetParticles(moveSliderColor);
		}
		//Debug.Log("SwipeSample:GestureSwipeEnd goHome=" + goHome + " move in " + moveSliderDirection + "  moveSliderTarget=" + moveSliderTarget + " at " + sliders[movingSurface].position);
		moveSlider = true;
		stopSlider = false;
		
		moveDirection = SwipeGesture.SwipeDirection.None;
	}
	
	private void SetMoveColour(SwipeGesture.SwipeDirection direction, int count, int fingerId)
	{
		float inc = 0f;
		switch (direction) {
		case SwipeGesture.SwipeDirection.Up:
			inc = -0.033f;
			break;
		case SwipeGesture.SwipeDirection.Right:
			inc = 0.273f;
			break;
		case SwipeGesture.SwipeDirection.Down:
			inc = 0.027f;
			break;
		case SwipeGesture.SwipeDirection.Left:
			inc = -0.259f;
			break;
		default:
			inc = 0.153f;
			break;
		}
		int id = count;
		if (FingerControl.Factory().IsFingers()) {
			id--;  // Zero based
		}
		else {
			id = fingerId;
		}
		
		Color newColor = duplicateColor(sliders[backSurface].GetComponent<Renderer>().material.color);
		
		//Debug.Log("SwipeSample:SetMoveColour  inc " + inc + ", id " +  id);
		switch (id) {
		case 0:
			newColor.r += inc;
			break;
		case 1:
			newColor.b += inc;
			break;
		case 2:
			newColor.g += inc;
			break;
		case 3:
			newColor.g += inc;
			newColor.b += inc;
			newColor.r += inc;
			break;
		case 4:
			newColor.g += inc * 1.5f;
			newColor.b += inc * 1.5f;
			newColor.r += inc * 1.5f;
			break;
		default:
			newColor.g += inc;
			newColor.b += inc;
			newColor.r += inc;
			break;
		}
		if (newColor.g > 1f) {
			newColor.g = 1f;
		}
		if (newColor.b > 1f) {
			newColor.b = 1f;
		}
		if (newColor.r > 1f) {
			newColor.r = 1f;
		}
		if (newColor.g < 0f) {
			newColor.g = 0f;
		}
		if (newColor.b < 0f) {
			newColor.b = 0f;
		}
		if (newColor.r < 0f) {
			newColor.r = 0f;
		}
		//Debug.Log("SwipeSample:SetMoveColour  " + sliders[backSurface].renderer.material.color + " to " +  newColor);
		
		sliders[backSurface].GetComponent<Renderer>().material.color = newColor;
	}

}

