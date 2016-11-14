using UnityEngine;
using System.Collections;

public class GestureEvents : MonoBehaviour
{
	//  This class has the methods used for all the different Gesture callback events for you the copy
	
	
	void Start()
	{
		// get a gesture object that is on this object.
		LineGesture lineGesture = this.gameObject.GetComponentInChildren<LineGesture>();
		lineGesture.AddLineFactoryType(LineFactory.LineType.M, false);
	}
	
	// DragGesture
	void GestureStartDrag(DragGesture gesture)
	{
		Debug.Log("GestureStartDrag  drag started");
	}
	void GestureMoveDrag(DragGesture gesture)
	{
		Debug.Log("GestureMoveDrag  drag moving " + gesture.dragPosition);
	}
	void GestureEndDrag(DragGesture gesture)
	{
		Debug.Log("GestureEndDrag  drag ended");
	}
	
	// LineGesture
	void GestureLineSwipe(LineGesture gesture)
	{
		Debug.Log("LineGesture  happened " + gesture.swipedLineType);
	}
	void GestureLineSwipeFailure(LineGesture gesture)
	{
		Debug.Log("LineGesture  finshed on failure " + gesture.errorString);
	}
	
	
	// LongPressGesture
	void GestureLongPress(LongPressGesture gesture)
	{
		Debug.Log("LongPressGesture  happened " + gesture.longPressTime);
	}

	// PinchGesture		
	void GestureStartPinch (PinchGesture gesture)
	{
		Debug.Log("PinchGesture  pinch started");
	}
	void GestureMovePinch (PinchGesture gesture)
	{
		Debug.Log("PinchGesture  pinching " + gesture.pinchAction + " for " + gesture.pinchDirection);
	}
	void GestureEndPinch (PinchGesture gesture)
	{
		Debug.Log("PinchGesture  pinch ended");
	}
	
	// RotateGesture		
	void GestureStartRotate (RotateGesture gesture)
	{
		Debug.Log("GestureStartRotate  rotate started");
	}	
	void GestureMoveRotate (RotateGesture gesture)
	{
		Debug.Log("GestureStartRotate  rotating on axis " + gesture.rotateAxis + " by " + gesture.rotationAngleDelta);
	}	
	void GestureEndRotate (RotateGesture gesture)
	{
		Debug.Log("GestureStartRotate  rotate ended");
	}

	// SliceGesture		
	void GestureSlice (SliceGesture gesture)
	{
		Debug.Log("GestureSlice  slice happened " + gesture.sliceDirection);
	}
	
	// SwipeGesture		
	void GestureSwipe(SwipeGesture gesture)
	{
		Debug.Log("SwipeGesture  swipe happened " + gesture.swipeDirection + " went from " + gesture.startPosition + " to " + gesture.endPosition);
	}
	void GestureSwipeMove(SwipeGesture gesture)
	{
		Debug.Log("SwipeGesture  move happened " + gesture.swipePosition + " with direction " + gesture.swipeDirection);
	}
	void GestureSwipeEnd(SwipeGesture gesture)
	{
		Debug.Log("SwipeGesture  end swipe happened " + gesture.endPosition);
	}

	// TapGesture		
	void GestureTap(TapGesture gesture)
	{
		Debug.Log("TapGesture  tap happened " + gesture.taps);
	}
	

	// TouchGesture
	void GestureStartTouch(TouchGesture gesture)
	{
		Debug.Log("GestureStartTouch  start touch " + gesture.finger.startPosition);
	}
	void GestureMoveTouch(TouchGesture gesture)
	{
		Debug.Log("GestureStartTouch  touch moving " + gesture.finger.position);
	}
	void GestureEndTouch(TouchGesture gesture)
	{
		Debug.Log("GestureEndTouch  end touch " + gesture.finger.endPosition);
	}

}

