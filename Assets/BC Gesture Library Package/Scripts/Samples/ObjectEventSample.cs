using UnityEngine;
using System.Collections;

public class ObjectEventSample : MonoBehaviour 
{
	
	void GestureTap(TapGesture gesture) {
		SampleGUI.StatusTextObj = "Tapped object " + gesture.taps + " times with finger " + gesture.finger.Index();
	}
	void GestureLongPress(LongPressGesture gesture) {
		SampleGUI.StatusTextObj = "Long press on object " + gesture.longPressTime + " seconds with finger " + gesture.finger.Index();
	}
	void GestureStartDrag(DragGesture gesture) {
		SampleGUI.StatusTextObj = "Start drag on object with finger " + gesture.finger.Index();
	}
	void GestureEndDrag(DragGesture gesture) {
		SampleGUI.StatusTextObj = "End drag on object with finger " + gesture.finger.Index();
	}
	void GestureSwipe(SwipeGesture gesture) {
		SampleGUI.StatusTextObj = "Swipe on object with finger " + gesture.finger.Index();
	}

}