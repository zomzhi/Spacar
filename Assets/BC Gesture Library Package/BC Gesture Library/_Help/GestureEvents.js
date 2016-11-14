/*TECHNICAL ASSISTANCE 
For the most up to date technical assistance visit: http://blackcherrydm.com/bcgesturelibrary/help
*/ 

/*
	//  This class has the methods used for all the different Gesture callback events for you the copy
	// 
	// This code is commented out because it will not compile from this folder.  If you move this file
	// outside the plugins folder (and remove the outer comment) then it will compile fine.

	function Start()
	{
		// get a gesture object that is on this object.
		var lineGesture = gameObject.GetComponentInChildren(LineGesture);
		lineGesture.AddLineFactoryType(LineFactory.LineType.M, false);
	}
	
	// DragGesture
	function GestureStartDrag(gesture:DragGesture)
	{
		Debug.Log("GestureStartDrag  drag started");
	}
	function GestureMoveDrag(gesture:DragGesture)
	{
		Debug.Log("GestureMoveDrag  drag moving " + gesture.dragPosition);
	}
	function GestureEndDrag(gesture:DragGesture)
	{
		Debug.Log("GestureEndDrag  drag ended");
	}
	
	// LineGesture
	function GestureLineSwipe(gesture:LineGesture)
	{
		Debug.Log("LineGesture  happened " + gesture.swipedLineType);
	}
	function GestureLineSwipeFailure(gesture:LineGesture)
	{
		Debug.Log("LineGesture  finshed on failure " + gesture.swipedLineType);
	}
	
	
	// LongPressGesture
	function GestureLongPress(gesture:LongPressGesture)
	{
		Debug.Log("LongPressGesture  happened " + gesture.longPressTime);
	}

	// PinchGesture		
	function GestureStartPinch (gesture:PinchGesture)
	{
		Debug.Log("PinchGesture  pinch started");
	}
	function GestureMovePinch (gesture:PinchGesture)
	{
		Debug.Log("PinchGesture  pinching " + gesture.pinchAction + " for " + gesture.pinchDirection);
	}
	function GestureEndPinch (gesture:PinchGesture)
	{
		Debug.Log("PinchGesture  pinch ended");
	}
	
	// RotateGesture		
	function GestureStartRotate (gesture:RotateGesture)
	{
		Debug.Log("GestureStartRotate  rotate started");
	}	
	function GestureMoveRotate (gesture:RotateGesture)
	{
		Debug.Log("GestureStartRotate  rotating on axis " + gesture.rotateAxis + " by " + gesture.rotationAngleDelta);
	}	
	function GestureEndRotate (gesture:RotateGesture)
	{
		Debug.Log("GestureStartRotate  rotate ended");
	}

	// SliceGesture		
	function GestureSlice (gesture:SliceGesture)
	{
		Debug.Log("GestureSlice  slice happened " + gesture.sliceDirection);
	}
	
	// SwipeGesture		
	function GestureSwipe(gesture:SwipeGesture)
	{
		Debug.Log("SwipeGesture  swipe happened " + gesture.swipeDirection + " went from " + gesture.startPosition + " to " + gesture.endPosition);
	}
	function GestureSwipeMove(gesture:SwipeGesture)
	{
		Debug.Log("SwipeGesture  move happened " + gesture.swipePosition + " with direction " + gesture.swipeDirection);
	}
	function GestureSwipeEnd(gesture:SwipeGesture)
	{
		Debug.Log("SwipeGesture  end swipe happened " + gesture.endPosition);
	}

	// TapGesture		
	function GestureTap(gesture:TapGesture)
	{
		Debug.Log("TapGesture  tap happened " + gesture.taps);
	}
	

	// TouchGesture
	function GestureStartTouch(gesture:TouchGesture)
	{
		Debug.Log("GestureStartTouch  start touch " + gesture.finger.startPosition);
	}
	function GestureMoveTouch(gesture:TouchGesture)
	{
		Debug.Log("GestureStartTouch  touch moving " + gesture.finger.position);
	}
	function GestureEndTouch(gesture:TouchGesture)
	{
		Debug.Log("GestureEndTouch  end touch " + gesture.finger.endPosition);
	}
	
*/